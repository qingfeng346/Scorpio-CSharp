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
            public bool IsGeneric;                      //是否是模板函数
            public bool HasDefault;                     //是否有默认参数
            public FunctionBase(Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) {
                this.ParameterType = ParameterType;
                this.DefaultParameter = DefaultParameter;
                this.ParamType = ParamType;
                this.Params = Params;
                this.ParameterTypes = ParameterTypes;
                this.Args = new object[ParameterType.Length];
                this.HasDefault = false;
                this.RequiredNumber = ParameterType.Length;
                this.ParameterCount = ParameterType.Length;
                if (DefaultParameter != null && DefaultParameter.Length > 0) {
                    HasDefault = true;
                    RequiredNumber -= DefaultParameter.Length;
                }
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
                this.IsGeneric = false;
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
                this.IsGeneric = Method.IsGenericMethod && Method.ContainsGenericParameters;
            }
            public override object Invoke(object obj, Type type) {
                return Method.Invoke(obj, Args);
            }
        }
        //扩展函数
        private class ExtensionMethod : FunctionBase {
            public MethodInfo Method;                   //普通函数对象
            private object[] FinishArgs;
            public ExtensionMethod(MethodInfo Method, Type[] ParameterType, object[] DefaultParameter, Type ParamType, bool Params, string ParameterTypes) : 
                base(ParameterType, DefaultParameter, ParamType, Params, ParameterTypes) {
                this.FinishArgs = new object[Args.Length + 1];
                this.Method = Method;
                this.IsGeneric = Method.IsGenericMethod && Method.ContainsGenericParameters;
            }
            public override object Invoke(object obj, Type type) {
                FinishArgs[0] = obj;
                Array.Copy(Args, 0, FinishArgs, 1, Args.Length);
                return Method.Invoke(null, FinishArgs);
            }
        }
        //去反射函数
        private class FunctionFastMethod : FunctionBase {
            public IScorpioFastReflectMethod Method;
            public FunctionFastMethod(IScorpioFastReflectMethod Method, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) :
                base(ParameterType, null, ParamType, Params, ParameterTypes) {
                this.IsGeneric = false;
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
        private int m_GenericCount;                     //模板函数数量
        private FunctionBase[] m_GenericMethods;        //所有模板函数
        private string m_MethodName;                    //函数名字
        private bool m_IsStatic;                        //是否是静态函数
        private bool m_IsClass;                         //是否是class
        public string MethodName { get { return m_MethodName; } }
        public bool IsStatic { get { return m_IsStatic; } }
        public bool IsClass { get { return m_IsClass; } }
        public UserdataMethod() { }
        private UserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods) {
            m_Script = script;
            m_IsStatic = methods[0].IsStatic;
            List<MethodBase> methodBases = new List<MethodBase>();
            methodBases.AddRange(methods.ToArray());
            Initialize_impl(type, methodName, methodBases);
        }
        protected void Initialize(Script script, Type type, string methodName, List<MethodInfo> methods) {
            m_Script = script;
            List<MethodBase> methodBases = new List<MethodBase>();
            foreach (MethodInfo method in methods) {
                if (method.Name.Equals(methodName))
                    methodBases.Add(method);
            }
            m_IsStatic = methodBases.Count == 0 ? false : (Util.IsExtensionMethod(methodBases[0]) ? false : methodBases[0].IsStatic);
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
            m_IsClass = type.IsClass;
            m_MethodName = methodName;
            List<FunctionBase> functionMethod = new List<FunctionBase>();
            List<FunctionBase> genericMethods = new List<FunctionBase>();
            bool Params = false;                                    //是否是不定参函数
            Type ParamType = null;                                  //不定参类型
            ParameterInfo[] Parameters;                             //所有参数
            List<Type> parameterTypes = new List<Type>();           //参数类型
            List<object> defaultParameter = new List<object>();     //默认参数
            int length = methods.Count;                             //总数量
            MethodBase method = null;
            FunctionBase functionBase;
            for (int i = 0; i < length; ++i) {
                method = methods[i];
                Params = false;
                ParamType = null;
                parameterTypes.Clear();
                defaultParameter.Clear();
                Parameters = method.GetParameters();
                if (Util.IsExtensionMethod(method)) {
                    for (int j = 1; j < Parameters.Length; ++j) {
                        var par = Parameters[j];
                        parameterTypes.Add(par.ParameterType);
                        if (par.DefaultValue != DBNull.Value) { defaultParameter.Add(par.DefaultValue); }
                        Params = Util.IsParamArray(par);
                        if (Params) ParamType = par.ParameterType.GetElementType();
                    }
                    functionBase = new ExtensionMethod(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), ParamType, Params, "");
                } else {
                    foreach (ParameterInfo par in Parameters) {
                        parameterTypes.Add(par.ParameterType);
                        if (par.DefaultValue != DBNull.Value) { defaultParameter.Add(par.DefaultValue); }
                        Params = Util.IsParamArray(par);
                        if (Params) ParamType = par.ParameterType.GetElementType();
                    }
                    if (method is MethodInfo)
                        functionBase = new FunctionMethod(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), ParamType, Params, "");
                    else
                        functionBase = new FunctionConstructor(method as ConstructorInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), ParamType, Params, "");
                }
                if (functionBase.IsGeneric)
                    genericMethods.Add(functionBase);
                else
                    functionMethod.Add(functionBase);
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
            m_GenericMethods = genericMethods.ToArray();
            m_GenericCount = m_GenericMethods.Length;
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
        bool CheckNormalType(ScriptObject[] pars, Type[] types, int count) {
            if (pars.Length != count)
                return false;
            for (int i = 0; i < count; ++i) {
                if (!Util.CanChangeType(pars[i], types[i]))
                    return false;
            }
            return true;
        }
        bool CheckDefaultType(ScriptObject[] parameters, Type[] types, int requiredNumber, int count) {
            int length = parameters.Length;
            if (length < requiredNumber || length > count)
                return false;
            for (int i = 0; i < length; ++i) {
                if (!Util.CanChangeType(parameters[i], types[i]))
                    return false;
            }
            return true;
        }
        bool CheckArgsType(ScriptObject[] parameters, Type[] types, Type paramType, int requiredNumber, int count) {
            int length = parameters.Length;
            if (length < requiredNumber) {
                return false;
            }
            for (int i = 0; i < length; ++i) {
                if (!Util.CanChangeType(parameters[i], i >= count ? paramType : types[i])) {
                    return false;
                }
            }
            return true;
        }
        public object Call(object obj, ScriptObject[] parameters) {
            try {
                int parameterNumber = parameters.Length;              //脚本传入的参数个数
                if (!m_IsClass && parameterNumber == 0) {
                    return Activator.CreateInstance(m_Type);
                } else {
                    FunctionBase methodInfo = null;
                    FunctionBase functionBase = null;
                    for (int i = 0; i < m_Count; ++i) {
                        functionBase = m_Methods[i];
                        if (functionBase.Params || functionBase.HasDefault) { continue; }
                        if (CheckNormalType(parameters, functionBase.ParameterType, functionBase.ParameterCount)) {
                            methodInfo = functionBase;
                            goto callMethod;
                        }
                    }
                    for (int i = 0; i < m_Count; ++i) {
                        functionBase = m_Methods[i];
                        if (functionBase.Params || !functionBase.HasDefault) { continue; }
                        if (CheckDefaultType(parameters, functionBase.ParameterType, functionBase.RequiredNumber, functionBase.ParameterCount)) {
                            methodInfo = functionBase;
                            goto callMethod;
                        }
                    }
                    for (int i = 0; i < m_Count; ++i) {
                        functionBase = m_Methods[i];
                        if (!functionBase.Params) { continue; }
                        if (CheckArgsType(parameters, functionBase.ParameterType, functionBase.ParamType, functionBase.RequiredNumber, functionBase.ParameterCount)) {
                            methodInfo = functionBase;
                            goto callMethod;
                        }
                    }
                    callMethod:
                    if (methodInfo != null) {
                        int requiredNumber = methodInfo.RequiredNumber;       //函数必须传入的参数个数
                        int parameterCount = methodInfo.ParameterCount;       //参数个数
                        object[] args = methodInfo.Args;                      //参数数组
                        if (methodInfo.Params) {
                            for (int i = 0; i < parameterCount; ++i)
                                args[i] = i >= parameterNumber ? methodInfo.DefaultParameter[i - requiredNumber] : Util.ChangeType(m_Script, parameters[i], methodInfo.ParameterType[i]);
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
                                args[i] = i >= parameterNumber ? methodInfo.DefaultParameter[i - requiredNumber] : Util.ChangeType(m_Script, parameters[i], methodInfo.ParameterType[i]);
                            return methodInfo.Invoke(obj, m_Type);
                        }
                    }
                }
            } catch (System.Exception e) {
                throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 调用函数出错 [" + MethodName + "] : " + e.ToString());
            }
            throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 找不到合适的函数 [" + MethodName + "]");
        }
        public UserdataMethod MakeGenericMethod(Type[] parameters) {
            if (m_GenericCount > 0 && m_GenericMethods != null) {
                List<MethodInfo> methods = new List<MethodInfo>();
                for (int i = 0; i < m_GenericCount; ++i) {
                    FunctionMethod method = m_GenericMethods[i] as FunctionMethod;
                    if (method != null) {
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
                if (methods.Count > 0)
                    return new UserdataMethod(m_Script, m_Type, MethodName, methods);
            }
            throw new ExecutionException(m_Script, "没有找到合适的泛型函数 " + MethodName);
        }
    }
    public class ReflectUserdataMethod : UserdataMethod {
        public ReflectUserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods) {
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
