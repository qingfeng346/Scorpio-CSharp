using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //去反射机制函数的数据
    public class ScorpioMethodInfo {
        public string Name;
        public bool IsStatic;
        public Type[] ParameterType;
        public bool Params;
        public Type ParamType;
        public string ParameterTypes;
        public ScorpioMethodInfo(string name, bool isStatic, Type[] parameterType, bool param, Type paramType, string parameterTypes) {
            this.Name = name;
            this.IsStatic = isStatic;
            this.ParameterType = parameterType;
            this.Params = param;
            this.ParamType = paramType;
            this.ParameterTypes = parameterTypes;
        }
    }
    /// <summary> 一个类的同名函数 </summary>
    public class UserdataMethod {
        //函数基类
        private abstract class FunctionBase {
            public Type[] ParameterType;                //所有参数类型
            public int RequiredNumber;                  //必须的参数个数
            public int ParameterCount;                  //除去变长参数的参数个数
            public object[] DefaultParameter;           //默认参数
            public bool Params;                         //是否是变长参数
            public Type ParamType;                      //变长参数类型
            public string ParameterTypes;               //传递参数的类型
            public object[] Args;                       //参数数组（预创建 可以共用）
            public bool IsValid;                        //是否是有效的函数 (模版函数没有声明的时候就是无效的)
            public FunctionBase(Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) {
                this.ParameterType = ParameterType;
                this.DefaultParameter = DefaultParameter;
                this.ParamType = ParamType;
                this.Params = Params;
                this.ParameterTypes = ParameterTypes;
                this.Args = new object[ParameterType.Length];
                RequiredNumber = ParameterType.Length;
                ParameterCount = ParameterType.Length;
                if (DefaultParameter != null) RequiredNumber -= DefaultParameter.Length;
                if (Params) {
                    --RequiredNumber;
                    --ParameterCount;
                }
            }
            public abstract object Invoke(object obj, Type type);
        }
        //构造函数
        private class FunctionConstructor : FunctionBase {
            public ConstructorInfo Constructor;         //构造函数对象
            public FunctionConstructor(ConstructorInfo Constructor, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) :
                base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes) {
                this.IsValid = true;
                this.Constructor = Constructor;
            }
            public override object Invoke(object obj, Type type) {
                return Constructor.Invoke(Args);
            }
        }
        //普通函数
        private class FunctionMethod : FunctionBase {
            public MethodInfo Method;                   //普通函数对象
            public FunctionMethod(MethodInfo Method, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) : 
                base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes) {
                this.Method = Method;
                this.IsValid = !Method.IsGenericMethod || !Method.ContainsGenericParameters;
            }
            public override object Invoke(object obj, Type type) {
                return Method.Invoke(obj, Args);
            }
        }
        //去反射函数
        private class FunctionFastMethod : FunctionBase {
            public IScorpioFastReflectMethod Method;
            public FunctionFastMethod(IScorpioFastReflectMethod Method, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) :
                base(ParameterType, null, ParamType, Params, ParameterTypes) {
                this.IsValid = true;
                this.Method = Method;
            }
            public override object Invoke(object obj, Type type) {
                return Method.Call(obj, ParameterTypes, Args);
            }
        }
        private Script m_Script;                        //所在脚本引擎
        private Type m_Type;                            //所在类
        private int m_Count;                            //相同名字函数数量
        private FunctionBase[] m_Methods;               //所有函数对象
        private string m_MethodName;                    //函数名字
        private bool m_IsStatic;                        //是否是静态函数
        public string MethodName { get { return m_MethodName; } }
        public bool IsStatic { get { return m_IsStatic; } }
        public UserdataMethod() { }
        private UserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods) {
            m_Script = script;
            m_IsStatic = methods[0].IsStatic;
            List<MethodBase> methodBases = new List<MethodBase>();
            methodBases.AddRange(methods.ToArray());
            Initialize_impl(type, methodName, methodBases);
        }
        protected void Initialize(Script script, Type type, string methodName, MethodInfo[] methods) {
            m_Script = script;
            List<MethodBase> methodBases = new List<MethodBase>();
            foreach (MethodInfo method in methods) {
                if (method.Name.Equals(methodName))
                    methodBases.Add(method);
            }
            m_IsStatic = methodBases.Count > 0 ? methodBases[0].IsStatic : false;
            Initialize_impl(type, methodName, methodBases);
        }
        protected void Initialize(Script script, Type type, string methodName, ConstructorInfo[] cons) {
            m_Script = script;
            m_IsStatic = false;
            List<MethodBase> methods = new List<MethodBase>();
            methods.AddRange(cons);
            Initialize_impl(type, methodName, methods);
        }
        private void Initialize_impl(Type type, string methodName, List<MethodBase> methods)
        {
            m_Type = type;
            m_MethodName = methodName;
            List<FunctionBase> functionMethod = new List<FunctionBase>();
            bool Params = false;
            Type ParamType = null;
            string ParameterTypes = null;
            MethodBase method = null;
            List<Type> parameters = new List<Type>();
            List<object> defaultParameter = new List<object>();
            int length = methods.Count;
            for (int i = 0; i < length; ++i) {
                Params = false;
                ParamType = null;
                ParameterTypes = "";
                parameters.Clear();
                defaultParameter.Clear();
                method = methods[i];
                ParameterInfo[] pars = method.GetParameters();
                foreach (ParameterInfo par in pars) {
                    ParameterTypes += (par.ParameterType.FullName + "+");
                    parameters.Add(par.ParameterType);
                    if (par.DefaultValue != DBNull.Value) { defaultParameter.Add(par.DefaultValue); }
                    Params = Util.IsParamArray(par);
                    if (Params) ParamType = par.ParameterType.GetElementType();
                }
                if (method is MethodInfo)
                    functionMethod.Add(new FunctionMethod(method as MethodInfo, parameters.ToArray(), defaultParameter.ToArray(), ParamType, Params, ParameterTypes));
                else
                    functionMethod.Add(new FunctionConstructor(method as ConstructorInfo, parameters.ToArray(), defaultParameter.ToArray(), ParamType, Params, ParameterTypes));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        protected void Initialize(bool isStatic, Script script, Type type, string methodName, ScorpioMethodInfo[] methods, IScorpioFastReflectMethod fastMethod) {
            m_Script = script;
            m_IsStatic = isStatic;
            m_Type = type;
            m_MethodName = methodName;
            List<FunctionBase> functionMethod = new List<FunctionBase>();
            foreach (ScorpioMethodInfo method in methods) {
                functionMethod.Add(new FunctionFastMethod(fastMethod, method.ParameterType, method.ParamType, method.Params, method.ParameterTypes));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public object Call(object obj, ScriptObject[] parameters) {
            FunctionBase methodInfo = null;
            FunctionBase functionBase = null;
            for (int i = 0; i < m_Count; ++i) {
                functionBase = m_Methods[i];
                //是否是有效函数 模版原型函数是无效的
                if (functionBase.IsValid) {
                    bool fit = true;
                    int requiredNumber = functionBase.RequiredNumber;       //函数必须传入的参数个数
                    int parameterNumber = parameters.Length;                //脚本传入的参数个数
                    if (parameterNumber >= requiredNumber) {
                        //变长参数函数
                        if (functionBase.Params) {
                            for (int j = 0; j < parameterNumber; ++j) {
                                if (!Util.CanChangeType(parameters[j], j >= functionBase.ParameterCount ? functionBase.ParamType : functionBase.ParameterType[j])) {
                                    fit = false;
                                    break;
                                }
                            }
                        } else {
                            for (int j = 0; j < parameterNumber && j < functionBase.ParameterCount; ++j) {
                                if (!Util.CanChangeType(parameters[j], functionBase.ParameterType[j])) {
                                    fit = false;
                                    break;
                                }
                            }
                        }
                        if (fit) {
                            methodInfo = functionBase;
                            break;
                        }
                    }
                }
            }
            try {
                if (methodInfo != null) {
                    int requiredNumber = functionBase.RequiredNumber;       //函数必须传入的参数个数
                    int parameterNumber = parameters.Length;                //脚本传入的参数个数
                    int parameterCount = functionBase.ParameterCount;       //除了变长参数的参数个数
                    object[] args = methodInfo.Args;                        //参数数组
                    if (methodInfo.Params) {
                        for (int i = 0; i < parameterCount; ++i)
                            args[i] = i >= parameterNumber ? functionBase.DefaultParameter[i - requiredNumber] : Util.ChangeType(m_Script, parameters[i], methodInfo.ParameterType[i]);
                        if (parameterNumber > parameterCount) {
                            Array array = Array.CreateInstance(methodInfo.ParamType, parameterNumber - parameterCount);
                            for (int i = parameterCount; i < parameterNumber; ++i)
                                array.SetValue(Util.ChangeType(m_Script, parameters[i], methodInfo.ParamType), i - parameterCount);
                            args[parameterCount] = array;
                        } else {
                            args[parameterCount] = Array.CreateInstance(methodInfo.ParamType, 0);
                        }
                        return methodInfo.Invoke(obj, m_Type);
                    } else {
                        for (int i = 0; i < parameterCount; ++i)
                            args[i] = i >= parameterNumber ? functionBase.DefaultParameter[i - requiredNumber] : Util.ChangeType(m_Script, parameters[i], methodInfo.ParameterType[i]);
                        return methodInfo.Invoke(obj, m_Type);
                    }
                }
            } catch (System.Exception e) {
                throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 调用函数出错 [" + MethodName + "] : " + e.ToString());
            }
            throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 找不到合适的函数 [" + MethodName + "]");
        }
        public UserdataMethod MakeGenericMethod(Type[] parameters) {
            List<MethodInfo> methods = new List<MethodInfo>();
            for (int i = 0; i < m_Count; ++i) {
                FunctionMethod method = m_Methods[i] as FunctionMethod;
                if (method != null) {
                    if (method.Method.IsGenericMethod) {
                        Type[] types = method.Method.GetGenericArguments();
                        if (types.Length == parameters.Length) {
                            bool accord = true;
                            int length = types.Length;
                            for (int j = 0; j < length; ++j) {
                                if (!types[j].GetTypeInfo().BaseType.GetTypeInfo().IsAssignableFrom(parameters[j])) {
                                    accord = false;
                                    break;
                                }
                            }
                            if (accord) {
                                methods.Add(method.Method.MakeGenericMethod(parameters));
                                break;
                            }
                        }
                    }
                }
            }
            if (methods.Count > 0)
                return new UserdataMethod(m_Script, m_Type, MethodName, methods);
            throw new ExecutionException(m_Script, "没有找到合适的泛型函数 " + MethodName);
        }
    }
    public class ReflectUserdataMethod : UserdataMethod {
        public ReflectUserdataMethod(Script script, Type type, string methodName, MethodInfo[] methods) {
            this.Initialize(script, type, methodName, methods);
        }
        public ReflectUserdataMethod(Script script, Type type, string methodName, ConstructorInfo[] cons) {
            this.Initialize(script, type, methodName, cons);
        }
    }
    public class FastReflectUserdataMethod : UserdataMethod {
        public FastReflectUserdataMethod(bool isStatic, Script script, Type type, string methodName, ScorpioMethodInfo[] methods, IScorpioFastReflectMethod fastMethod) {
            this.Initialize(isStatic, script, type, methodName, methods, fastMethod);
        }
    }
}
