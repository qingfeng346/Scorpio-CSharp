using Scorpio.Compiler;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Tools;
namespace Scorpio.Runtime {

    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public class ScriptContext {
        private const int ParameterLength = 128;        //函数参数最大数量
        private const int ValueCacheLength = 128;       //函数最大调用层级,超过会堆栈溢出
        private const int StackValueLength = 256;       //堆栈数据最大数量
        private const int VariableValueLength = 128;    //局部变量最大数量
        protected static ScriptValue[] Parameters = new ScriptValue[ParameterLength];               //函数调用共用数组
        protected static ScriptValue[][] VariableValues = new ScriptValue[ValueCacheLength][];      //局部变量数据
        protected static ScriptValue[][] StackValues = new ScriptValue[ValueCacheLength][];         //堆栈数据
        protected static int VariableValueIndex = 0;
        static ScriptContext() {
            for (var i = 0; i < StackValues.Length; ++i) {
                StackValues[i] = new ScriptValue[StackValueLength];
            }
            for (var i = 0; i < VariableValues.Length; ++i) {
                VariableValues[i] = new ScriptValue[VariableValueLength];
            }
        }

        public Script m_script;                                 //脚本类
        private ScriptGlobal m_global;                          //global

        public readonly double[] constDouble;                   //double常量
        public readonly long[] constLong;                       //long常量
        public readonly string[] constString;                   //string常量
        public readonly ScriptContext[] constContexts;          //所有定义的函数 常量
        public int internalCount;                               //内部变量数量

        private string m_Breviary;                              //摘要
        private ScriptFunctionData m_FunctionData;              //函数数据
        private ScriptInstruction[] m_scriptInstructions;       //指令集

        public ScriptContext(Script script, string breviary, ScriptFunctionData functionData, double[] constDouble, long[] constLong, string[] constString, ScriptContext[] constContexts) {
            m_script = script;
            m_global = script.Global;
            this.constDouble = constDouble;
            this.constLong = constLong;
            this.constString = constString;
            this.constContexts = constContexts;
            this.internalCount = functionData.internalCount;

            m_Breviary = breviary;
            m_FunctionData = functionData;
            m_scriptInstructions = functionData.scriptInstructions;
        }
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] internalValues) {
            Logger.debug("执行命令 =>\n" + m_FunctionData.ToString(constDouble, constLong, constString));
            var variableObjects = VariableValues[VariableValueIndex];   //局部变量
            var stackObjects = StackValues[VariableValueIndex++];       //堆栈数据
            variableObjects[0] = thisObject;
            InternalValue[] internalObjects = null;
            if (internalCount > 0) {
                internalObjects = new InternalValue[internalCount];     //内部变量，有外部引用
                for (int i = 0; i < internalCount; ++i) {
                    if (internalValues != null)
                        internalObjects[i] = internalValues[i] ?? new InternalValue();
                    else
                        internalObjects[i] = new InternalValue();
                }
            }
            var stackIndex = -1;                                        //堆栈索引
            var parameterCount = m_FunctionData.parameterCount;         //参数数量
            var param = m_FunctionData.param;                           //是否是变长参数
            if (param) {
                var array = new ScriptArray(m_script);
                for (var i = parameterCount - 1; i < length; ++i) {
                    array.Add(args[i]);
                }
                stackObjects[++stackIndex].scriptValue = array;
                stackObjects[stackIndex].valueType = ScriptValue.scriptValueType;
                for (var i = parameterCount - 2; i >= 0; --i) {
                    stackObjects[++stackIndex] = i >= length ? ScriptValue.Null : args[i];
                }
            } else {
                for (var i = parameterCount - 1; i >= 0; --i) {
                    stackObjects[++stackIndex] = i >= length ? ScriptValue.Null : args[i];
                }
            }
            var parent = ScriptValue.Null;
            var parameters = Parameters;                                //传递参数
            var iInstruction = 0;                                       //当前执行命令索引
            var iInstructionCount = m_scriptInstructions.Length;          //指令数量
            ScriptInstruction instruction = null;
            try {
                while (iInstruction < iInstructionCount) {
                    instruction = m_scriptInstructions[iInstruction++];
                    var opvalue = instruction.opvalue;
                    var opcode = instruction.opcode;
                    byte valueType;
                    int index;
                    switch (instruction.optype) {
                        case OpcodeType.Load:
                            switch (opcode) {
                                case Opcode.LoadConstDouble:
                                    stackObjects[++stackIndex].doubleValue = constDouble[opvalue];
                                    stackObjects[stackIndex].valueType = ScriptValue.doubleValueType;
                                    continue;
                                case Opcode.LoadConstString:
                                    stackObjects[++stackIndex].stringValue = constString[opvalue];
                                    stackObjects[stackIndex].valueType = ScriptValue.stringValueType;
                                    continue;
                                case Opcode.LoadConstNull: stackObjects[++stackIndex].valueType = ScriptValue.nullValueType; continue;
                                case Opcode.LoadConstTrue: stackObjects[++stackIndex].valueType = ScriptValue.trueValueType; continue;
                                case Opcode.LoadConstFalse: stackObjects[++stackIndex].valueType = ScriptValue.falseValueType; continue;
                                case Opcode.LoadConstLong:
                                    stackObjects[++stackIndex].longValue = constLong[opvalue];
                                    stackObjects[stackIndex].valueType = ScriptValue.longValueType;
                                    continue;
                                case Opcode.LoadLocal: {
                                    switch (variableObjects[opvalue].valueType) {
                                        case ScriptValue.scriptValueType:
                                            stackObjects[++stackIndex].scriptValue = variableObjects[opvalue].scriptValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.scriptValueType;
                                            continue;
                                        case ScriptValue.doubleValueType:
                                            stackObjects[++stackIndex].doubleValue = variableObjects[opvalue].doubleValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.doubleValueType;
                                            continue;
                                        case ScriptValue.nullValueType: stackObjects[++stackIndex].valueType = ScriptValue.nullValueType; continue;
                                        case ScriptValue.trueValueType: stackObjects[++stackIndex].valueType = ScriptValue.trueValueType; continue;
                                        case ScriptValue.falseValueType: stackObjects[++stackIndex].valueType = ScriptValue.falseValueType; continue;
                                        case ScriptValue.stringValueType:
                                            stackObjects[++stackIndex].stringValue = variableObjects[opvalue].stringValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.stringValueType;
                                            continue;
                                        case ScriptValue.longValueType:
                                            stackObjects[++stackIndex].longValue = variableObjects[opvalue].longValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.longValueType;
                                            continue;
                                        case ScriptValue.objectValueType:
                                            stackObjects[++stackIndex].objectValue = variableObjects[opvalue].objectValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.objectValueType;
                                            continue;
                                        default: throw new ExecutionException("LoadLocal : 未知错误数据类型 : " + variableObjects[opvalue].valueType);
                                    }
                                }
                                case Opcode.LoadInternal: {
                                    switch (internalObjects[opvalue].value.valueType) {
                                        case ScriptValue.scriptValueType:
                                            stackObjects[++stackIndex].scriptValue = internalObjects[opvalue].value.scriptValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.scriptValueType;
                                            continue;
                                        case ScriptValue.doubleValueType:
                                            stackObjects[++stackIndex].doubleValue = internalObjects[opvalue].value.doubleValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.doubleValueType;
                                            continue;
                                        case ScriptValue.nullValueType: stackObjects[++stackIndex].valueType = ScriptValue.nullValueType; continue;
                                        case ScriptValue.trueValueType: stackObjects[++stackIndex].valueType = ScriptValue.trueValueType; continue;
                                        case ScriptValue.falseValueType: stackObjects[++stackIndex].valueType = ScriptValue.falseValueType; continue;
                                        case ScriptValue.stringValueType:
                                            stackObjects[++stackIndex].stringValue = internalObjects[opvalue].value.stringValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.stringValueType;
                                            continue;
                                        case ScriptValue.longValueType:
                                            stackObjects[++stackIndex].longValue = internalObjects[opvalue].value.longValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.longValueType;
                                            continue;
                                        case ScriptValue.objectValueType:
                                            stackObjects[++stackIndex].objectValue = internalObjects[opvalue].value.objectValue;
                                            stackObjects[stackIndex].valueType = ScriptValue.objectValueType;
                                            continue;
                                        default: throw new ExecutionException("LoadInternal : 未知错误数据类型 : " + internalObjects[opvalue].value.valueType);
                                    }
                                }
                                case Opcode.LoadValue:
                                    parent = stackObjects[stackIndex];
                                    stackObjects[stackIndex] = stackObjects[stackIndex].GetValueByIndex(opvalue, m_script);
                                    continue;
                                case Opcode.LoadValueString:
                                    parent = stackObjects[stackIndex];
                                    stackObjects[stackIndex] = stackObjects[stackIndex].GetValue(constString[opvalue], m_script);
                                    continue;
                                case Opcode.LoadValueObject: {
                                    parent = stackObjects[stackIndex - 1];
                                    valueType = stackObjects[stackIndex].valueType;
                                    switch (valueType) {
                                        case ScriptValue.stringValueType: stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].stringValue, m_script); break;
                                        case ScriptValue.doubleValueType: stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].doubleValue, m_script); break;
                                        case ScriptValue.longValueType: stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].longValue, m_script); break;
                                        case ScriptValue.scriptValueType: stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].scriptValue, m_script); break;
                                        case ScriptValue.objectValueType: stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].objectValue, m_script); break;
                                        default: throw new ExecutionException("不支持当前类型获取变量 : " + valueType);
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.LoadValueObjectDup: {
                                    parent = stackObjects[stackIndex - 1];
                                    valueType = stackObjects[stackIndex].valueType;
                                    switch (valueType) {
                                        case ScriptValue.stringValueType: stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].stringValue, m_script); break;
                                        case ScriptValue.doubleValueType: stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].doubleValue, m_script); break;
                                        case ScriptValue.longValueType: stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].longValue, m_script); break;
                                        case ScriptValue.scriptValueType: stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].scriptValue, m_script); break;
                                        case ScriptValue.objectValueType: stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].objectValue, m_script); break;
                                        default: throw new ExecutionException("不支持当前类型获取变量 : " + valueType);
                                    }
                                    ++stackIndex;
                                    continue;
                                }
                                case Opcode.LoadGlobal:
                                    stackObjects[++stackIndex] = m_global.GetValueByIndex(opvalue);
                                    continue;
                                case Opcode.LoadGlobalString: {
                                    stackObjects[++stackIndex] = m_global.GetValue(constString[opvalue]);
                                    instruction.SetOpcode(Opcode.LoadGlobal, m_global.GetIndex(constString[opvalue]));
                                    continue;
                                }
                                case Opcode.CopyStackTop: {
                                    index = stackIndex;
                                    stackObjects[++stackIndex] = stackObjects[index];
                                    continue;
                                }
                                case Opcode.CopyStackTopIndex: {
                                    index = stackIndex - opvalue;
                                    stackObjects[++stackIndex] = stackObjects[index];
                                    continue;
                                }
                            }
                            continue;
                        case OpcodeType.Store:
                            switch (opcode) {
                                case Opcode.StoreLocal: {
                                    index = stackIndex--;
                                    switch (stackObjects[index].valueType) {
                                        case ScriptValue.scriptValueType:
                                            variableObjects[opvalue].scriptValue = stackObjects[index].scriptValue;
                                            variableObjects[opvalue].valueType = ScriptValue.scriptValueType;
                                            continue;
                                        case ScriptValue.doubleValueType:
                                            variableObjects[opvalue].doubleValue = stackObjects[index].doubleValue;
                                            variableObjects[opvalue].valueType = ScriptValue.doubleValueType;
                                            continue;
                                        case ScriptValue.nullValueType: variableObjects[opvalue].valueType = ScriptValue.nullValueType; continue;
                                        case ScriptValue.trueValueType: variableObjects[opvalue].valueType = ScriptValue.trueValueType; continue;
                                        case ScriptValue.falseValueType: variableObjects[opvalue].valueType = ScriptValue.falseValueType; continue;
                                        case ScriptValue.stringValueType:
                                            variableObjects[opvalue].stringValue = stackObjects[index].stringValue;
                                            variableObjects[opvalue].valueType = ScriptValue.stringValueType;
                                            continue;
                                        case ScriptValue.longValueType:
                                            variableObjects[opvalue].longValue = stackObjects[index].longValue;
                                            variableObjects[opvalue].valueType = ScriptValue.longValueType;
                                            continue;
                                        case ScriptValue.objectValueType:
                                            variableObjects[opvalue].objectValue = stackObjects[index].objectValue;
                                            variableObjects[opvalue].valueType = ScriptValue.objectValueType;
                                            continue;
                                        default: throw new ExecutionException("StoreLocal : 未知错误数据类型 : " + stackObjects[index].valueType);
                                    }
                                }
                                case Opcode.StoreInternal: {
                                    index = stackIndex--;
                                    switch (stackObjects[index].valueType) {
                                        case ScriptValue.scriptValueType:
                                            internalObjects[opvalue].value.scriptValue = stackObjects[index].scriptValue;
                                            internalObjects[opvalue].value.valueType = ScriptValue.scriptValueType;
                                            continue;
                                        case ScriptValue.doubleValueType:
                                            internalObjects[opvalue].value.doubleValue = stackObjects[index].doubleValue;
                                            internalObjects[opvalue].value.valueType = ScriptValue.doubleValueType;
                                            continue;
                                        case ScriptValue.nullValueType: internalObjects[opvalue].value.valueType = ScriptValue.nullValueType; continue;
                                        case ScriptValue.trueValueType: internalObjects[opvalue].value.valueType = ScriptValue.trueValueType; continue;
                                        case ScriptValue.falseValueType: internalObjects[opvalue].value.valueType = ScriptValue.falseValueType; continue;
                                        case ScriptValue.stringValueType:
                                            internalObjects[opvalue].value.stringValue = stackObjects[index].stringValue;
                                            internalObjects[opvalue].value.valueType = ScriptValue.stringValueType;
                                            continue;
                                        case ScriptValue.longValueType:
                                            internalObjects[opvalue].value.longValue = stackObjects[index].longValue;
                                            internalObjects[opvalue].value.valueType = ScriptValue.longValueType;
                                            continue;
                                        case ScriptValue.objectValueType:
                                            internalObjects[opvalue].value.objectValue = stackObjects[index].objectValue;
                                            internalObjects[opvalue].value.valueType = ScriptValue.objectValueType;
                                            continue;
                                        default: throw new ExecutionException("StoreInternal : 未知错误数据类型 : " + stackObjects[index].valueType);
                                    }
                                }
                                case Opcode.StoreGlobal: m_global.SetValueByIndex(opvalue, stackObjects[stackIndex--]); continue;
                                case Opcode.StoreGlobalString:
                                    m_global.SetValue(constString[opvalue], stackObjects[stackIndex--]);
                                    instruction.SetOpcode(Opcode.StoreGlobal, m_global.GetIndex(constString[opvalue]));
                                    continue;
                                case Opcode.StoreValue:
                                    stackObjects[stackIndex - 1].SetValueByIndex(opvalue, stackObjects[stackIndex]);
                                    stackIndex -= 2;
                                    continue;
                                case Opcode.StoreValueString:
                                    stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex]);
                                    stackIndex -= 2;
                                    continue;
                                case Opcode.StoreValueObject:
                                    valueType = stackObjects[stackIndex - 1].valueType;
                                    switch (valueType) {
                                        case ScriptValue.stringValueType: stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].stringValue, stackObjects[stackIndex]); break;
                                        case ScriptValue.doubleValueType: stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].doubleValue, stackObjects[stackIndex]); break;
                                        case ScriptValue.longValueType: stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].longValue, stackObjects[stackIndex]); break;
                                        case ScriptValue.scriptValueType: stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].scriptValue, stackObjects[stackIndex]); break;
                                        case ScriptValue.objectValueType: stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].objectValue, stackObjects[stackIndex]); break;
                                        default: throw new ExecutionException("不支持当前类型设置变量 : " + valueType);
                                    }
                                    stackIndex -= 3;
                                    continue;
                            }
                            continue;
                        case OpcodeType.Compute:
                            switch (opcode) {
                                case Opcode.Plus: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.stringValueType) {
                                        stackObjects[index].stringValue += stackObjects[stackIndex].ToString();
                                    } else if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Plus(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].doubleValue += stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue += stackObjects[stackIndex].longValue;
                                                break;
                                            default: throw new ExecutionException("【+】运算符不支持当前类型");
                                        }
                                    } else if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                        stackObjects[index].stringValue = stackObjects[index].ToString() + stackObjects[stackIndex].stringValue;
                                        stackObjects[index].valueType = ScriptValue.stringValueType;
                                    } else {
                                        throw new ExecutionException("【+】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Minus: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Minus(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].doubleValue -= stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue -= stackObjects[stackIndex].longValue;
                                                break;
                                            default: throw new ExecutionException("【-】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【-】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Multiply: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Multiply(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].doubleValue *= stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue *= stackObjects[stackIndex].longValue;
                                                break;
                                            default: throw new ExecutionException("【*】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【*】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Divide: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Divide(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].doubleValue /= stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue /= stackObjects[stackIndex].longValue;
                                                break;
                                            default: throw new ExecutionException("【/】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【/】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Modulo: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Modulo(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].doubleValue %= stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue %= stackObjects[stackIndex].longValue;
                                                break;
                                            default: throw new ExecutionException("【%】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【%】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.InclusiveOr: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.InclusiveOr(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType && valueType == ScriptValue.longValueType) {
                                        stackObjects[index].longValue |= stackObjects[stackIndex].longValue;
                                    } else {
                                        throw new ExecutionException("【|】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Combine: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Combine(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType && valueType == ScriptValue.longValueType) {
                                        stackObjects[index].longValue &= stackObjects[stackIndex].longValue;
                                    } else {
                                        throw new ExecutionException("【&】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.XOR: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.XOR(stackObjects[stackIndex]);
                                    } else if (valueType == stackObjects[stackIndex].valueType && valueType == ScriptValue.longValueType) {
                                        stackObjects[index].longValue ^= stackObjects[stackIndex].longValue;
                                    } else {
                                        throw new ExecutionException("【^】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Shi: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Shi(stackObjects[stackIndex]);
                                    } else if (valueType == ScriptValue.longValueType) {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].longValue <<= System.Convert.ToInt32(stackObjects[stackIndex].doubleValue);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue <<= System.Convert.ToInt32(stackObjects[stackIndex].longValue);
                                                break;
                                            default:
                                                throw new ExecutionException("【<<】运算符值支持long和number间的运算");
                                        }
                                    } else {
                                        throw new ExecutionException("【<<】运算符值支持long和number间的运算");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Shr: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index] = stackObjects[index].scriptValue.Shi(stackObjects[stackIndex]);
                                    } else if (valueType == ScriptValue.longValueType) {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].longValue >>= System.Convert.ToInt32(stackObjects[stackIndex].doubleValue);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].longValue >>= System.Convert.ToInt32(stackObjects[stackIndex].longValue);
                                                break;
                                            default:
                                                throw new ExecutionException("【>>】运算符值支持long和number间的运算");
                                        }
                                    } else {
                                        throw new ExecutionException("【>>】运算符值支持long和number间的运算");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.FlagNot: {
                                    valueType = stackObjects[stackIndex].valueType;
                                    switch (valueType) {
                                        case ScriptValue.trueValueType: stackObjects[stackIndex].valueType = ScriptValue.falseValueType; continue;
                                        case ScriptValue.falseValueType: stackObjects[stackIndex].valueType = ScriptValue.trueValueType; continue;
                                        default: throw new ExecutionException("当前数据类型不支持取反操作 : " + valueType);
                                    }
                                }
                                case Opcode.FlagMinus: {
                                    valueType = stackObjects[stackIndex].valueType;
                                    switch (valueType) {
                                        case ScriptValue.doubleValueType: stackObjects[stackIndex].doubleValue = -stackObjects[stackIndex].doubleValue; continue;
                                        case ScriptValue.longValueType: stackObjects[stackIndex].longValue = -stackObjects[stackIndex].longValue; continue;
                                        default: throw new ExecutionException("当前数据类型不支持取负操作 : " + valueType);
                                    }
                                }
                                case Opcode.FlagNegative: {
                                    if (stackObjects[stackIndex].valueType == ScriptValue.longValueType) {
                                        stackObjects[stackIndex].longValue = ~stackObjects[stackIndex].longValue;
                                        continue;
                                    } else {
                                        throw new ExecutionException("当前数据类型不支持取非操作 : " + stackObjects[stackIndex].valueType);
                                    }
                                }
                            }
                            continue;
                        case OpcodeType.Compare:
                            switch (opcode) {
                                case Opcode.Less: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.Less(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue < stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue < stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default: throw new ExecutionException("【<】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【<】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.LessOrEqual: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.LessOrEqual(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue <= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue <= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default: throw new ExecutionException("【<=】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【<=】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Greater: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.Greater(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue > stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue > stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default: throw new ExecutionException("【>】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【>】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.GreaterOrEqual: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.GreaterOrEqual(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue >= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue >= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default: throw new ExecutionException("【>=】运算符不支持当前类型");
                                        }
                                    } else {
                                        throw new ExecutionException("【>=】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.Equal: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.Equals(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue == stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue == stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.stringValueType:
                                                stackObjects[index].valueType = stackObjects[index].stringValue == stackObjects[stackIndex].stringValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[index].valueType = stackObjects[index].objectValue.Equals(stackObjects[stackIndex].objectValue) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:    //剩余的就是 null true false 类型相同则相同
                                                stackObjects[index].valueType = ScriptValue.trueValueType;
                                                break;
                                        }
                                    } else {
                                        stackObjects[index].valueType = ScriptValue.falseValueType;
                                        // throw new ExecutionException("【==】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.NotEqual: {
                                    index = stackIndex - 1;
                                    valueType = stackObjects[index].valueType;
                                    if (valueType == ScriptValue.scriptValueType) {
                                        stackObjects[index].valueType = stackObjects[index].scriptValue.Equals(stackObjects[stackIndex]) ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                    } else if (valueType == stackObjects[stackIndex].valueType) {
                                        switch (valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[index].valueType = stackObjects[index].doubleValue != stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[index].valueType = stackObjects[index].longValue != stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.stringValueType:
                                                stackObjects[index].valueType = stackObjects[index].stringValue != stackObjects[stackIndex].stringValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[index].valueType = stackObjects[index].objectValue.Equals(stackObjects[stackIndex].objectValue) ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                break;
                                            default:        //剩余的就是 null true false 类型相同则相同
                                                stackObjects[index].valueType = ScriptValue.falseValueType;
                                                break;
                                        }
                                    } else {
                                        stackObjects[index].valueType = ScriptValue.trueValueType;
                                        // throw new ExecutionException("【!=】运算符必须两边数据类型一致,或者不支持此操作符");
                                    }
                                    --stackIndex;
                                    continue;
                                }
                            }
                            continue;
                        case OpcodeType.Jump:
                            switch (opcode) {
                                case Opcode.Jump: iInstruction = opvalue; continue;
                                case Opcode.Pop: --stackIndex; continue;
                                case Opcode.PopNumber: stackIndex -= opvalue; continue;
                                case Opcode.Call: {
                                    var value = stackObjects[stackIndex--];
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        parameters[i] = stackObjects[stackIndex--];
                                    }
                                    value.Call(ScriptValue.Null, parameters, opvalue);
                                    continue;
                                }
                                case Opcode.CallRet: {
                                    var value = stackObjects[stackIndex--];
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        parameters[i] = stackObjects[stackIndex--];
                                    }
                                    stackObjects[++stackIndex] = value.Call(ScriptValue.Null, parameters, opvalue);
                                    continue;
                                }
                                case Opcode.CallEach: {
                                    index = stackIndex - 1;
                                    var value = stackObjects[stackIndex];
                                    stackObjects[++stackIndex] = value.Call(stackObjects[index]);
                                    continue;
                                }
                                case Opcode.CallVi: {
                                    var value = stackObjects[stackIndex--];
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        parameters[i] = stackObjects[stackIndex--];
                                    }
                                    value.Call(parent, parameters, opvalue);
                                    continue;
                                }
                                case Opcode.CallViRet: {
                                    var value = stackObjects[stackIndex--];
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        parameters[i] = stackObjects[stackIndex--];
                                    }
                                    stackObjects[++stackIndex] = value.Call(parent, parameters, opvalue);
                                    continue;
                                }
                                case Opcode.TrueTo: {
                                    valueType = stackObjects[stackIndex].valueType;
                                    if (valueType != ScriptValue.falseValueType && valueType != ScriptValue.nullValueType) {
                                        iInstruction = opvalue;
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.FalseTo: {
                                    valueType = stackObjects[stackIndex].valueType;
                                    if (valueType == ScriptValue.falseValueType || valueType == ScriptValue.nullValueType) {
                                        iInstruction = opvalue;
                                    }
                                    --stackIndex;
                                    continue;
                                }
                                case Opcode.TrueLoadTrue: if (stackObjects[stackIndex].valueType == ScriptValue.trueValueType) { iInstruction = opvalue; } else { --stackIndex; } continue;
                                case Opcode.FalseLoadFalse: if (stackObjects[stackIndex].valueType == ScriptValue.falseValueType) { iInstruction = opvalue; } else { --stackIndex; } continue;
                                case Opcode.RetNone: return ScriptValue.Null;
                                case Opcode.Ret: return stackObjects[stackIndex--];
                            }
                            continue;
                        case OpcodeType.New:
                            switch (opcode) {
                                case Opcode.NewFunction: {
                                    var functionData = constContexts[opvalue];
                                    var function = new ScriptScriptFunction(functionData);
                                    for (int i = 0; i < functionData.m_FunctionData.internals.Length; ++i) {
                                        var internalIndex = functionData.m_FunctionData.internals[i];
                                        function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                    }
                                    stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                    stackObjects[stackIndex].scriptValue = function;
                                    continue;
                                }
                                case Opcode.NewArray: {
                                    var array = new ScriptArray(m_script);
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        array.Add(stackObjects[stackIndex - i]);
                                    }
                                    stackIndex -= opvalue;
                                    stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                    stackObjects[stackIndex].scriptValue = array;
                                    continue;
                                }
                                case Opcode.NewMap: {
                                    var map = new ScriptMap(m_script);
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        map.SetValue(stackObjects[stackIndex - i].stringValue, stackObjects[stackIndex - i - opvalue]);
                                    }
                                    stackIndex -= opvalue * 2;
                                    stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                    stackObjects[stackIndex].scriptValue = map;
                                    continue;
                                }
                                case Opcode.NewMapObject: {
                                    var map = new ScriptMap(m_script);
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        map.SetValue(stackObjects[stackIndex - i].Value, stackObjects[stackIndex - i - opvalue]);
                                    }
                                    stackIndex -= opvalue * 2;
                                    stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                    stackObjects[stackIndex].scriptValue = map;
                                    continue;
                                }
                                case Opcode.NewType:
                                case Opcode.NewTypeParent: {
                                    var parentType = opcode == Opcode.NewTypeParent ? stackObjects[stackIndex - opvalue * 2 - 1] : m_script.TypeObjectValue;
                                    var className = stackObjects[stackIndex - opvalue * 2].stringValue;
                                    var type = new ScriptType(className, parentType);
                                    for (var i = opvalue - 1; i >= 0; --i) {
                                        type.SetValue(stackObjects[stackIndex - i].stringValue, stackObjects[stackIndex - i - opvalue]);
                                    }
                                    stackIndex -= (opvalue * 2 + (opcode == Opcode.NewTypeParent ? 2 : 1));
                                    m_global.SetValue(className, new ScriptValue(type));
                                    continue;
                                }
                            }
                            continue;
                    }
                }
            } catch (ExecutionException e) {
                throw new ExecutionStackException($"{m_Breviary}:{instruction.line}({iInstruction})\n    {e.ToString()}");
            } catch (ExecutionStackException e) {
                throw new ExecutionStackException($"{m_Breviary}:{instruction.line}({iInstruction})\n    {e.ToString()}");
            } catch (System.Exception e) {
                throw new ExecutionException($"{m_Breviary}:{instruction.line}({iInstruction}) : {e.ToString()}");
            } finally {
                --VariableValueIndex;
                Logger.debug(stackIndex != -1, "堆栈数据未清空，有泄露情况 : " + stackIndex);
            }
            return ScriptValue.Null;
        }
    }
}
