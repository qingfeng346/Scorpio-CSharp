#if SCORPIO_UWP && !UNITY_EDITOR
#define UWP
#endif
using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 一个类的同名函数 </summary>
    public class UserdataMethod {
        private abstract class FunctionBase {
            public Type[] ParameterType;                //所有参数类型
            public bool Params;                         //是否是变长参数
            public Type ParamType;                      //变长参数类型
            public string ParameterTypes;               //传递参数的类型
            public object[] Args;                       //参数数组（预创建 可以共用）
            public bool IsValid { get; protected set; }   //是否是有效的函数 (模版函数没有声明的时候就是无效的)
            public FunctionBase(Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) {
                this.ParameterType = ParameterType;
                this.ParamType = ParamType;
                this.Params = Params;
                this.ParameterTypes = ParameterTypes;
                this.Args = new object[ParameterType.Length];
            }
            public abstract object Invoke(object obj, Type type);
        }
        private class FunctionMethod : FunctionBase {
            public MethodInfo Method;                   //普通函数对象
            public FunctionMethod(MethodInfo Method, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) : 
                base(ParameterType, ParamType, Params, ParameterTypes) {
                this.Method = Method;
                IsValid = !Method.IsGenericMethod || !Method.ContainsGenericParameters;
            }
            public override object Invoke(object obj, Type type) {
                return Method.Invoke(obj, Args);
            }
        }
        private class FunctionConstructor : FunctionBase {
            public ConstructorInfo Constructor;         //构造函数对象
            public FunctionConstructor(ConstructorInfo Constructor, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) :
                base(ParameterType, ParamType, Params, ParameterTypes) {
                this.IsValid = true;
                this.Constructor = Constructor;
            }
            public override object Invoke(object obj, Type type) {
                return Constructor.Invoke(obj, Args);
            }
        }
        private class FunctionFastMethod : FunctionBase {
            public IScorpioFastReflectMethod Method;
            public FunctionFastMethod(IScorpioFastReflectMethod Method, Type[] ParameterType, Type ParamType, bool Params, string ParameterTypes) :
                base(ParameterType, ParamType, Params, ParameterTypes) {
                this.IsValid = true;
                this.Method = Method;
            }
            public override object Invoke(object obj, Type type) {
                return Method.Call(obj, ParameterTypes, Args);
            }
        }
        private Script m_Script;                        //所在脚本引擎
        private Type m_Type;                            //所在类型
        private int m_Count;                            //相同名字函数数量
        private FunctionBase[] m_Methods;               //所有函数对象
        public string MethodName { get; private set; }  //函数名字
        public bool IsStatic { get; private set; }      //是否是静态函数
        public UserdataMethod() { }
        private UserdataMethod(Script script, Type type, string methodName, List<MethodInfo> methods) {
            m_Script = script;
            IsStatic = methods[0].IsStatic;
            List<MethodBase> methodBases = new List<MethodBase>();
            methodBases.AddRange(methods.ToArray());
            Initialize_impl(type, methodName, methodBases, null);
        }
        protected void Initialize(Script script, Type type, string methodName, MethodInfo[] methods) {
            m_Script = script;
            List<MethodBase> methodBases = new List<MethodBase>();
            foreach (MethodInfo method in methods) {
                if (method.Name.Equals(methodName))
                    methodBases.Add(method);
            }
            IsStatic = methodBases.Count > 0 ? methodBases[0].IsStatic : false;
            Initialize_impl(type, methodName, methodBases, null);
        }
        protected void Initialize(Script script, Type type, string methodName, ConstructorInfo[] cons) {
            m_Script = script;
            IsStatic = false;
            List<MethodBase> methods = new List<MethodBase>();
            methods.AddRange(cons);
            Initialize_impl(type, methodName, methods, null);
        }
        protected void Initialize(int t, bool isStatic, Script script, Type type, string methodName, MethodInfo[] methods, IScorpioFastReflectMethod fastMethod) {
            m_Script = script;
            IsStatic = isStatic;
            List<MethodBase> methodBases = new List<MethodBase>();
            if (t == 0) {
                methodBases.AddRange(type.GetConstructors());
            } else {
                foreach (MethodInfo method in methods) {
                    if (method.Name.Equals(methodName))
                        methodBases.Add(method);
                }
            }
            Initialize_impl(type, methodName, methodBases, fastMethod);
        }
        private void Initialize_impl(Type type, string methodName, List<MethodBase> methods, IScorpioFastReflectMethod fastMethod)
        {
            m_Type = type;
            MethodName = methodName;
            List<FunctionBase> functionMethod = new List<FunctionBase>();
            bool Params = false;
            Type ParamType = null;
            string ParameterTypes = null;
            MethodBase method = null;
            List<Type> parameters = new List<Type>();
            int length = methods.Count;
            for (int i = 0; i < length; ++i) {
                Params = false;
                ParamType = null;
                ParameterTypes = "";
                parameters.Clear();
                method = methods[i];
                ParameterInfo[] pars = method.GetParameters();
                foreach (ParameterInfo par in pars) {
                    ParameterTypes += (par.ParameterType.FullName + "+");
                    parameters.Add(par.ParameterType);
                    Params = Util.IsParamArray(par);
                    if (Params) ParamType = par.ParameterType.GetElementType();
                }
                if (fastMethod != null && !method.IsGenericMethod)
                    functionMethod.Add(new FunctionFastMethod(fastMethod, parameters.ToArray(), ParamType, Params, ParameterTypes));
                else if (method is MethodInfo)
                    functionMethod.Add(new FunctionMethod(method as MethodInfo, parameters.ToArray(), ParamType, Params, ParameterTypes));
                else
                    functionMethod.Add(new FunctionConstructor(method as ConstructorInfo, parameters.ToArray(), ParamType, Params, ParameterTypes));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public object Call(object obj, ScriptObject[] parameters)
        {
            if (m_Count == 0) throw new ExecutionException(m_Script, "找不到函数 [" + MethodName + "]");
            FunctionBase methodInfo = null;
            if (m_Count == 1) {
                methodInfo = m_Methods[0];
                if (!methodInfo.IsValid) throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 找不到合适的函数 [" + MethodName + "]");
            } else {
                foreach (FunctionBase method in m_Methods) {
                    if (method.IsValid && Util.CanChangeType(parameters, method.ParameterType)) {
                        methodInfo = method;
                        break;
                    }
                }
            }
            try {
                if (methodInfo != null && !methodInfo.Params) {
                    int length = methodInfo.ParameterType.Length;
                    object[] objs = methodInfo.Args;
                    for (int i = 0; i < length; i++)
                        objs[i] = Util.ChangeType(m_Script, parameters[i], methodInfo.ParameterType[i]);
                    return methodInfo.Invoke(obj, m_Type);
                } else {
                    foreach (FunctionBase method in m_Methods) {
                        int length = method.ParameterType.Length;
                        if (method.Params && parameters.Length >= length - 1) {
                            bool fit = true;
                            for (int i = 0; i < parameters.Length; ++i) {
                                if (!Util.CanChangeType(parameters[i], i >= length - 1 ? method.ParamType : method.ParameterType[i])) {
                                    fit = false;
                                    break;
                                }
                            }
                            if (fit) {
                                object[] objs = method.Args;
                                for (int i = 0; i < length - 1; ++i)
                                    objs[i] = Util.ChangeType(m_Script, parameters[i], method.ParameterType[i]);
                                Array array = Array.CreateInstance(method.ParamType, parameters.Length - length + 1);
                                for (int i = length - 1; i < parameters.Length; ++i)
                                    array.SetValue(Util.ChangeType(m_Script, parameters[i], method.ParamType), i - length + 1);
                                objs[length - 1] = array;
                                return method.Invoke(obj, m_Type);
                            }
                        }
                    }
                }
            } catch (System.Exception e) {
                throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 调用函数出错 [" + MethodName + "] : " + e.ToString());
            }
            throw new ExecutionException(m_Script, "Type[" + m_Type.ToString() + "] 找不到合适的函数 [" + MethodName + "]");
        }
        public UserdataMethod MakeGenericMethod(Type[] parameters)
        {
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
#if UWP
                            if (!types[j].GetTypeInfo().BaseType.IsAssignableFrom(parameters[j])) {
                                accord = false;
                                break;
                            }
#else
                                if (!types[j].BaseType.IsAssignableFrom(parameters[j])) {
                                    accord = false;
                                    break;
                                }
#endif
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
        public FastReflectUserdataMethod(int t, bool isStatic, Script script, Type type, string methodName, MethodInfo[] methods, IScorpioFastReflectMethod fastMethod) {
            this.Initialize(t, isStatic, script, type, methodName, methods, fastMethod);
        }
    }
}
