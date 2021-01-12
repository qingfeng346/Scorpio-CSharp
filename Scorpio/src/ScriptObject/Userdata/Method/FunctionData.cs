using System;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    //单个函数的所有数据
    public abstract class FunctionData {
        protected const string RefOutValue = "value";
        protected readonly static Type[] EmptyTypes = new Type[0];
        protected readonly static bool[] EmptyBool = new bool[0];

        public Type[] ParameterType; //所有参数类型
        public object[] DefaultParameter; //默认参数
        public object[] Args; //参数数组（预创建）
        public bool[] RefOuts; //是否是 ref out 标识
        public int RequiredNumber; //必须的参数个数
        public int ParameterCount; //除去变长参数的参数个数
        public Type ParamType; //变长参数类型

        public bool IsNormal; //是否是普通函数，没有默认参数，没有变长参数
        public bool IsDefault; //是否有默认参数函数，没有变长参数
        public bool IsParams; //是否是变长参数

        public FunctionData (Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) {
            this.ParameterType = parameterType;
            this.DefaultParameter = defaultParameter;
            this.RefOuts = refOut;
            this.RequiredNumber = requiredNumber;
            this.ParamType = paramType;
            this.ParameterCount = parameterType.Length;
            this.Args = new object[parameterType.Length];
            this.IsParams = paramType != null;

            //必须参数和总共参数不同证明有默认参数
            var hadDefault = parameterType != null && requiredNumber != parameterType.Length;
            this.IsNormal = !hadDefault && !IsParams;
            this.IsDefault = hadDefault && !IsParams;

            if (IsParams) {
                --RequiredNumber;
                --ParameterCount;
            }
        }
        //是否是静态函数
        public virtual bool IsStatic => false;
        public int Priority => IsNormal ? 0 : (IsDefault ? 1 : 2);
        public abstract object Invoke (object obj, ScriptValue[] parameters);

        //优先检查无默认值，非不定参的函数
        public virtual bool CheckNormalType (ScriptValue[] parameters, int length) {
            if (IsNormal && length == ParameterCount) {
                for (var i = 0; i < length; ++i) {
                    if (!Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 0; i < length; ++i) {
                    Args[i] = Util.ChangeType (parameters[i], ParameterType[i]);
                }
                return true;
            }
            return false;
        }
        //检查有默认参数的函数
        public virtual bool CheckDefaultType (ScriptValue[] parameters, int length) {
            if (IsDefault && length >= RequiredNumber && length <= ParameterCount) {
                for (var i = 0; i < length; ++i) {
                    if (!Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 0; i < length; ++i) {
                    Args[i] = Util.ChangeType (parameters[i], ParameterType[i]);
                }
                for (var i = length; i < ParameterCount; ++i) {
                    Args[i] = DefaultParameter[i];
                }
                return true;
            }
            return false;
        }
        //检查不定参函数
        public virtual bool CheckArgsType (ScriptValue[] parameters, int length) {
            if (IsParams && length >= RequiredNumber) {
                for (var i = 0; i < ParameterCount; ++i) {
                    if (!Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = ParameterCount; i < length; ++i) {
                    if (!Util.CanChangeType (parameters[i], ParamType)) {
                        return false;
                    }
                }
                for (var i = 0; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (Util.ChangeType (parameters[i], ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance (ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue (Util.ChangeType (parameters[i], ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance (ParamType, 0);
                }
                return true;
            }
            return false;
        }
        public void SetArgs(ScriptValue[] parameters, int length) {
            if (IsNormal) {
                for (var i = 0; i < length; ++i) {
                    Args[i] = Util.ChangeType(parameters[i], ParameterType[i]);
                }
            } else if (IsDefault) {
                for (var i = 0; i < length; ++i) {
                    Args[i] = Util.ChangeType(parameters[i], ParameterType[i]);
                }
                for (var i = length; i < ParameterCount; ++i) {
                    Args[i] = DefaultParameter[i];
                }
            } else {
                for (var i = 0; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (Util.ChangeType (parameters[i], ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance(ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue(Util.ChangeType(parameters[i], ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance(ParamType, 0);
                }
            }
        }
    }
    public abstract class FunctionDataWithRefOut : FunctionData {
        public FunctionDataWithRefOut (Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType):
            base (parameterType, defaultParameter, refOut, requiredNumber, paramType) { }
        //优先检查无默认值，非不定参的函数
        public override bool CheckNormalType (ScriptValue[] parameters, int length) {
            if (IsNormal && length == ParameterCount) {
                for (var i = 0; i < length; ++i) {
                    if (RefOuts[i] ? !Util.CanChangeTypeRefOut (parameters[i].GetValue (RefOutValue), ParameterType[i]) : !Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 0; i < length; ++i) {
                    Args[i] = Util.ChangeType (RefOuts[i] ? parameters[i].GetValue (RefOutValue) : parameters[i], ParameterType[i]);
                }
            }
            return false;
        }
        //检查有默认参数的函数
        public override bool CheckDefaultType (ScriptValue[] parameters, int length) {
            if (IsDefault && length >= RequiredNumber && length <= ParameterCount) {
                for (var i = 0; i < length; ++i) {
                    if (RefOuts[i] ? !Util.CanChangeTypeRefOut (parameters[i].GetValue (RefOutValue), ParameterType[i]) : !Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = 0; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (Util.ChangeType (RefOuts[i] ? parameters[i].GetValue (RefOutValue) : parameters[i], ParameterType[i])) : DefaultParameter[i];
                }
                return true;
            }
            return false;
        }
        //检查不定参函数
        public override bool CheckArgsType (ScriptValue[] parameters, int length) {
            if (IsParams && length >= RequiredNumber) {
                for (var i = 0; i < ParameterCount; ++i) {
                    if (RefOuts[i] ? !Util.CanChangeTypeRefOut (parameters[i].GetValue (RefOutValue), ParameterType[i]) : !Util.CanChangeType (parameters[i], ParameterType[i])) {
                        return false;
                    }
                }
                for (var i = ParameterCount; i < length; ++i) {
                    if (!Util.CanChangeType (parameters[i], ParamType)) {
                        return false;
                    }
                }
                for (var i = 0; i < ParameterCount; ++i) {
                    Args[i] = i < length ? (Util.ChangeType (RefOuts[i] ? parameters[i].GetValue (RefOutValue) : parameters[i], ParameterType[i])) : DefaultParameter[i];
                }
                if (length > ParameterCount) {
                    var array = Array.CreateInstance (ParamType, length - ParameterCount);
                    for (int i = ParameterCount; i < length; ++i)
                        array.SetValue (Util.ChangeType (parameters[i], ParamType), i - ParameterCount);
                    Args[ParameterCount] = array;
                } else {
                    Args[ParameterCount] = Array.CreateInstance (ParamType, 0);
                }
                return true;
            }
            return false;
        }
    }
}