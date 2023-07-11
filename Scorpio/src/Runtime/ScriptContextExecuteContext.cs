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
            var constDouble = this.constDouble;
            var constLong = this.constLong;
#if !EXECUTE_CONTEXT
            var constString = m_script.ConstString;
#endif
            var constScriptString = m_script.ConstString;
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
            variableObjects[0].CopyFrom(thisObject);
            var stackIndex = -1; //堆栈索引
            var parameterCount = m_FunctionData.parameterCount; //参数数量
            //是否是变长参数
            if (m_FunctionData.param) {
                var array = m_script.NewArray();
                for (var i = parameterCount - 1; i < length; ++i) {
                    array.Add(args[i]);
                }
                stackObjects[++stackIndex].SetScriptValue(array);
                for (var i = parameterCount - 2; i >= 0; --i) {
                    stackObjects[++stackIndex].CopyFrom(i >= length ? ScriptValue.Null : args[i]);
                }
            } else {
                for (var i = parameterCount - 1; i >= 0; --i) {
                    stackObjects[++stackIndex].CopyFrom(i >= length ? ScriptValue.Null : args[i]);
                }
            }
            #endregion
            var parameters = ScorpioUtil.Parameters; //传递参数
            var iInstruction = 0; //当前执行命令索引
            var m_scriptInstructions = m_FunctionData.scriptInstructions;
            var iInstructionCount = m_scriptInstructions.Length; //指令数量
            var m_global = m_script.Global;
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
                        switch (opcode) {
                            #region Load
                            case Opcode.LoadConstDouble: {
                                stackObjects[++stackIndex].setDoubleValue = constDouble[opvalue];
                                continue;
                            }
                            case Opcode.LoadConstNull: {
                                stackObjects[++stackIndex].SetNull();
                                continue;
                            }
                            case Opcode.LoadConstTrue: {
                                stackObjects[++stackIndex].SetTrue();
                                continue;
                            }
                            case Opcode.LoadConstFalse: {
                                stackObjects[++stackIndex].SetFalse();
                                continue;
                            }
                            case Opcode.LoadConstLong: {
                                stackObjects[++stackIndex].setLongValue = constLong[opvalue];
                                continue;
                            }
                            case Opcode.LoadConstString: {
                                stackObjects[++stackIndex].SetStringValue(constString[opvalue]);
                                continue;
                            }
                            case Opcode.LoadLocal: {
                                stackObjects[++stackIndex].CopyFrom(variableObjects[opvalue]);
                                continue;
                            }
                            case Opcode.LoadInternal: {
                                stackObjects[++stackIndex].CopyFrom(internalValues[opvalue].value);
                                continue;
                            }
                            case Opcode.LoadValue: {
                                stackObjects[stackIndex].CopyFrom(stackObjects[stackIndex].GetValueByIndex(opvalue, m_script));
                                continue;
                            }
                            case Opcode.LoadValueString: {
                                stackObjects[stackIndex].CopyFrom(stackObjects[stackIndex].GetValueByString(constString[opvalue], m_script));
                                continue;
                            }
                            case Opcode.LoadValueObject: {
                                stackObjects[stackIndex - 1].CopyFrom(stackObjects[stackIndex - 1].GetValueByScriptValue(stackObjects[stackIndex], m_script));
                                --stackIndex;
                                continue;
                            }
                            case Opcode.LoadValueObjectDup: {
                                stackObjects[stackIndex + 1].CopyFrom(stackObjects[stackIndex - 1].GetValueByScriptValue(stackObjects[stackIndex], m_script));
                                ++stackIndex;
                                continue;
                            }
                            case Opcode.LoadGlobal: {
                                stackObjects[++stackIndex].CopyFrom(m_global.GetValueByIndex(opvalue));
                                continue;
                            }
                            case Opcode.LoadGlobalString: {
                                stackObjects[++stackIndex].CopyFrom(m_global.GetValue(constString[opvalue]));
                                instruction.SetOpcode(Opcode.LoadGlobal, m_global.GetIndex(constString[opvalue]));
                                continue;
                            }
                            case Opcode.CopyStackTop: {
                                stackObjects[++stackIndex].CopyFrom(stackObjects[stackIndex - 1]);
                                continue;
                            }
                            case Opcode.CopyStackTopIndex: {
                                stackObjects[++stackIndex].CopyFrom(stackObjects[stackIndex - opvalue - 1]);
                                continue;
                            }
                            case Opcode.LoadBase: {
#if EXECUTE_BASE
                                stackObjects[++stackIndex].CopyFrom(baseType.PrototypeValue);
#else
                                stackObjects[++stackIndex].CopyFrom(thisObject.Get<ScriptInstance>().Prototype.PrototypeValue);
#endif
                                continue;
                            }
                            case Opcode.ToGlobal: {
                                tempIndex = m_global.GetIndex(stackObjects[stackIndex--].stringValue);
                                m_global.SetValueByIndex(tempIndex, stackObjects[stackIndex]);
                                instruction.SetOpcode(Opcode.LoadGlobal, tempIndex);
                                for (var i = 0; i < opvalue; ++i) {
                                    m_scriptInstructions[iInstruction - i - 2].SetOpcode(Opcode.Nop);
                                }
                                continue;
                            }
                            case Opcode.ToGlobalFunction: {
                                tempIndex = m_global.GetIndex(stackObjects[stackIndex--].stringValue);
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
                                variableObjects[opvalue].CopyFrom(stackObjects[stackIndex]);
                                continue;
                            }
                            case Opcode.StoreInternalAssign: {
                                internalValues[opvalue].value.CopyFrom(stackObjects[stackIndex]);
                                continue;
                            }
                            case Opcode.StoreValueStringAssign: {
                                tempIndex = stackIndex;
                                stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex]);
                                stackObjects[--stackIndex].CopyFrom(stackObjects[tempIndex]);
                                continue;
                            }
                            case Opcode.StoreValueObjectAssign: {
                                tempIndex = stackIndex;
                                stackObjects[stackIndex - 2].SetValueByScriptValue(stackObjects[stackIndex - 1], stackObjects[stackIndex]);
                                stackObjects[stackIndex -= 2].CopyFrom(stackObjects[tempIndex]);
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
                                stackObjects[--stackIndex].CopyFrom(stackObjects[tempIndex]);
                                continue;
                            }
                            #endregion
                            #region Store
                            //-----------------下面为普通赋值操作 不压入结果
                            case Opcode.StoreLocal: {
                                variableObjects[opvalue].CopyFrom(stackObjects[stackIndex--]);
                                continue;
                            }
                            case Opcode.StoreInternal: {
                                internalValues[opvalue].value.CopyFrom(stackObjects[stackIndex--]);
                                continue;
                            }
                            case Opcode.StoreValueString: {
                                stackObjects[stackIndex - 1].SetValue(constString[opvalue], stackObjects[stackIndex]);
                                stackIndex -= 2;
                                continue;
                            }
                            case Opcode.StoreValueObject: {
                                stackObjects[stackIndex - 2].SetValueByScriptValue(stackObjects[stackIndex - 1], stackObjects[stackIndex]);
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
                                        stackObjects[tempIndex].SetStringValue(stackObjects[tempIndex].stringValue + stackObjects[stackIndex].ToString());
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Plus(stackObjects[stackIndex]));
                                        --stackIndex;
                                        continue;
                                    }
                                    default: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].SetStringValue(stackObjects[tempIndex].ToString() + stackObjects[stackIndex].stringValue);
                                        } else {
                                            if (tempValueType == ScriptValue.doubleValueType) {
                                                stackObjects[tempIndex].doubleValue += stackObjects[stackIndex].ToDouble();
                                            } else if (tempValueType == ScriptValue.int64ValueType) {
                                                if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                                    stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                                    stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue + stackObjects[stackIndex].doubleValue;
                                                } else {
                                                    stackObjects[tempIndex].longValue += stackObjects[stackIndex].ToLong();
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
                                        stackObjects[tempIndex].doubleValue -= stackObjects[stackIndex].ToDouble();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Minus(stackObjects[stackIndex]));
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue - stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue -= stackObjects[stackIndex].ToLong();
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
                                        stackObjects[tempIndex].doubleValue *= stackObjects[stackIndex].ToDouble();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Multiply(stackObjects[stackIndex]));
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue * stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue *= stackObjects[stackIndex].ToLong();
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
                                        stackObjects[tempIndex].doubleValue /= stackObjects[stackIndex].ToDouble();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Divide(stackObjects[stackIndex]));
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue / stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue /= stackObjects[stackIndex].ToLong();
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
                                        stackObjects[tempIndex].doubleValue %= stackObjects[stackIndex].ToDouble();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Modulo(stackObjects[stackIndex]));
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.doubleValueType) {
                                            stackObjects[tempIndex].valueType = ScriptValue.doubleValueType;
                                            stackObjects[tempIndex].doubleValue = stackObjects[tempIndex].longValue % stackObjects[stackIndex].doubleValue;
                                        } else {
                                            stackObjects[tempIndex].longValue %= stackObjects[stackIndex].ToLong();
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
                                        stackObjects[tempIndex].longValue |= stackObjects[stackIndex].ToLong();
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
                                                stackObjects[tempIndex].SetTrue();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.InclusiveOr(stackObjects[stackIndex]));
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
                                        stackObjects[tempIndex].longValue &= stackObjects[stackIndex].ToLong();
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
                                                stackObjects[tempIndex].SetFalse();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Combine(stackObjects[stackIndex]));
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
                                        stackObjects[tempIndex].longValue ^= stackObjects[stackIndex].ToLong();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.XOR(stackObjects[stackIndex]));
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
                                        stackObjects[tempIndex].longValue <<= stackObjects[stackIndex].ToInt32();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Shi(stackObjects[stackIndex]));
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
                                        stackObjects[tempIndex].longValue >>= stackObjects[stackIndex].ToInt32();
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].Set(stackObjects[tempIndex].scriptValue.Shr(stackObjects[stackIndex]));
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
                                        stackObjects[stackIndex].SetFalse();
                                        continue;
                                    case ScriptValue.falseValueType:
                                    case ScriptValue.nullValueType:
                                        stackObjects[stackIndex].SetTrue();
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
                                            stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].index == stackObjects[stackIndex].index;
                                        } else {
                                            stackObjects[tempIndex].SetFalse();
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
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.Equals(stackObjects[stackIndex]);
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
                                            stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].index != stackObjects[stackIndex].index;
                                        } else {
                                            stackObjects[tempIndex].SetTrue();
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
                                        stackObjects[tempIndex].setBoolValue = !stackObjects[tempIndex].scriptValue.Equals(stackObjects[stackIndex]);
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
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue < stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
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
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue < stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.Less(stackObjects[stackIndex]);
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
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue <= stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
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
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue <= stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.LessOrEqual(stackObjects[stackIndex]);
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
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue > stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
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
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue > stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.Greater(stackObjects[stackIndex]);
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
                                            stackObjects[tempIndex].valueType = stackObjects[tempIndex].doubleValue >= stackObjects[stackIndex].ToDouble() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
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
                                                stackObjects[tempIndex].valueType = stackObjects[tempIndex].longValue >= stackObjects[stackIndex].ToLong() ? ScriptValue.trueValueType : ScriptValue.falseValueType;
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.GreaterOrEqual(stackObjects[stackIndex]);
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
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].doubleValue == stackObjects[stackIndex].longValue;
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetFalse();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.stringValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].setBoolValue = ReferenceEquals(stackObjects[tempIndex].stringValue, stackObjects[stackIndex].stringValue);
                                        } else {
                                            stackObjects[tempIndex].SetFalse();
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.trueValueType:
                                    case ScriptValue.nullValueType:
                                    case ScriptValue.falseValueType: {
                                        stackObjects[tempIndex].setBoolValue = tempValueType == stackObjects[stackIndex].valueType;
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].longValue == stackObjects[stackIndex].longValue;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].longValue == stackObjects[stackIndex].doubleValue;
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetFalse();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].scriptValue.EqualReference(stackObjects[stackIndex]);
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
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].doubleValue;
                                                break;
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].doubleValue != stackObjects[stackIndex].longValue;
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetTrue();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.stringValueType: {
                                        if (stackObjects[stackIndex].valueType == ScriptValue.stringValueType) {
                                            stackObjects[tempIndex].setBoolValue = !ReferenceEquals(stackObjects[tempIndex].stringValue, stackObjects[stackIndex].stringValue);
                                        } else {
                                            stackObjects[tempIndex].SetTrue();
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.trueValueType:
                                    case ScriptValue.nullValueType:
                                    case ScriptValue.falseValueType: {
                                        stackObjects[tempIndex].setBoolValue = tempValueType != stackObjects[stackIndex].valueType;
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.int64ValueType: {
                                        switch (stackObjects[stackIndex].valueType) {
                                            case ScriptValue.int64ValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].longValue != stackObjects[stackIndex].longValue;
                                                break;
                                            case ScriptValue.doubleValueType:
                                                stackObjects[tempIndex].setBoolValue = stackObjects[tempIndex].longValue != stackObjects[stackIndex].doubleValue;
                                                break;
                                            default:
                                                stackObjects[tempIndex].SetTrue();
                                                break;
                                        }
                                        --stackIndex;
                                        continue;
                                    }
                                    case ScriptValue.scriptValueType: {
                                        stackObjects[tempIndex].setBoolValue = !stackObjects[tempIndex].scriptValue.EqualReference(stackObjects[stackIndex]);
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
                                    stackObjects[stackIndex].Set(stackObjects[stackIndex].Call(ScriptValue.Null, parameters, opvalue));
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
                                    stackObjects[++stackIndex].Set(func.Call(parent, parameters, opvalue));
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
                            case Opcode.CallEmpty: {
                                var func = stackObjects[stackIndex--];
                                var parent = stackObjects[stackIndex--];
                                stackObjects[++stackIndex].Set(func.Call(parent));
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
                                        stackObjects[stackIndex].SetTrue();
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
                                        stackObjects[stackIndex].SetFalse();
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
                                m_script.CoroutineResult.CopyFrom(stackObjects[stackIndex]);
                                yield break;
#else
                                var ret = stackObjects[stackIndex].Reference();
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(ScriptValue.Null, parameters, parameterIndex));
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(parent, parameters, parameterIndex));
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
                                    stackObjects[++stackIndex].Set(func.Call(thisObject, parameters, opvalue, prototype.Get<ScriptType>()));
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
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.Call(thisObject, parameters, parameterIndex, prototype.Get<ScriptType>()));
                                } finally {
                                    m_script.PopStackInfo();
                                }
                                continue;
                            }
#if EXECUTE_COROUTINE
                            case Opcode.Await: {
                                yield return stackObjects[stackIndex--].Value;
                                continue;
                            }
                            case Opcode.NewAwait: {
                                yield return stackObjects[stackIndex--].Value;
                                stackObjects[++stackIndex].CopyFrom(m_script.CoroutineResult);
                                m_script.CoroutineResult.SetNull();
                                continue;
                            }
                            case Opcode.CallAsync: {
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    parameters[i] = stackObjects[stackIndex--];
                                }
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[stackIndex].Set(stackObjects[stackIndex].CallAsync(ScriptValue.Null, parameters, opvalue));
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
                                    stackObjects[++stackIndex].Set(func.CallAsync(parent, parameters, opvalue));
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(ScriptValue.Null, parameters, parameterIndex));
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
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(parent, parameters, parameterIndex));
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
                                    stackObjects[++stackIndex].Set(func.CallAsync(thisObject, parameters, opvalue, prototype.Get<ScriptType>()));
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
                                var prototype = stackObjects[stackIndex--];
                                m_script.PushStackInfo(m_Breviary, instruction.line);
                                try {
                                    stackObjects[++stackIndex].Set(func.CallAsync(thisObject, parameters, parameterIndex, prototype.Get<ScriptType>()));
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
#if SCORPIO_DEBUG
                                map.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                stackObjects[++stackIndex].SetScriptValue(map);
                                continue;
                            }
                            case Opcode.NewArray: {
                                var array = m_script.NewArray();
#if SCORPIO_DEBUG
                                array.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                array.SetArrayCapacity(opvalue);
                                for (var i = opvalue - 1; i >= 0; --i) {
                                    array.Add(stackObjects[stackIndex - i]);
                                }
                                stackIndex -= opvalue;
                                stackObjects[++stackIndex].SetScriptValue(array);
                                continue;
                            }
                            case Opcode.NewFunction: {
                                var function = m_script.NewFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData);
#if SCORPIO_DEBUG
                                function.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function);
                                continue;
                            }
                            case Opcode.NewLambdaFunction: {
                                var function = m_script.NewBindFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData, thisObject);
#if SCORPIO_DEBUG
                                function.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function);
                                continue;
                            }
                            case Opcode.NewAsyncFunction: {
                                var function = m_script.NewAsyncFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData);
#if SCORPIO_DEBUG
                                function.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function);
                                continue;
                            }
                            case Opcode.NewAsyncLambdaFunction: {
                                var function = m_script.NewAsyncBindFunction();
                                var functionData = constContexts[opvalue];
                                var internals = functionData.m_FunctionData.internals;
                                function.SetContext(functionData, thisObject);
#if SCORPIO_DEBUG
                                function.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                for (var i = 0; i < internals.Length; ++i) {
                                    var internalIndex = internals[i];
                                    function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                }
                                stackObjects[++stackIndex].SetScriptValue(function);
                                continue;
                            }
                            case Opcode.NewType: {
                                var classData = constClasses[opvalue];
                                var parentType = classData.parent >= 0 ? m_global.GetValue(constString[classData.parent]) : m_script.TypeObjectValue;
                                var className = constString[classData.name];
                                string functionName;
                                var type = m_script.NewType();
#if SCORPIO_DEBUG
                                type.Source = $"{m_Breviary}:{instruction.line}";
#endif
                                type.Set(className, parentType);
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
                                    functionName = constString[func >> 32];
                                    if (funcType == 0) {
                                        var function = m_script.NewFunction().SetContext(functionData);
#if SCORPIO_DEBUG
                                        function.Source = $"{className}.{functionName}";
#endif
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.SetValue(functionName, function);
                                    } else if (funcType == 1) {
                                        var function = m_script.NewAsyncFunction().SetContext(functionData);
#if SCORPIO_DEBUG
                                        function.Source = $"{className}.{functionName}";
#endif
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.SetValue(functionName, function);
                                    } else if (funcType == 2) {
                                        var function = m_script.NewFunction().SetContext(functionData);
#if SCORPIO_DEBUG
                                        function.Source = $"{className}.{functionName}";
#endif
                                        for (var i = 0; i < internals.Length; ++i) {
                                            var internalIndex = internals[i];
                                            function.SetInternal(internalIndex & 0xffff, internalValues[internalIndex >> 16]);
                                        }
                                        type.AddGetProperty(functionName, function);
                                    }
                                }
                                stackObjects[++stackIndex].SetScriptValue(type);
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
                            stackObjects[stackIndex = 0].CopyFrom(e.value);
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
                            stackObjects[stackIndex = 0].Set(ScriptValue.CreateValue(m_script, e));
                            iInstruction = tryStack[tryIndex--];
                            goto KeepOn;
                        } else {
                            throw;
                        }
                        //其他错误
                    } catch (System.Exception e) {
                        if (tryIndex > -1) {
                            stackObjects[stackIndex = 0].Set(ScriptValue.CreateValue(m_script, e));
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
