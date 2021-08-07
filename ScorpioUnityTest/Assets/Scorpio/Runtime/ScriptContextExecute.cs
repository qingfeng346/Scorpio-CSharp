using System.Collections;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Instruction;
using Scorpio.Tools;
namespace Scorpio.Runtime {
    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public partial class ScriptContext {
#if EXECUTE_COROUTINE && EXECUTE_BASE
        public IEnumerator ExecuteCoroutine(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] internalValues, ScriptType baseType) {
#elif EXECUTE_COROUTINE
        public IEnumerator ExecuteCoroutine(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] internalValues) {
#elif EXECUTE_BASE
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] internalValues, ScriptType baseType) {
#else
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] internalValues) {
#endif

#if SCORPIO_DEBUG
            //Logger.debug($"执行命令 =>\n{m_FunctionData.ToString(constDouble, constLong, constString)}");
            if (VariableValueIndex < 0 || VariableValueIndex >= ValueCacheLength) {
                throw new ExecutionException("Stack overflow : " + VariableValueIndex);
            }
#endif
#if SCORPIO_THREAD
            var currentThread = System.Threading.Thread.CurrentThread;
            if (currentThread.ManagedThreadId != m_script.MainThreadId) {
                throw new ExecutionException($"only run script on mainthread : {m_script.MainThreadId} - {currentThread.ManagedThreadId}({currentThread.Name})");
            }
#endif
            var variableObjects = VariableValues[VariableValueIndex]; //局部变量
            var tryStack = TryStackValues[VariableValueIndex]; //try catch
            var stackObjects = StackValues[VariableValueIndex++]; //堆栈数据
            variableObjects[0] = thisObject;
            InternalValue[] internalObjects = null;
            if (internalCount > 0) {
                internalObjects = new InternalValue[internalCount]; //内部变量，有外部引用
                if (internalValues == null) {
                    for (int i = 0; i < internalCount; ++i) {
                        internalObjects[i] = new InternalValue();
                    }
                } else {
                    for (int i = 0; i < internalCount; ++i) {
                        internalObjects[i] = internalValues[i] ?? new InternalValue();
                    }
                }
            }
            var stackIndex = -1; //堆栈索引
#if !EXECUTE_COROUTINE
            var tryIndex = -1; //try索引
#endif
            var parameterCount = m_FunctionData.parameterCount; //参数数量
            //是否是变长参数
            if (m_FunctionData.param) {
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
            var parameters = ScriptValue.Parameters; //传递参数
            var iInstruction = 0; //当前执行命令索引
            var iInstructionCount = m_scriptInstructions.Length; //指令数量
            byte tempValueType; //临时存储
            int tempIndex; //临时存储
            ScriptInstruction instruction = null;
            try {
#if !EXECUTE_COROUTINE
            KeepOn: 
                try {
#endif
                    while (iInstruction < iInstructionCount) {
                        instruction = m_scriptInstructions[iInstruction++];
                        var opvalue = instruction.opvalue;
                        var opcode = instruction.opcode;
                        switch (instruction.optype) {
                            case OpcodeType.Load:
                                switch (opcode) {
                                    case Opcode.LoadConstDouble: {
                                        stackObjects[++stackIndex].doubleValue = constDouble[opvalue];
                                        stackObjects[stackIndex].valueType = ScriptValue.doubleValueType;
                                        continue;
                                    }
                                    case Opcode.LoadConstString: {
                                        stackObjects[++stackIndex].stringValue = constString[opvalue];
                                        stackObjects[stackIndex].valueType = ScriptValue.stringValueType;
                                        continue;
                                    }
                                    case Opcode.LoadConstNull: {
                                        stackObjects[++stackIndex].valueType = ScriptValue.nullValueType;
                                        continue;
                                    }
                                    case Opcode.LoadConstTrue: {
                                        stackObjects[++stackIndex].valueType = ScriptValue.trueValueType;
                                        continue;
                                    }
                                    case Opcode.LoadConstFalse: {
                                        stackObjects[++stackIndex].valueType = ScriptValue.falseValueType;
                                        continue;
                                    }
                                    case Opcode.LoadConstLong: {
                                        stackObjects[++stackIndex].longValue = constLong[opvalue];
                                        stackObjects[stackIndex].valueType = ScriptValue.longValueType;
                                        continue;
                                    }
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
                                            case ScriptValue.nullValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.falseValueType;
                                                continue;
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
                                            default:
                                                throw new ExecutionException($"LoadLocal : 未知错误数据类型 : {variableObjects[opvalue].ValueTypeName}");
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
                                            case ScriptValue.nullValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                stackObjects[++stackIndex].valueType = ScriptValue.falseValueType;
                                                continue;
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
                                            default:
                                                throw new ExecutionException($"LoadInternal : 未知错误数据类型 : {internalObjects[opvalue].value.valueType}");
                                        }
                                    }
                                    case Opcode.LoadValue: {
                                        stackObjects[stackIndex] = stackObjects[stackIndex].GetValueByIndex(opvalue, m_script);
                                        continue;
                                    }
                                    case Opcode.LoadValueString: {
                                        stackObjects[stackIndex] = stackObjects[stackIndex].GetValue(constString[opvalue], m_script);
                                        continue;
                                    }
                                    case Opcode.LoadValueObject: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.stringValueType:
                                                stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].stringValue, m_script);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].doubleValue);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].longValue);
                                                break;
                                            case ScriptValue.scriptValueType:
                                                stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].scriptValue);
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[stackIndex - 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].objectValue);
                                                break;
                                            default:
                                                throw new ExecutionException($"不支持当前类型获取变量 LoadValueObject : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case Opcode.LoadValueObjectDup: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.stringValueType:
                                                stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].stringValue, m_script);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].doubleValue);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].longValue);
                                                break;
                                            case ScriptValue.scriptValueType:
                                                stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].scriptValue);
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[stackIndex + 1] = stackObjects[stackIndex - 1].GetValue(stackObjects[stackIndex].objectValue);
                                                break;
                                            default:
                                                throw new ExecutionException($"不支持当前类型获取变量 LoadValueObjectDup : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                        ++stackIndex;
                                        continue;
                                    }
                                    case Opcode.LoadGlobal: {
                                        stackObjects[++stackIndex] = m_global.GetValueByIndex(opvalue);
                                        continue;
                                    }
                                    case Opcode.LoadGlobalString: {
                                        stackObjects[++stackIndex] = m_global.GetValue(constString[opvalue]);
                                        instruction.SetOpcode(Opcode.LoadGlobal, m_global.GetIndex(constString[opvalue]));
                                        continue;
                                    }
                                    case Opcode.CopyStackTop: {
                                        stackObjects[++stackIndex] = stackObjects[stackIndex - 1];
                                        continue;
                                    }
                                    case Opcode.CopyStackTopIndex: {
                                        stackObjects[++stackIndex] = stackObjects[stackIndex - opvalue - 1];
                                        continue;
                                    }
                                    case Opcode.LoadBase: {
#if EXECUTE_BASE
                                        stackObjects[++stackIndex] = baseType.Prototype;
#else
                                        stackObjects[++stackIndex] = thisObject.Get<ScriptInstance>().Prototype.Get<ScriptType>().Prototype;
#endif
                                        continue;
                                    }
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                            case OpcodeType.Store:
                                switch (opcode) {
                                    //-------------下面为 = *= -= 等赋值操作, 压入计算结果
                                    case Opcode.StoreLocalAssign: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.scriptValueType:
                                                variableObjects[opvalue].scriptValue = stackObjects[stackIndex].scriptValue;
                                                variableObjects[opvalue].valueType = ScriptValue.scriptValueType;
                                                continue;
                                            case ScriptValue.doubleValueType:
                                                variableObjects[opvalue].doubleValue = stackObjects[stackIndex].doubleValue;
                                                variableObjects[opvalue].valueType = ScriptValue.doubleValueType;
                                                continue;
                                            case ScriptValue.nullValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.falseValueType;
                                                continue;
                                            case ScriptValue.stringValueType:
                                                variableObjects[opvalue].stringValue = stackObjects[stackIndex].stringValue;
                                                variableObjects[opvalue].valueType = ScriptValue.stringValueType;
                                                continue;
                                            case ScriptValue.longValueType:
                                                variableObjects[opvalue].longValue = stackObjects[stackIndex].longValue;
                                                variableObjects[opvalue].valueType = ScriptValue.longValueType;
                                                continue;
                                            case ScriptValue.objectValueType:
                                                variableObjects[opvalue].objectValue = stackObjects[stackIndex].objectValue;
                                                variableObjects[opvalue].valueType = ScriptValue.objectValueType;
                                                continue;
                                            default:
                                                throw new ExecutionException($"StoreLocalAssign : 未知错误数据类型 : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.StoreInternalAssign: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.scriptValueType:
                                                internalObjects[opvalue].value.scriptValue = stackObjects[stackIndex].scriptValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.scriptValueType;
                                                continue;
                                            case ScriptValue.doubleValueType:
                                                internalObjects[opvalue].value.doubleValue = stackObjects[stackIndex].doubleValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.doubleValueType;
                                                continue;
                                            case ScriptValue.nullValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.falseValueType;
                                                continue;
                                            case ScriptValue.stringValueType:
                                                internalObjects[opvalue].value.stringValue = stackObjects[stackIndex].stringValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.stringValueType;
                                                continue;
                                            case ScriptValue.longValueType:
                                                internalObjects[opvalue].value.longValue = stackObjects[stackIndex].longValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.longValueType;
                                                continue;
                                            case ScriptValue.objectValueType:
                                                internalObjects[opvalue].value.objectValue = stackObjects[stackIndex].objectValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.objectValueType;
                                                continue;
                                            default:
                                                throw new ExecutionException("StoreInternalAssign : 未知错误数据类型 : " + stackObjects[stackIndex].valueType);
                                        }
                                    }
                                    case Opcode.StoreValueStringAssign: {
                                        tempIndex = stackIndex;
                                        stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex]);
                                        stackObjects[--stackIndex] = stackObjects[tempIndex];
                                        continue;
                                    }
                                    case Opcode.StoreValueObjectAssign: {
                                        tempIndex = stackIndex;
                                        switch (stackObjects[stackIndex - 1].valueType) {
                                            case ScriptValue.stringValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].stringValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].doubleValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].longValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.scriptValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].scriptValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].objectValue, stackObjects[stackIndex]);
                                                break;
                                            default:
                                                throw new ExecutionException($"类型[{stackObjects[stackIndex-2].ValueTypeName}]不支持设置变量:{stackObjects[stackIndex-1].ValueTypeName}");
                                        }
                                        stackObjects[stackIndex -= 2] = stackObjects[tempIndex];
                                        continue;
                                    }
                                    case Opcode.StoreGlobalAssign: {
                                        m_global.SetValueByIndex(opvalue, stackObjects[stackIndex]);
                                        continue;
                                    }
                                    case Opcode.StoreGlobalStringAssign: {
                                        m_global.SetValue(constString[opvalue], stackObjects[stackIndex]);
                                        instruction.SetOpcode(Opcode.StoreGlobalAssign, m_global.GetIndex(constString[opvalue]));
                                        continue;
                                    }
                                    case Opcode.StoreValueAssign: {
                                        tempIndex = stackIndex;
                                        stackObjects[stackIndex - 1].SetValueByIndex(opvalue, stackObjects[stackIndex]);
                                        stackObjects[--stackIndex] = stackObjects[tempIndex];
                                        continue;
                                    }

                                    //-----------------下面为普通赋值操作 不压入结果
                                    case Opcode.StoreLocal: {
                                        tempIndex = stackIndex--;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.scriptValueType:
                                                variableObjects[opvalue].scriptValue = stackObjects[tempIndex].scriptValue;
                                                variableObjects[opvalue].valueType = ScriptValue.scriptValueType;
                                                continue;
                                            case ScriptValue.doubleValueType:
                                                variableObjects[opvalue].doubleValue = stackObjects[tempIndex].doubleValue;
                                                variableObjects[opvalue].valueType = ScriptValue.doubleValueType;
                                                continue;
                                            case ScriptValue.nullValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                variableObjects[opvalue].valueType = ScriptValue.falseValueType;
                                                continue;
                                            case ScriptValue.stringValueType:
                                                variableObjects[opvalue].stringValue = stackObjects[tempIndex].stringValue;
                                                variableObjects[opvalue].valueType = ScriptValue.stringValueType;
                                                continue;
                                            case ScriptValue.longValueType:
                                                variableObjects[opvalue].longValue = stackObjects[tempIndex].longValue;
                                                variableObjects[opvalue].valueType = ScriptValue.longValueType;
                                                continue;
                                            case ScriptValue.objectValueType:
                                                variableObjects[opvalue].objectValue = stackObjects[tempIndex].objectValue;
                                                variableObjects[opvalue].valueType = ScriptValue.objectValueType;
                                                continue;
                                            default:
                                                throw new ExecutionException($"StoreLocal : 未知错误数据类型 : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.StoreInternal: {
                                        tempIndex = stackIndex--;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.scriptValueType:
                                                internalObjects[opvalue].value.scriptValue = stackObjects[tempIndex].scriptValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.scriptValueType;
                                                continue;
                                            case ScriptValue.doubleValueType:
                                                internalObjects[opvalue].value.doubleValue = stackObjects[tempIndex].doubleValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.doubleValueType;
                                                continue;
                                            case ScriptValue.nullValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.nullValueType;
                                                continue;
                                            case ScriptValue.trueValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.trueValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                                internalObjects[opvalue].value.valueType = ScriptValue.falseValueType;
                                                continue;
                                            case ScriptValue.stringValueType:
                                                internalObjects[opvalue].value.stringValue = stackObjects[tempIndex].stringValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.stringValueType;
                                                continue;
                                            case ScriptValue.longValueType:
                                                internalObjects[opvalue].value.longValue = stackObjects[tempIndex].longValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.longValueType;
                                                continue;
                                            case ScriptValue.objectValueType:
                                                internalObjects[opvalue].value.objectValue = stackObjects[tempIndex].objectValue;
                                                internalObjects[opvalue].value.valueType = ScriptValue.objectValueType;
                                                continue;
                                            default:
                                                throw new ExecutionException("StoreInternal : 未知错误数据类型 : " + stackObjects[tempIndex].valueType);
                                        }
                                    }
                                    case Opcode.StoreValueString: {
                                        stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex]);
                                        stackIndex -= 2;
                                        continue;
                                    }
                                    case Opcode.StoreValueObject: {
                                        switch (stackObjects[stackIndex - 1].valueType) {
                                            case ScriptValue.stringValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].stringValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].doubleValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.longValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].longValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.scriptValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].scriptValue, stackObjects[stackIndex]);
                                                break;
                                            case ScriptValue.objectValueType:
                                                stackObjects[stackIndex - 2].SetValue(stackObjects[stackIndex - 1].objectValue, stackObjects[stackIndex]);
                                                break;
                                            default:
                                                throw new ExecutionException($"类型[{stackObjects[stackIndex-2].ValueTypeName}]不支持设置变量:{stackObjects[stackIndex-1].ValueTypeName}");
                                        }
                                        stackIndex -= 3;
                                        continue;
                                    }
                                    case Opcode.StoreGlobal: {
                                        m_global.SetValueByIndex(opvalue, stackObjects[stackIndex--]);
                                        continue;
                                    }
                                    case Opcode.StoreGlobalString: {
                                        m_global.SetValue(constString[opvalue], stackObjects[stackIndex--]);
                                        instruction.SetOpcode(Opcode.StoreGlobal, m_global.GetIndex(constString[opvalue]));
                                        continue;
                                    }
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                            case OpcodeType.Compute:
                                switch (opcode) {
                                    case Opcode.Plus: {
                                        tempIndex = stackIndex - 1;
                                        tempValueType = stackObjects[tempIndex].valueType;
                                        switch (tempValueType) {
                                            case ScriptValue.stringValueType: {
                                                stackObjects[tempIndex].stringValue += stackObjects[stackIndex].ToString();
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Plus(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                                    stackObjects[tempIndex].stringValue = stackObjects[tempIndex].ToString() + stackObjects[stackIndex].stringValue;
                                                    stackObjects[tempIndex].valueType = ScriptValue.stringValueType;
                                                } else {
                                                    if (tempValueType == ScriptValue.doubleValueType) {
                                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                            stackObjects[tempIndex].doubleValue += stackObjects[stackIndex].doubleValue;
                                                        } else {
                                                            stackObjects[tempIndex].doubleValue += stackObjects[stackIndex].ToDouble();
                                                        }
                                                    } else if (tempValueType == ScriptValue.longValueType) {
                                                        switch (stackObjects[stackIndex].valueType) {
                                                            case ScriptValue.longValueType:
                                                                stackObjects[tempIndex].longValue += stackObjects[stackIndex].longValue;
                                                                break;
                                                            case ScriptValue.doubleValueType:
                                                                stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue + stackObjects[stackIndex].doubleValue;
                                                                stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                                break;
                                                            default:
                                                                stackObjects[tempIndex].longValue += stackObjects[stackIndex].ToLong();
                                                                break;
                                                        }
                                                    } else {
                                                        throw new ExecutionException($"【+】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                                    }
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                        }
                                    }
                                    case Opcode.Minus: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].doubleValue -= stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].doubleValue -= stackObjects[stackIndex].ToDouble();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Minus(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].longValue -= stackObjects[stackIndex].longValue;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue - stackObjects[stackIndex].doubleValue;
                                                        stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].longValue += stackObjects[stackIndex].ToLong();
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【-】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Multiply: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].doubleValue *= stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].doubleValue *= stackObjects[stackIndex].ToDouble();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Multiply(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].longValue *= stackObjects[stackIndex].longValue;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue * stackObjects[stackIndex].doubleValue;
                                                        stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].longValue *= stackObjects[stackIndex].ToLong();
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【*】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Divide: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].doubleValue /= stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].doubleValue /= stackObjects[stackIndex].ToDouble();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Divide(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].longValue /= stackObjects[stackIndex].longValue;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue / stackObjects[stackIndex].doubleValue;
                                                        stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].longValue /= stackObjects[stackIndex].ToLong();
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【/】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Modulo: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].doubleValue %= stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].doubleValue %= stackObjects[stackIndex].ToDouble();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Modulo(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].longValue %= stackObjects[stackIndex].longValue;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue % stackObjects[stackIndex].doubleValue;
                                                        stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].longValue %= stackObjects[stackIndex].ToLong();
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【%】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.InclusiveOr: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.longValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.longValueType) {
                                                    stackObjects[tempIndex].longValue |= stackObjects[stackIndex].longValue;
                                                } else {
                                                    stackObjects[tempIndex].longValue |= stackObjects[stackIndex].ToLong();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.trueValueType: {
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.falseValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.nullValueType:
                                                    case ScriptValue.falseValueType:
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = ScriptValue.trueValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.InclusiveOr(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【|】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Combine: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.longValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.longValueType) {
                                                    stackObjects[tempIndex].longValue &= stackObjects[stackIndex].longValue;
                                                } else {
                                                    stackObjects[tempIndex].longValue &= stackObjects[stackIndex].ToLong();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.falseValueType: {
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.trueValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.nullValueType:
                                                    case ScriptValue.falseValueType:
                                                        stackObjects[tempIndex].valueType = ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Combine(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【&】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.XOR: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.longValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.longValueType) {
                                                    stackObjects[tempIndex].longValue ^= stackObjects[stackIndex].longValue;
                                                } else {
                                                    stackObjects[tempIndex].longValue ^= stackObjects[stackIndex].ToLong();
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.XOR(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【^】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Shi: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.longValueType: {
                                                stackObjects[tempIndex].longValue <<= stackObjects[stackIndex].ToInt32();
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Shi(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【<<】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Shr: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.longValueType: {
                                                stackObjects[tempIndex].longValue >>= stackObjects[stackIndex].ToInt32();
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex] = stackObjects[tempIndex].scriptValue.Shr(stackObjects[stackIndex]);
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【>>】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.FlagNot: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.trueValueType:
                                                stackObjects[stackIndex].valueType = ScriptValue.falseValueType;
                                                continue;
                                            case ScriptValue.falseValueType:
                                            case ScriptValue.nullValueType:
                                                stackObjects[stackIndex].valueType = ScriptValue.trueValueType;
                                                continue;
                                            default:
                                                throw new ExecutionException($"当前数据类型不支持取反操作 : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.FlagMinus: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[stackIndex].doubleValue = -stackObjects[stackIndex].doubleValue;
                                                continue;
                                            case ScriptValue.longValueType:
                                                stackObjects[stackIndex].longValue = -stackObjects[stackIndex].longValue;
                                                continue;
                                            default:
                                                throw new ExecutionException($"当前数据类型不支持取负操作 : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.FlagNegative: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.longValueType) {
                                            stackObjects[stackIndex].longValue = ~stackObjects[stackIndex].longValue;
                                            continue;
                                        } else {
                                            throw new ExecutionException($"当前数据类型不支持取非操作 : {stackObjects[stackIndex].ValueTypeName}");
                                        }
                                    }
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                            case OpcodeType.Compare:
                                switch (opcode) {
                                    case Opcode.Less: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue < stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue < stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.Less(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【<】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.LessOrEqual: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue <= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue <= stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.LessOrEqual(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【<=】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Greater: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue > stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue > stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.Greater(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【>】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.GreaterOrEqual: {
                                        tempIndex = stackIndex - 1;
                                        switch (stackObjects[tempIndex].valueType) {
                                            case ScriptValue.doubleValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue >= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue >= stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.GreaterOrEqual(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【>=】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.Equal: {
                                        tempIndex = stackIndex - 1;
                                        tempValueType = stackObjects[tempIndex].valueType;
                                        switch (tempValueType) {
                                            case ScriptValue.doubleValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.stringValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].stringValue == stackObjects[stackIndex].stringValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = ScriptValue.falseValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.trueValueType:
                                            case ScriptValue.nullValueType:
                                            case ScriptValue.falseValueType: {
                                                stackObjects[tempIndex].valueType = tempValueType == stackObjects[stackIndex].valueType ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue == stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue == stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = ScriptValue.falseValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.Equals(stackObjects[stackIndex]) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.objectValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].objectValue.Equals(stackObjects[stackIndex].Value) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【==】未知的数据类型 {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    case Opcode.NotEqual: {
                                        tempIndex = stackIndex - 1;
                                        tempValueType = stackObjects[tempIndex].valueType;
                                        switch (tempValueType) {
                                            case ScriptValue.doubleValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].doubleValue ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                        break;
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].longValue ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = ScriptValue.trueValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.stringValueType: {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                                    stackObjects[tempIndex].valueType = stackObjects[tempIndex].stringValue == stackObjects[stackIndex].stringValue ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                } else {
                                                    stackObjects[tempIndex].valueType = ScriptValue.trueValueType;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.trueValueType:
                                            case ScriptValue.nullValueType:
                                            case ScriptValue.falseValueType: {
                                                stackObjects[tempIndex].valueType = tempValueType == stackObjects[stackIndex].valueType ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.longValueType: {
                                                switch (stackObjects[stackIndex].valueType) {
                                                    case ScriptValue.longValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue == stackObjects[stackIndex].longValue ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                        break;
                                                    case ScriptValue.doubleValueType:
                                                        stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue == stackObjects[stackIndex].doubleValue ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                        break;
                                                    default:
                                                        stackObjects[tempIndex].valueType = ScriptValue.trueValueType;
                                                        break;
                                                }
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.scriptValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].scriptValue.Equals(stackObjects[stackIndex]) ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            case ScriptValue.objectValueType: {
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].objectValue.Equals(stackObjects[stackIndex].Value) ? ScriptValue.falseValueType : ScriptValue.trueValueType;
                                                --stackIndex;
                                                continue;
                                            }
                                            default:
                                                throw new ExecutionException($"【!=】未知的数据类型 {stackObjects[tempIndex].ValueTypeName}");
                                        }
                                    }
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                            case OpcodeType.Jump:
                                switch (opcode) {
                                    case Opcode.Jump: {
                                        iInstruction = opvalue;
                                        continue;
                                    }
                                    case Opcode.Pop: {
                                        --stackIndex;
                                        continue;
                                    }
                                    case Opcode.PopNumber: {
                                        stackIndex -= opvalue;
                                        continue;
                                    }
                                    case Opcode.Call: {
                                        for (var i = opvalue - 1; i >= 0; --i) {
                                            parameters[i] = stackObjects[stackIndex--];
                                        }
                                        var func = stackObjects[stackIndex--];
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(ScriptValue.Null, parameters, opvalue);
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (ScriptValue.Null, parameters, opvalue);
#endif
                                        continue;
                                    }
                                    case Opcode.CallVi: {
                                        for (var i = opvalue - 1; i >= 0; --i) {
                                            parameters[i] = stackObjects[stackIndex--];
                                        }
                                        var func = stackObjects[stackIndex--];
                                        var parent = stackObjects[stackIndex--];
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(parent, parameters, opvalue);
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (parent, parameters, opvalue);
#endif
                                        continue;
                                    }
                                    case Opcode.CallEmpty: {
                                        var func = stackObjects[stackIndex--];
                                        var parent = stackObjects[stackIndex--];
                                        stackObjects[++stackIndex] = func.Call(parent);
                                        continue;
                                    }
                                    case Opcode.TrueTo: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.falseValueType:
                                            case ScriptValue.nullValueType: {
                                                --stackIndex;
                                                continue;
                                            }
                                            default: {
                                                iInstruction = opvalue;
                                                --stackIndex;
                                                continue;
                                            }
                                        }
                                    }
                                    case Opcode.FalseTo: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.falseValueType:
                                            case ScriptValue.nullValueType: {
                                                iInstruction = opvalue;
                                                --stackIndex;
                                                continue;
                                            }
                                            default: {
                                                --stackIndex;
                                                continue;
                                            }
                                        }
                                    }
                                    case Opcode.TrueLoadTrue: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.trueValueType:
                                                iInstruction = opvalue;
                                                continue;
                                            case ScriptValue.falseValueType:
                                            case ScriptValue.nullValueType:
                                                --stackIndex;
                                                continue;
                                            default:
                                                stackObjects[stackIndex].valueType = ScriptValue.trueValueType;
                                                iInstruction = opvalue;
                                                continue;
                                        }
                                    }
                                    case Opcode.FalseLoadFalse: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.falseValueType:
                                                iInstruction = opvalue;
                                                continue;
                                            case ScriptValue.nullValueType:
                                                stackObjects[stackIndex].valueType = ScriptValue.falseValueType;
                                                iInstruction = opvalue;
                                                continue;
                                            default:
                                                --stackIndex;
                                                continue;
                                        }
                                    }
                                    case Opcode.RetNone: {
#if EXECUTE_COROUTINE
                                        yield break;
#else
                                        --VariableValueIndex;
                                        return ScriptValue.Null;
#endif
                                    }
                                    case Opcode.Ret: {
#if EXECUTE_COROUTINE
                                        yield break;
#else
                                        --VariableValueIndex;
                                        return stackObjects[stackIndex];
#endif
                                    }
                                    case Opcode.CallUnfold: {
                                        var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                        var unfold = value & 0xff; //折叠标志位
                                        var funcParameterCount = (int)(value >> 8); //参数个数
                                        var startIndex = stackIndex - funcParameterCount + 1;
                                        var parameterIndex = 0;
                                        for (var i = 0; i < funcParameterCount; ++i) {
                                            var parameter = stackObjects[startIndex + i];
                                            if ((unfold & (1L << i)) != 0) {
                                                var array = parameter.Get<ScriptArray>();
                                                if (array != null) {
                                                    var values = array.getObjects();
                                                    var valueLength = array.Length();
                                                    for (var j = 0; j < valueLength; ++j) {
                                                        parameters[parameterIndex++] = values[j];
                                                    }
                                                } else {
                                                    parameters[parameterIndex++] = parameter;
                                                }
                                            } else {
                                                parameters[parameterIndex++] = parameter;
                                            }
                                        }
                                        stackIndex -= funcParameterCount;
                                        var func = stackObjects[stackIndex--]; //函数对象
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(ScriptValue.Null, parameters, parameterIndex);
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (ScriptValue.Null, parameters, parameterIndex);
#endif
                                        continue;
                                    }
                                    case Opcode.CallViUnfold: {
                                        var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                        var unfold = value & 0xff; //折叠标志位
                                        var funcParameterCount = (int)(value >> 8); //参数个数
                                        var startIndex = stackIndex - funcParameterCount + 1;
                                        var parameterIndex = 0;
                                        for (var i = 0; i < funcParameterCount; ++i) {
                                            var parameter = stackObjects[startIndex + i];
                                            if ((unfold & (1L << i)) != 0) {
                                                var array = parameter.Get<ScriptArray>();
                                                if (array != null) {
                                                    var values = array.getObjects();
                                                    var valueLength = array.Length();
                                                    for (var j = 0; j < valueLength; ++j) {
                                                        parameters[parameterIndex++] = values[j];
                                                    }
                                                } else {
                                                    parameters[parameterIndex++] = parameter;
                                                }
                                            } else {
                                                parameters[parameterIndex++] = parameter;
                                            }
                                        }
                                        stackIndex -= funcParameterCount;
                                        var func = stackObjects[stackIndex--]; //函数对象
                                        var parent = stackObjects[stackIndex--]; //函数父级
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(parent, parameters, parameterIndex);
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (parent, parameters, parameterIndex);
#endif
                                        continue;
                                    }
                                    case Opcode.NotNullTo: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.nullValueType) {
                                            --stackIndex;
                                        } else {
                                            iInstruction = opvalue;
                                        }
                                        continue;
                                    }
                                    case Opcode.NullTo: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.nullValueType) {
                                            iInstruction = opvalue;
                                        }
                                        continue;
                                    }
#if !EXECUTE_COROUTINE
                                    case Opcode.TryTo: {
                                        tryStack[++tryIndex] = opvalue;
                                        continue;
                                    }
                                    case Opcode.TryEnd: {
                                        iInstruction = opvalue;
                                        --tryIndex;
                                        continue;
                                    }
#endif
                                    case Opcode.Throw: {
                                        throw new ScriptException(stackObjects[stackIndex]);
                                    }
                                    case Opcode.CallBase: {
                                        for (var i = opvalue - 1; i >= 0; --i) {
                                            parameters[i] = stackObjects[stackIndex--];
                                        }
                                        var func = stackObjects[stackIndex--].Get<ScriptScriptFunction>(); //函数对象
                                        var prototype = stackObjects[stackIndex--];
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(thisObject, parameters, opvalue, prototype.Get<ScriptType>());
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (thisObject, parameters, opvalue, prototype.Get<ScriptType>());
#endif
                                        continue;
                                    }
                                    case Opcode.CallBaseUnfold: {
                                        var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                        var unfold = value & 0xff; //折叠标志位
                                        var funcParameterCount = (int)(value >> 8); //参数个数
                                        var startIndex = stackIndex - funcParameterCount + 1;
                                        var parameterIndex = 0;
                                        for (var i = 0; i < funcParameterCount; ++i) {
                                            var parameter = stackObjects[startIndex + i];
                                            if ((unfold & (1L << i)) != 0) {
                                                var array = parameter.Get<ScriptArray>();
                                                if (array != null) {
                                                    var values = array.getObjects();
                                                    var valueLength = array.Length();
                                                    for (var j = 0; j < valueLength; ++j) {
                                                        parameters[parameterIndex++] = values[j];
                                                    }
                                                } else {
                                                    parameters[parameterIndex++] = parameter;
                                                }
                                            } else {
                                                parameters[parameterIndex++] = parameter;
                                            }
                                        }
                                        stackIndex -= funcParameterCount;
                                        var func = stackObjects[stackIndex--].Get<ScriptScriptFunction>(); //函数对象
                                        var prototype = stackObjects[stackIndex--];
#if SCORPIO_DEBUG || SCORPIO_STACK
                                        m_script.PushStackInfo(m_Breviary, instruction.line);
                                        try {
                                            stackObjects[++stackIndex] = func.Call(thisObject, parameters, parameterIndex, prototype.Get<ScriptType>());
                                        } finally {
                                            m_script.PopStackInfo();
                                        }
#else
                                        stackObjects[++stackIndex] = func.Call (thisObject, parameters, parameterIndex, prototype.Get<ScriptType>());
#endif
                                        continue;
                                    }
#if EXECUTE_COROUTINE
                                    case Opcode.Await: {
                                        yield return stackObjects[stackIndex--].Value;
                                        continue;
                                    }
#endif
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                            case OpcodeType.New:
                                switch (opcode) {
                                    case Opcode.NewMap: {
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        if (opvalue == 0) {
                                            stackObjects[stackIndex].scriptValue = new ScriptMapObject(m_script);
                                        } else {
                                            stackObjects[stackIndex].scriptValue = new ScriptMapString(m_script);
                                        }
                                        continue;
                                    }
                                    case Opcode.NewMapString: {
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = new ScriptMapString(m_script);
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
                                    case Opcode.NewMapObject: {
                                        var map = new ScriptMapObject(m_script);
                                        for (var i = opvalue - 1; i >= 0; --i) {
                                            map.SetValue(stackObjects[stackIndex - i].Value, stackObjects[stackIndex - i - opvalue]);
                                        }
                                        stackIndex -= opvalue * 2;
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = map;
                                        continue;
                                    }
                                    case Opcode.NewFunction: {
                                        var functionData = constContexts[opvalue];
                                        var internals = functionData.m_FunctionData.internals;
                                        var function = new ScriptScriptFunction(functionData);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = function;
                                        continue;
                                    }
                                    case Opcode.NewLambdaFunction: {
                                        var functionData = constContexts[opvalue];
                                        var internals = functionData.m_FunctionData.internals;
                                        var function = new ScriptScriptBindFunction(functionData, thisObject);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = function;
                                        continue;
                                    }
                                    case Opcode.NewType: {
                                        var classData = constClasses[opvalue];
                                        var parentType = classData.parent >= 0 ? m_global.GetValue(constString[classData.parent]) : m_script.TypeObjectValue;
                                        var className = constString[classData.name];
                                        var type = new ScriptType(className, parentType.valueType == ScriptValue.nullValueType ? m_script.TypeObjectValue : parentType);
                                        var functions = classData.functions;
                                        for (var j = 0; j < functions.Length; ++j) {
                                            var func = functions[j];
                                            var functionData = constContexts[func & 0xffffffff];
                                            var internals = functionData.m_FunctionData.internals;
                                            var function = new ScriptScriptFunction(functionData);
                                            for (var i = 0; i < internals.Length; ++i) {
                                                var internalIndex = internals[i];
                                                function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                            }
                                            type.SetValue(constString[func >> 32], new ScriptValue(function));
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = type;
                                        continue;
                                    }
                                    case Opcode.NewAsyncFunction: {
                                        var functionData = constContexts[opvalue];
                                        var internals = functionData.m_FunctionData.internals;
                                        var function = new ScriptScriptAsyncFunction(functionData);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = function;
                                        continue;
                                    }
                                    case Opcode.NewAsyncLambdaFunction: {
                                        var functionData = constContexts[opvalue];
                                        var internals = functionData.m_FunctionData.internals;
                                        var function = new ScriptScriptAsyncBindFunction(functionData, thisObject);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = function;
                                        continue;
                                    }
                                    case Opcode.NewAsyncType: {
                                        var classData = constClasses[opvalue];
                                        var parentType = classData.parent >= 0 ? m_global.GetValue(constString[classData.parent]) : m_script.TypeObjectValue;
                                        var className = constString[classData.name];
                                        var type = new ScriptType(className, parentType.valueType == ScriptValue.nullValueType ? m_script.TypeObjectValue : parentType);
                                        var functions = classData.functions;
                                        for (var j = 0; j < functions.Length; ++j) {
                                            var func = functions[j];
                                            var functionData = constContexts[func & 0xffffffff >> 1];
                                            var async = (func & 1) == 1;
                                            var internals = functionData.m_FunctionData.internals;
                                            if ((func & 1) == 1) {
                                                var function = new ScriptScriptAsyncFunction(functionData);
                                                for (var i = 0; i < internals.Length; ++i) {
                                                    var internalIndex = internals[i];
                                                    function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                                }
                                                type.SetValue(constString[func >> 32], new ScriptValue(function));
                                            } else {
                                                var function = new ScriptScriptFunction(functionData);
                                                for (var i = 0; i < internals.Length; ++i) {
                                                    var internalIndex = internals[i];
                                                    function.SetInternal(internalIndex & 0xffff, internalObjects[internalIndex >> 16]);
                                                }
                                                type.SetValue(constString[func >> 32], new ScriptValue(function));
                                            }
                                        }
                                        stackObjects[++stackIndex].valueType = ScriptValue.scriptValueType;
                                        stackObjects[stackIndex].scriptValue = type;
                                        continue;
                                    }
                                    default: throw new ExecutionException("unknown opcode : " + opcode);
                                }
                        }
                    }
#if !EXECUTE_COROUTINE
                    --VariableValueIndex;
#endif
                    //正常执行命令到最后,判断堆栈是否清空 return 或 exception 不判断
                    Logger.debug(stackIndex != -1, "堆栈数据未清空，有泄露情况 : " + stackIndex);
#if !EXECUTE_COROUTINE
                    //主动throw的情况
                } catch (ScriptException e) {
                    if (tryIndex > -1) {
                        stackObjects[stackIndex = 0] = e.value;
                        iInstruction = tryStack[tryIndex--];
                        goto KeepOn;
                    } else {
                        e.message = $"{m_Breviary}:{instruction.line}({iInstruction})\n  {e.message}";
                        throw;
                    }
                //脚本系统错误
                } catch (ExecutionException e) {
                    e.message = $"{m_Breviary}:{instruction.line}({iInstruction})\n  {e.message}";
                    if (tryIndex > -1) {
                        stackObjects[stackIndex = 0] = ScriptValue.CreateValue(e);
                        iInstruction = tryStack[tryIndex--];
                        goto KeepOn;
                    } else {
                        throw;
                    }
                //其他错误
                } catch (System.Exception e) {
                    if (tryIndex > -1) {
                        stackObjects[stackIndex = 0] = ScriptValue.CreateValue(e);
                        iInstruction = tryStack[tryIndex--];
                        goto KeepOn;
                    } else {
                        throw new ExecutionException($"{m_Breviary}:{instruction.line}({iInstruction})", e);
                    }
                }
            } catch(System.Exception) {
                --VariableValueIndex;
                throw;
            }
            return ScriptValue.Null;
#else
            } finally {
                --VariableValueIndex;
            }
#endif
        }
    }
}