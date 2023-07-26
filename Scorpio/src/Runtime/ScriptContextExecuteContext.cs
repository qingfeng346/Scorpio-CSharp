#define EXECUTE_CONTEXT
using System;
using System.Collections;
using Scorpio.Exception;
using Scorpio.Instruction;
using Scorpio.Tools;
namespace Scorpio.Runtime {
    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public partial class ScriptContext {
#if EXECUTE_COROUTINE && EXECUTE_BASE
        public IEnumerator ExecuteCoroutine(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] parentInternalValues, ScriptType baseType) {
#elif EXECUTE_COROUTINE
        public IEnumerator ExecuteCoroutine(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] parentInternalValues) {
#elif EXECUTE_BASE
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] parentInternalValues, ScriptType baseType) {
#elif EXECUTE_CONTEXT
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] parentInternalValues, string[] constString) {
#else
        public ScriptValue Execute(ScriptValue thisObject, ScriptValue[] args, int length, InternalValue[] parentInternalValues) {
#endif
            var script = m_script;
            var constDouble = this.constDouble;
            var constLong = this.constLong;
#if !EXECUTE_CONTEXT
            var constString = script.ConstString;
#endif
            var constScriptString = script.ConstString;
            #region 堆栈和线程判断
#if SCORPIO_ASSERT
            //System.Console.WriteLine($"执行命令 =>\n{m_FunctionData.ToString(constDouble, constLong, constString)}");
            if (VariableValueIndex < 0 || VariableValueIndex >= ValueCacheLength) {
                throw new ExecutionException($"Stack overflow : {VariableValueIndex}");
            }
#endif
#if SCORPIO_THREAD
            var currentThread = System.Threading.Thread.CurrentThread;
            if (currentThread.ManagedThreadId != m_script.MainThreadId) {
                throw new ExecutionException($"only run script on mainthread : {m_script.MainThreadId} - {currentThread.ManagedThreadId}({currentThread.Name})");
            }
#endif
            #endregion
            #region 申请堆栈和局部变量
#if EXECUTE_COROUTINE
            var asyncValue = AsyncValuePoolLength > 0 ? AsyncValuePool[--AsyncValuePoolLength] : new AsyncValue();
            var variableObjects = asyncValue.variable;      //局部变量
            var stackObjects = asyncValue.stack;            //堆栈数据
#else
            var variableObjects = VariableValues[VariableValueIndex];   //局部变量
            var stackObjects = StackValues[VariableValueIndex];         //堆栈数据
            var tryStack = TryStackValues[VariableValueIndex++];        //try catch
            var tryIndex = -1; //try索引
            if (VariableValueIndex > MaxVariableValueIndex) {
                MaxVariableValueIndex = VariableValueIndex;
            }
#endif
            #endregion
            #region 初始化内部变量
            var internalCount = m_FunctionData.internalCount;
            InternalValue[] internalValues = null;
            if (internalCount > 0) {
                internalValues = m_script.NewIntervalValues();          //内部变量，有外部引用
                if (parentInternalValues == null) {
                    for (int i = 0; i < internalCount; ++i) {
                        internalValues[i] = m_script.NewIntervalValue();
                    }
                } else {
                    for (int i = 0; i < internalCount; ++i) {
                        if (parentInternalValues[i] == null) {
                            internalValues[i] = m_script.NewIntervalValue();
                        } else {
                            internalValues[i] = parentInternalValues[i].Reference();
                        }
                    }
                }
            }
            #endregion
            #region 初始化参数和this
            variableObjects[0].CopyFrom(thisObject, script);
            var stackIndex = -1; //堆栈索引
            var parameterCount = m_FunctionData.parameterCount; //参数数量
            //是否是变长参数
            if (m_FunctionData.param) {
                var array = m_script.NewArray();
                for (var i = parameterCount - 1; i < length; ++i) {
                    array.Add(args[i]);
                }
                stackObjects[++stackIndex].SetScriptValue(array, script);
                for (var i = parameterCount - 2; i >= 0; --i) {
                    stackObjects[++stackIndex].CopyFrom(i >= length ? ScriptValue.Null : args[i], script);
                }
            } else {
                for (var i = parameterCount - 1; i >= 0; --i) {
                    stackObjects[++stackIndex].CopyFrom(i >= length ? ScriptValue.Null : args[i], script);
                }
            }
            #endregion
            var parameters = ScorpioUtil.Parameters; //传递参数
            var iInstruction = 0; //当前执行命令索引
            var m_scriptInstructions = m_FunctionData.scriptInstructions;
            var iInstructionCount = m_scriptInstructions.Length; //指令数量
            var m_global = m_script.m_global;
            byte tempValueType; //临时存储
            int tempIndex; //临时存储
            ScriptInstruction instruction = null;
            Opcode opcode = Opcode.Nop;
            int opvalue = 0;
            try {
#if EXECUTE_COROUTINE
                //进函数先调用一次 MoveNext,否则 finally 无法正常调用,会导致泄漏
                yield return null;
#else
            KeepOn:
                try {
#endif
                while (iInstruction < iInstructionCount) {
                        instruction = m_scriptInstructions[iInstruction++];
                        opvalue = instruction.opvalue;
                        opcode = instruction.opcode;
#if SCORPIO_DEBUG
                        m_script.RecordStack.Breviary = m_Breviary;
                        m_script.RecordStack.Line = instruction.line;
#endif
                        switch (opcode) {
                            #region Load
                            case Opcode.LoadConstDouble: {
                                stackObjects[++stackIndex].SetDoubleValue(constDouble[opvalue], script);
                                continue;
                            }
                            case Opcode.LoadConstNull: {
                                stackObjects[++stackIndex].SetNull(script);
                                continue;
                            }
                            case Opcode.LoadConstTrue: {
                                stackObjects[++stackIndex].SetTrue(script);
                                continue;
                            }
                            case Opcode.LoadConstFalse: {
                                stackObjects[++stackIndex].SetFalse(script);
                                continue;
                            }
                            case Opcode.LoadConstLong: {
                                stackObjects[++stackIndex].SetLongValue(constLong[opvalue], script);
                                continue;
                            }
                            case Opcode.LoadConstString: {
                                stackObjects[++stackIndex].SetStringValue(constString[opvalue], script);
                                continue;
                            }
                            case Opcode.LoadLocal: {
                                stackObjects[++stackIndex].CopyFrom(variableObjects[opvalue], script);
                                continue;
                            }
                            case Opcode.LoadInternal: {
                                stackObjects[++stackIndex].CopyFrom(internalValues[opvalue].value, script);
                                continue;
                            }
                            case Opcode.LoadValue: {
                                stackObjects[stackIndex].CopyFrom(stackObjects[stackIndex].GetValueByIndex(opvalue, m_script), script);
                                continue;
                            }
                            case Opcode.LoadValueString: {
                                stackObjects[stackIndex].CopyFrom(stackObjects[stackIndex].GetValueByString(constString[opvalue], m_script), script);
                                continue;
                            }
                            case Opcode.LoadValueObject: {
                                stackObjects[stackIndex - 1].CopyFrom(stackObjects[stackIndex - 1].GetValueByScriptValue(stackObjects[stackIndex], m_script), script);
                                --stackIndex;
                                continue;
                            }
                            case Opcode.LoadValueObjectDup: {
                                stackObjects[stackIndex + 1].CopyFrom(stackObjects[stackIndex - 1].GetValueByScriptValue(stackObjects[stackIndex], m_script), script);
                                ++stackIndex;
                                continue;
                            }
                            case Opcode.LoadGlobal: {
                                stackObjects[++stackIndex].CopyFrom(m_global.GetValueByIndex(opvalue), script);
                                continue;
                            }
                            case Opcode.LoadGlobalString: {
                                stackObjects[++stackIndex].CopyFrom(m_global.GetValue(constString[opvalue]), script);
                                instruction.SetOpcode(Opcode.LoadGlobal, m_global.GetIndex(constString[opvalue]));
                                continue;
                            }
                            case Opcode.CopyStackTop: {
                                stackObjects[++stackIndex].CopyFrom(stackObjects[stackIndex - 1], script);
                                continue;
                            }
                            case Opcode.CopyStackTopIndex: {
                                stackObjects[++stackIndex].CopyFrom(stackObjects[stackIndex - opvalue - 1], script);
                                continue;
                            }
                            case Opcode.LoadBase: {
#if EXECUTE_BASE
                                stackObjects[++stackIndex].SetScriptValue(baseType.Prototype, script);
#else
                                stackObjects[++stackIndex].SetScriptValue(thisObject.Get<ScriptInstance>(script).Prototype.Prototype, script);
#endif
                                continue;
                            }
                            case Opcode.ToGlobal: {
                                tempIndex = m_global.GetIndex(stackObjects[stackIndex--].GetStringValue(script));
                                m_global.SetValueByIndex(tempIndex, stackObjects[stackIndex]);
                                instruction.SetOpcode(Opcode.LoadGlobal, tempIndex);
                                for (var i = 0; i < opvalue; ++i) {
                                    m_scriptInstructions[iInstruction - i - 2].SetOpcode(Opcode.Nop);
                                }
                                continue;
                            }
                            case Opcode.ToGlobalFunction: {
                                tempIndex = m_global.GetIndex(stackObjects[stackIndex--].GetStringValue(script));
                                m_global.SetValueByIndex(tempIndex, stackObjects[stackIndex]);
                                instruction.SetOpcode(Opcode.LoadGlobal, tempIndex);
                                for (var i = 0; i < opvalue; ++i) {
                                    if (i == 0) {
                                        m_scriptInstructions[iInstruction - i - 2].SetOpcode(Opcode.LoadConstNull);
                                    } else {
                                        m_scriptInstructions[iInstruction - i - 2].SetOpcode(Opcode.Nop);
                                    }
                                }
                                continue;
                            }
                            #endregion
                            #region StoreAssign
                            //-------------下面为 = *= -= 等赋值操作, 压入计算结果
                            case Opcode.StoreLocalAssign: {
                                variableObjects[opvalue].CopyFrom(stackObjects[stackIndex], script);
                                continue;
                            }
                            case Opcode.StoreInternalAssign: {
                                internalValues[opvalue].value.CopyFrom(stackObjects[stackIndex], script);
                                continue;
                            }
                            case Opcode.StoreValueStringAssign: {
                                tempIndex = stackIndex;
                                stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex], script);
                                stackObjects[--stackIndex].CopyFrom(stackObjects[tempIndex], script);
                                continue;
                            }
                            case Opcode.StoreValueObjectAssign: {
                                tempIndex = stackIndex;
                                stackObjects[stackIndex - 2].SetValueByScriptValue(stackObjects[stackIndex - 1], stackObjects[stackIndex], script);
                                stackObjects[stackIndex -= 2].CopyFrom(stackObjects[tempIndex], script);
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
                                stackObjects[stackIndex - 1].SetValueByIndex(opvalue, stackObjects[stackIndex], script);
                                stackObjects[--stackIndex].CopyFrom(stackObjects[tempIndex], script);
                                continue;
                            }
                            #endregion
                            #region Store
                            //-----------------下面为普通赋值操作 不压入结果
                            case Opcode.StoreLocal: {
                                variableObjects[opvalue].CopyFrom(stackObjects[stackIndex--], script);
                                continue;
                            }
                            case Opcode.StoreInternal: {
                                internalValues[opvalue].value.CopyFrom(stackObjects[stackIndex--], script);
                                continue;
                            }
                            case Opcode.StoreValueString: {
                                stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex], script);
                                stackIndex -= 2;
                                continue;
                            }
                            case Opcode.StoreValueObject: {
                                stackObjects[stackIndex - 2].SetValueByScriptValue(stackObjects[stackIndex - 1], stackObjects[stackIndex], script);
                                stackIndex -= 3;
                                continue;
                            }
                            case Opcode.StoreGlobal: {
                                m_global.SetValueByIndex(opvalue, stackObjects[stackIndex--]);
                                continue;
                            }
                            case Opcode.StoreGlobalString: {
                                var globalValue = stackObjects[stackIndex--];
                                m_global.SetValue(constString[opvalue], globalValue);
                                instruction.SetOpcode(Opcode.StoreGlobal, m_global.GetIndex(constString[opvalue]));
                                continue;
                            }
                            #endregion
                            #region Compute
                            case Opcode.Plus: {
                                tempIndex = stackIndex - 1;
                                tempValueType = stackObjects[tempIndex].valueType;
                                switch (tempValueType) {
                                    case ScriptValue.stringValueType: {
                                        stackObjects[tempIndex].SetStringValue(stackObjects[tempIndex].GetStringValue(script) + stackObjects[stackIndex].ToString(script), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Plus(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].SetStringValue(stackObjects[tempIndex].ToString(script) + stackObjects[stackIndex].GetStringValue(script), script);
                                        } else {
                                            if (tempValueType == ScriptValue.doubleValueType) {
                                                stackObjects[tempIndex].doubleValue += stackObjects[stackIndex].ToDouble(script);
                                            } else if (tempValueType == ScriptValue.int64ValueType) {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                    stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue + stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].longValue += stackObjects[stackIndex].ToLong(script);
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
                                        stackObjects[tempIndex].doubleValue -= stackObjects[stackIndex].ToDouble(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Minus(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue - stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue -= stackObjects[stackIndex].ToLong(script);
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
                                        stackObjects[tempIndex].doubleValue *= stackObjects[stackIndex].ToDouble(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Multiply(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue * stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue *= stackObjects[stackIndex].ToLong(script);
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
                                        stackObjects[tempIndex].doubleValue /= stackObjects[stackIndex].ToDouble(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Divide(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue / stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue /= stackObjects[stackIndex].ToLong(script);
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
                                        stackObjects[tempIndex].doubleValue %= stackObjects[stackIndex].ToDouble(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Modulo(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue % stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue %= stackObjects[stackIndex].ToLong(script);
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
                                    case ScriptValue.int64ValueType: {
                                        stackObjects[tempIndex].longValue |= stackObjects[stackIndex].ToLong(script);
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
                                                stackObjects[tempIndex].SetTrue(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).InclusiveOr(stackObjects[stackIndex]), script);
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
                                    case ScriptValue.int64ValueType: {
                                        stackObjects[tempIndex].longValue &= stackObjects[stackIndex].ToLong(script);
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
                                                stackObjects[tempIndex].SetFalse(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Combine(stackObjects[stackIndex]), script);
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
                                    case ScriptValue.int64ValueType: {
                                        stackObjects[tempIndex].longValue ^= stackObjects[stackIndex].ToLong(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).XOR(stackObjects[stackIndex]), script);
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
                                    case ScriptValue.int64ValueType: {
                                        stackObjects[tempIndex].longValue <<= stackObjects[stackIndex].ToInt32(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Shi(stackObjects[stackIndex]), script);
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
                                    case ScriptValue.int64ValueType: {
                                        stackObjects[tempIndex].longValue >>= stackObjects[stackIndex].ToInt32(script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].GetScriptValue(script).Shr(stackObjects[stackIndex]), script);
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
                                        stackObjects[stackIndex].SetFalse(script);
                                        continue;
                                    case ScriptValue.falseValueType:
                                    case ScriptValue.nullValueType:
                                        stackObjects[stackIndex].SetTrue(script);
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
                                    case ScriptValue.int64ValueType:
                                        stackObjects[stackIndex].longValue = -stackObjects[stackIndex].longValue;
                                        continue;
                                    default:
                                        throw new ExecutionException($"当前数据类型不支持取负操作 : {stackObjects[stackIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.FlagNegative: {
                                if (stackObjects[stackIndex].valueType == ScriptValue.int64ValueType) {
                                    stackObjects[stackIndex].longValue = ~stackObjects[stackIndex].longValue;
                                    continue;
                                } else {
                                    throw new ExecutionException($"当前数据类型不支持取非操作 : {stackObjects[stackIndex].ValueTypeName}");
                                }
                            }
                            #endregion
                            #region Compare
                            case Opcode.Equal: {
                                tempIndex = stackIndex - 1;
                                tempValueType = stackObjects[tempIndex].valueType;
                                //保证 tempValueType 已知的情况下
                                switch (tempValueType) {
                                    case ScriptValue.doubleValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.int64ValueType:
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
                                            stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].index == stackObjects[stackIndex].index, script);
                                        } else {
                                            stackObjects[tempIndex].SetFalse(script);
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
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
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
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).Equals(stackObjects[stackIndex]), script);
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
                                //保证 tempValueType 已知的情况下
                                switch (tempValueType) {
                                    case ScriptValue.doubleValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
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
                                            stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].index != stackObjects[stackIndex].index, script);
                                        } else {
                                            stackObjects[tempIndex].SetTrue(script);
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.trueValueType:
                                    case ScriptValue.nullValueType:
                                    case ScriptValue.falseValueType: {
                                        stackObjects[tempIndex].valueType = tempValueType != stackObjects[stackIndex].valueType ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue != stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue != stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:
                                                stackObjects[tempIndex].valueType = ScriptValue.trueValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(!stackObjects[tempIndex].GetScriptValue(script).Equals(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【!=】未知的数据类型 {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.Less: {
                                tempIndex = stackIndex - 1;
                                //保证 tempValueType 已知的情况下
                                switch (stackObjects[tempIndex].valueType) {
                                    case ScriptValue.doubleValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue < stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        } else {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue < stackObjects[stackIndex].ToDouble(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].ToLong(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).Less(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【<】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.LessOrEqual: {
                                tempIndex = stackIndex - 1;
                                //保证 tempValueType 已知的情况下
                                switch (stackObjects[tempIndex].valueType) {
                                    case ScriptValue.doubleValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue <= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        } else {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue <= stackObjects[stackIndex].ToDouble(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].ToLong(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).LessOrEqual(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【<=】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.Greater: {
                                tempIndex = stackIndex - 1;
                                //保证 tempValueType 已知的情况下
                                switch (stackObjects[tempIndex].valueType) {
                                    case ScriptValue.doubleValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue > stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        } else {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue > stackObjects[stackIndex].ToDouble(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].ToLong(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).Greater(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【>】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.GreaterOrEqual: {
                                tempIndex = stackIndex - 1;
                                //保证 tempValueType 已知的情况下
                                switch (stackObjects[tempIndex].valueType) {
                                    case ScriptValue.doubleValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue >= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        } else {
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue >= stackObjects[stackIndex].ToDouble(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].longValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].doubleValue ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                            default:
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].ToLong(script) ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).GreaterOrEqual(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【>=】运算符不支持当前类型 : {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.EqualReference: {
                                tempIndex = stackIndex - 1;
                                tempValueType = stackObjects[tempIndex].valueType;
                                switch (tempValueType) {
                                    case ScriptValue.doubleValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].doubleValue, script);
                                                break;
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].longValue, script);
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetFalse(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.stringValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].index == stackObjects[stackIndex].index, script);
                                        } else {
                                            stackObjects[tempIndex].SetFalse(script);
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.trueValueType:
                                    case ScriptValue.nullValueType:
                                    case ScriptValue.falseValueType: {
                                        stackObjects[tempIndex].SetBoolValue(tempValueType == stackObjects[stackIndex].valueType, script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].longValue == stackObjects[stackIndex].longValue, script);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].longValue == stackObjects[stackIndex].doubleValue, script);
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetFalse(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].GetScriptValue(script).EqualReference(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【==】未知的数据类型 {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            case Opcode.NotEqualReference: {
                                tempIndex = stackIndex - 1;
                                tempValueType = stackObjects[tempIndex].valueType;
                                switch (tempValueType) {
                                    case ScriptValue.doubleValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].doubleValue, script);
                                                break;
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].longValue, script);
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetTrue(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.stringValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].index != stackObjects[stackIndex].index, script);
                                        } else {
                                            stackObjects[tempIndex].SetTrue(script);
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.trueValueType:
                                    case ScriptValue.nullValueType:
                                    case ScriptValue.falseValueType: {
                                        stackObjects[tempIndex].SetBoolValue(tempValueType != stackObjects[stackIndex].valueType, script);
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].longValue != stackObjects[stackIndex].longValue, script);
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].SetBoolValue(stackObjects[tempIndex].longValue != stackObjects[stackIndex].doubleValue, script);
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetTrue(script);
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].SetBoolValue(!stackObjects[tempIndex].GetScriptValue(script).EqualReference(stackObjects[stackIndex]), script);
                                        --stackIndex;
                                        continue;
                                    }
                                    default:
                                        throw new ExecutionException($"【!=】未知的数据类型 {stackObjects[tempIndex].ValueTypeName}");
                                }
                            }
                            #endregion
                            #region Jump
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[stackIndex].Set(stackObjects[stackIndex].Call(script, ScriptValue.Null, parameters, opvalue), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallVi: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                var func = stackObjects[stackIndex--];
                                var parent = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(script, parent, parameters, opvalue), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallEmpty: {
                                var func = stackObjects[stackIndex--];
                                var parent = stackObjects[stackIndex--];
                                stackObjects[++stackIndex].Set(func.Call(script, parent), script);
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
                                        stackObjects[stackIndex].SetTrue(script);
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
                                        stackObjects[stackIndex].SetFalse(script);
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
                                if (internalValues != null) {
                                    for (var i = 0; i < internalCount; ++i) {
                                        if (internalValues[i] != null) {
                                            internalValues[i].Release();
                                            internalValues[i] = null;
                                        }
                                    }
                                    m_script.Free(internalValues);
                                }
                                return ScriptValue.Null;
#endif
                            }
                            case Opcode.Ret: {
#if EXECUTE_COROUTINE
                                m_script.CoroutineResult.CopyFrom(stackObjects[stackIndex], script);
                                yield break;
#else
                                var ret = stackObjects[stackIndex].Reference(script);
                                --VariableValueIndex;
                                if (internalValues != null) {
                                    for (var i = 0; i < internalCount; ++i) {
                                        if (internalValues[i] != null) {
                                            internalValues[i].Release();
                                            internalValues[i] = null;
                                        }
                                    }
                                    m_script.Free(internalValues);
                                }
                                return ret;
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
                                        var array = parameter.Get<ScriptArray>(script);
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(script, ScriptValue.Null, parameters, parameterIndex), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
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
                                        var array = parameter.Get<ScriptArray>(script);
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(script, parent, parameters, parameterIndex), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
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
                            case Opcode.Throw: {
                                throw new ScriptException(stackObjects[stackIndex]);
                            }
                            case Opcode.CallBase: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                var func = stackObjects[stackIndex--];              //函数对象
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(script, thisObject, parameters, opvalue, prototype.Get<ScriptType>(script)), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
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
                                        var array = parameter.Get<ScriptArray>(script);
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
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(script, thisObject, parameters, parameterIndex, prototype.Get<ScriptType>(script)), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
#if EXECUTE_COROUTINE
                            case Opcode.Await: {
                                yield return stackObjects[stackIndex--].GetObject(script);
                                continue;
                            }
                            case Opcode.NewAwait: {
                                yield return stackObjects[stackIndex--].GetObject(script);
                                stackObjects[++stackIndex].CopyFrom(m_script.CoroutineResult, script);
                                m_script.CoroutineResult.SetNull(script);
                                continue;
                            }
                            case Opcode.CallAsync: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[stackIndex].Set(stackObjects[stackIndex].CallAsync(script, ScriptValue.Null, parameters, opvalue), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallViAsync: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                var func = stackObjects[stackIndex--];
                                var parent = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(script, parent, parameters, opvalue), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallUnfoldAsync: {
                                var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                var unfold = value & 0xff; //折叠标志位
                                var funcParameterCount = (int)(value >> 8); //参数个数
                                var startIndex = stackIndex - funcParameterCount + 1;
                                var parameterIndex = 0;
                                for (var i = 0; i < funcParameterCount; ++i) {
                                    var parameter = stackObjects[startIndex + i];
                                    if ((unfold & (1L << i)) != 0) {
                                        var array = parameter.Get<ScriptArray>(script);
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(script, ScriptValue.Null, parameters, parameterIndex), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallViUnfoldAsync: {
                                var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                var unfold = value & 0xff; //折叠标志位
                                var funcParameterCount = (int)(value >> 8); //参数个数
                                var startIndex = stackIndex - funcParameterCount + 1;
                                var parameterIndex = 0;
                                for (var i = 0; i < funcParameterCount; ++i) {
                                    var parameter = stackObjects[startIndex + i];
                                    if ((unfold & (1L << i)) != 0) {
                                        var array = parameter.Get<ScriptArray>(script);
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(script, parent, parameters, parameterIndex), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallBaseAsync: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                var func = stackObjects[stackIndex--];              //函数对象
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(script, thisObject, parameters, opvalue, prototype.Get<ScriptType>(script)), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallBaseUnfoldAsync: {
                                var value = constLong[opvalue]; //值 前8位为 参数个数  后56位标识 哪个参数需要展开
                                var unfold = value & 0xff; //折叠标志位
                                var funcParameterCount = (int)(value >> 8); //参数个数
                                var startIndex = stackIndex - funcParameterCount + 1;
                                var parameterIndex = 0;
                                for (var i = 0; i < funcParameterCount; ++i) {
                                    var parameter = stackObjects[startIndex + i];
                                    if ((unfold & (1L << i)) != 0) {
                                        var array = parameter.Get<ScriptArray>(script);
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
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(script, thisObject, parameters, parameterIndex, prototype.Get<ScriptType>(script)), script);
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
#else
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
#endregion
#region New
                            case Opcode.NewMap: {
                                var map = m_script.NewMapObject();
                                stackObjects[++stackIndex].SetScriptValue(map, script);
                                continue;
                            }
                            case Opcode.NewArray: {
                                var array = m_script.NewArray();
                                array.SetArrayCapacity(opvalue);
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    array.Add(stackObjects[stackIndex - i]);
                                }
                                stackIndex -= opvalue;
                                stackObjects[++stackIndex].SetScriptValue(array, script);
                                continue;
                            }
                            case Opcode.NewFunction: {
                                var function = m_script.NewFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData);
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function, script);
                                continue;
                            }
                            case Opcode.NewLambdaFunction: {
                                var function = m_script.NewBindFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData, thisObject);
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function, script);
                                continue;
                            }
                            case Opcode.NewAsyncFunction: {
                                var function = m_script.NewAsyncFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData);
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function, script);
                                continue;
                            }
                            case Opcode.NewAsyncLambdaFunction: {
                                var function = m_script.NewAsyncBindFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData, thisObject);
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function, script);
                                continue;
                            }
                            case Opcode.NewType: {
                                var classData = constClasses[opvalue];
                                var parentType = classData.parent >= 0 ? m_global.GetValue(constScriptString[classData.parent]).Get<ScriptType>(script) : m_script.TypeObject;
                                var className = constScriptString[classData.name];
                                var type = m_script.NewType();
                                type.Set(className, parentType ?? m_script.TypeObject);
                                var functions = classData.functions;
                                var functionCount = 0;
                                var functionLength = functions.Length;
                                for (var j = 0 ; j < functionLength; ++j) {
                                    if ((functions[j] & 0xf) != 2) {
                                        functionCount++;
                                    }
                                }
                                type.SetFunctionCapacity(functionCount);
                                type.SetGetPropertyCapacity(functionLength - functionCount);
                                for (var j = 0; j < functionLength; ++j) {
                                    var func = functions[j];
                                    var functionData = constContexts[(func & 0xffffffff) >> 4];
                                    var internals = functionData.m_FunctionData.internals;
                                    var funcType = func & 0xf;
                                    var functionName = constScriptString[func >> 32];
                                    if (funcType == 0) {
                                        var function = m_script.NewFunction().SetContext(functionData);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.SetValue(functionName, function);
                                    } else if (funcType == 1) {
                                        var function = m_script.NewAsyncFunction().SetContext(functionData);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.SetValue(functionName, function);
                                    } else if (funcType == 2) {
                                        var function = m_script.NewFunction().SetContext(functionData);
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.AddGetProperty(functionName, function);
                                    }
                                }
                                stackObjects[++stackIndex].SetScriptValue(type, script);
                                continue;
                            }
                            #endregion
                            case Opcode.Nop:
                                continue;
                            default:
                                throw new ExecutionException($"未知的Opcode : {opcode}");
                        }
                    }
#if !EXECUTE_COROUTINE
                    --VariableValueIndex;
                    if (internalValues != null) {
                        for (var i = 0; i < internalCount; ++i) {
                            if (internalValues[i] != null) {
                                internalValues[i].Release();
                                internalValues[i] = null;
                            }
                        }
                        m_script.Free(internalValues);
                    }
#endif
                    //正常执行命令到最后,判断堆栈是否清空 return 或 exception 不判断
#if SCORPIO_ASSERT
                    if (stackIndex != -1) {
                        throw new ExecutionException($"堆栈数据未清空,有泄露情况,需要检查指令 : {stackIndex}");
                    }
#endif
#if !EXECUTE_COROUTINE
                    //主动throw的情况
                    } catch (ScriptException e) {
                        if (tryIndex > -1) {
                            stackObjects[stackIndex = 0].CopyFrom(e.value, script);
                            iInstruction = tryStack[tryIndex--];
                            goto KeepOn;
                        } else {
                            e.message = $"{m_Breviary}:{instruction.line}({opcode})\n  {e.message}";
                            throw;
                        }
                    //脚本系统错误
                    } catch (ExecutionException e) {
                        e.message = $"{m_Breviary}:{instruction.line}({opcode})\n  {e.message}";
                        if (tryIndex > -1) {
                            stackObjects[stackIndex = 0].Set(ScriptValue.CreateValue(script, e), script);
                            iInstruction = tryStack[tryIndex--];
                            goto KeepOn;
                        } else {
                            throw;
                        }
                        //其他错误
                    } catch (System.Exception e) {
                        if (tryIndex > -1) {
                            stackObjects[stackIndex = 0].Set(ScriptValue.CreateValue(script, e), script);
                            iInstruction = tryStack[tryIndex--];
                            goto KeepOn;
                        } else {
                            throw new ExecutionException($"{m_Breviary}:{instruction.line}({opcode}:{opvalue})", e);
                        }
                    }
                } catch (System.Exception) {
                    --VariableValueIndex;
                    if (internalValues != null) {
                        for (var i = 0; i < internalCount; ++i) {
                            if (internalValues[i] != null) {
                                internalValues[i].Release();
                                internalValues[i] = null;
                            }
                        }
                        m_script.Free(internalValues);
                    }
                    throw;
                }
                return ScriptValue.Null;
#else
            } finally {
                if (internalValues != null) {
                    for (var i = 0; i < internalCount; ++i) {
                        if (internalValues[i] != null) {
                            internalValues[i].Release();
                            internalValues[i] = null;
                        }
                    }
                    m_script.Free(internalValues);
                }
                if (AsyncValuePoolLength == AsyncValuePool.Length) {
                    var newPool = new AsyncValue[AsyncValuePoolLength + AsyncValueLength];
                    Array.Copy(AsyncValuePool, newPool, AsyncValuePoolLength);
                    AsyncValuePool = newPool;
                }
                AsyncValuePool[AsyncValuePoolLength++] = asyncValue;
            }
#endif
        }
    }
}
