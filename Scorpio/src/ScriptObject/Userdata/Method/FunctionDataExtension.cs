using System;
using System.Reflection;
using Scorpio.Tools;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //扩展函数
    public class FunctionDataExtensionMethod : FunctionData {
        public MethodInfo Method;                   //普通函数对象
        public FunctionDataExtensionMethod(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Method = method;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            Args[0] = obj;
            return Method.Invoke(obj, Args);
        }
        //优先检查无默认值，非不定参的函数
        public override bool CheckNormalType(ScriptValue[] parameters, int length) {
            if (IsNormal && ++length == ParameterCount) {
                for (var i = 1; i < length; ++i) {
                    if (!parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 1; i < length; ++i) {
                    Args[i] = parameters[i - 1].ChangeType(ParameterType[i]);
                }
                return true;
            }
            return false;
        }
        //检查有默认参数的函数
        public override bool CheckDefaultType(ScriptValue[] parameters, int length) {
            ++length;
            if (IsDefault && length >= RequiredNumber && length <= ParameterCount) {
                for (var i = 1; i < length; ++i) {
                    if (!parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 1; i < length; ++i) {
                    Args[i] = parameters[i - 1].ChangeType(ParameterType[i]);
                }
                for (var i = length; i < ParameterCount; ++i) {
                    Args[i] = DefaultParameter[i];
                }
                return true;
            }
            return false;
        }
        //检查不定参函数
        public override bool CheckArgsType(ScriptValue[] parameters, int length) {
            ++length;
            if (IsParams && length >= RequiredNumber) {
                for (var i = 1; i < ParameterCount; ++i) {
                    if (!parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = ParameterCount; i < length; ++i) {
                    if (!parameters[i - 1].CanChangeType(ParamType)) {
                        return false;
                    }
                }
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (parameters[i - 1].ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance(ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue(parameters[i - 1].ChangeType(ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance(ParamType, 0);
                }
                return true;
            }
            return false;
        }
        //函数数量为1时,直接设置参数
        public override void SetArgs(ScriptValue[] parameters, int length) {
            ++length;
            if (IsNormal) {
                for (var i = 1; i < length; ++i) {
                    Args[i] = parameters[i - 1].ChangeType(ParameterType[i]);
                }
            } else if (IsDefault) {
                for (var i = 1; i < length; ++i) {
                    Args[i] = parameters[i - 1].ChangeType(ParameterType[i]);
                }
                for (var i = length; i < ParameterCount; ++i) {
                    Args[i] = DefaultParameter[i];
                }
            } else {
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (parameters[i - 1].ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance(ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue(parameters[i - 1].ChangeType(ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance(ParamType, 0);
                }
            }
        }
    }
    //带 ref out 参数的 普通反射函数
    public class FunctionDataExtensionMethodWithRefOut : FunctionDataExtensionMethod {
        public FunctionDataExtensionMethodWithRefOut(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(method, parameterType, defaultParameter, refOut, requiredNumber, paramType) {
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            Args[0] = obj;
            var ret = Method.Invoke(obj, Args);
            for (var i = 1; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    parameters[i - 1].Get<ScriptInstanceBase>().SetValue(RefOutValue, ScriptValue.CreateValue(Args[i]));
                }
            }
            return ret;
        }
        //优先检查无默认值，非不定参的函数
        public override bool CheckNormalType(ScriptValue[] parameters, int length) {
            if (IsNormal && ++length == ParameterCount) {
                for (var i = 1; i < length; ++i) {
                    if (RefOuts[i] ? !parameters[i - 1].GetValue(RefOutValue).CanChangeTypeRefOut(ParameterType[i]) : !parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 1; i < length; ++i) {
                    Args[i] = (RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i]);
                }
                return true;
            }
            return false;
        }
        //检查有默认参数的函数
        public override bool CheckDefaultType(ScriptValue[] parameters, int length) {
            ++length;
            if (IsDefault && length >= RequiredNumber && length <= ParameterCount) {
                for (var i = 1; i < length; ++i) {
                    if (RefOuts[i] ? !parameters[i - 1].GetValue(RefOutValue).CanChangeTypeRefOut(ParameterType[i]) : !parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? ((RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
                return true;
            }
            return false;
        }
        //检查不定参函数
        public override bool CheckArgsType(ScriptValue[] parameters, int length) {
            if (IsParams && ++length >= RequiredNumber) {
                for (var i = 1; i < ParameterCount; ++i) {
                    if (RefOuts[i] ? !parameters[i - 1].GetValue(RefOutValue).CanChangeTypeRefOut(ParameterType[i]) : !parameters[i - 1].CanChangeType(ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = ParameterCount; i < length; ++i) {
                    if (!parameters[i - 1].CanChangeType(ParamType)) {
                        return false;
                    }
                }
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? ((RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance(ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue(parameters[i - 1].ChangeType(ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance(ParamType, 0);
                }
                return true;
            }
            return false;
        }
        //函数数量为1时,直接设置参数
        public override void SetArgs(ScriptValue[] parameters, int length) {
            ++length;
            if (IsNormal) {
                for (var i = 1; i < length; ++i) {
                    Args[i] = (RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i]);
                }
            } else if (IsDefault) {
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? ((RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
            } else {
                for (var i = 1; i < ParameterCount; ++i) {
                    Args[i] = i < length ? ((RefOuts[i] ? parameters[i - 1].GetValue(RefOutValue) : parameters[i - 1]).ChangeType(ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance(ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue(parameters[i - 1].ChangeType(ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance(ParamType, 0);
                }
            }
        }
    }
}
