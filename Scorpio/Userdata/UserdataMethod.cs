using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio;
using Scorpio.Exception;
using System.Diagnostics;
namespace Scorpio.Userdata
{
    /// <summary> 一个类的同名函数 </summary>
    public class UserdataMethod
    {
        private class FunctionMethod
        {
            private int m_Type;                     //是普通函数还是构造函数
            public ConstructorInfo Constructor;  //构造函数对象
            public MethodInfo Method;              //普通函数对象
            public Type[] ParameterType;            //所有参数类型
            public bool Params;                     //是否是变长参数
            public Type ParamType;                  //变长参数类型
            public object[] Args;                   //参数数组（预创建 可以共用）
            public FunctionMethod(ConstructorInfo Constructor, Type[] ParameterType, Type ParamType, bool Params)
            {
                m_Type = 0;
                this.Constructor = Constructor;
                this.ParameterType = ParameterType;
                this.ParamType = ParamType;
                this.Params = Params;
                this.Args = new object[ParameterType.Length];
            }
            public FunctionMethod(MethodInfo Method, Type[] ParameterType, Type ParamType, bool Params)
            {
                m_Type = 1;
                this.Method = Method;
                this.ParameterType = ParameterType;
                this.ParamType = ParamType;
                this.Params = Params;
                this.Args = new object[ParameterType.Length];
            }
            public object Invoke(object obj, Type type)
            {
                return m_Type == 1 ? Method.Invoke(obj, Args) : Constructor.Invoke(Args);
            }
        }
        private Type m_Type;                            //所在类型
        private int m_Count;                            //相同名字函数数量
        private FunctionMethod[] m_Methods;             //所有函数对象
        public string MethodName { get; private set; }  //函数名字
        public bool IsStatic { get; private set; }      //是否是静态函数
        public UserdataMethod(Type type, string methodName, MethodInfo[] methods)
        {
            List<MethodBase> methodBases = new List<MethodBase>();
            foreach (MethodInfo method in methods)
            {
                if (method.Name.Equals(methodName))
                    methodBases.Add(method);
            }
            IsStatic = methodBases.Count > 0 ? methodBases[0].IsStatic : false;
            Initialize(type, methodName, methodBases);
        }
        public UserdataMethod(Type type, string methodName, ConstructorInfo[] cons)
        {
            IsStatic = false;
            List<MethodBase> methods = new List<MethodBase>();
            methods.AddRange(cons);
            Initialize(type, methodName, methods);
        }
        private UserdataMethod(Type type, string methodName, List<MethodInfo> methods)
        {
            IsStatic = methods[0].IsStatic;
            List<MethodBase> methodBases = new List<MethodBase>();
            methodBases.AddRange(methods.ToArray());
            Initialize(type, methodName, methodBases);
        }
        public UserdataMethod MakeGenericMethod(ScriptObject[] parameters)
        {
            int length = parameters.Length;
            Type[] typeArguments = new Type[length];
            for (int i = 0; i < length; ++i) {
                Util.Assert(parameters[i] is ScriptUserdata);
                typeArguments[i] = (parameters[i] as ScriptUserdata).ValueType;
            }
            List<MethodInfo> methods = new List<MethodInfo>();
            for (int i = 0; i < m_Count; ++i) {
                if (m_Methods[i].Method.IsGenericMethod) {
                    //此处代码 因为暂时没找到获取泛型个数的函数 所以只能用此笨方法 见谅见谅
                    try {
                        methods.Add(m_Methods[i].Method.MakeGenericMethod(typeArguments));
                    } catch (System.Exception ) { }
                }
            }
            if (methods.Count > 0)
                return new UserdataMethod(m_Type, MethodName, methods);
            throw new ExecutionException("没有找到合适的泛型函数 " + MethodName);
        }
        private void Initialize(Type type, string methodName, List<MethodBase> methods)
        {
            m_Type = type;
            MethodName = methodName;
            List<FunctionMethod> functionMethod = new List<FunctionMethod>();
            bool Params = false;
            Type ParamType = null;
            MethodBase method = null;
            List<Type> parameters = new List<Type>();
            int length = methods.Count;
            for (int i = 0; i < length; ++i)
            {
                Params = false;
                ParamType = null;
                parameters.Clear();
                method = methods[i];
                ParameterInfo[] pars = method.GetParameters();
                foreach (ParameterInfo par in pars)
                {
                    parameters.Add(par.ParameterType);
                    Params = Util.IsParamArray(par);
                    if (Params) ParamType = par.ParameterType.GetElementType();
                }
                if (method is MethodInfo)
                    functionMethod.Add(new FunctionMethod(method as MethodInfo, parameters.ToArray(), ParamType, Params));
                else
                    functionMethod.Add(new FunctionMethod(method as ConstructorInfo, parameters.ToArray(), ParamType, Params));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public object Call(object obj, ScriptObject[] parameters)
        {
            if (m_Count == 0) throw new ScriptException("找不到函数 [" + MethodName + "]");
            FunctionMethod methodInfo = null;
            if (m_Count == 1) {
                if (parameters.Length == m_Methods[0].ParameterType.Length)
                    methodInfo = m_Methods[0];
            } else {
                foreach (FunctionMethod method in m_Methods) {
                    if (Util.CanChangeType(parameters, method.ParameterType)) {
                        methodInfo = method;
                        break;
                    }
                }
            }
            if (methodInfo != null) {
                object[] objs = methodInfo.Args;
                int length = methodInfo.ParameterType.Length;
                for (int i = 0; i < length; i++) {
                    objs[i] = Util.ChangeType(parameters[i], methodInfo.ParameterType[i]);
                }
                return methodInfo.Invoke(obj, m_Type);
            } else {
                foreach (FunctionMethod method in m_Methods) {
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
                            {
                                objs[i] = Util.ChangeType(parameters[i], method.ParameterType[i]);
                            }
                            List<object> param = new List<object>();
                            for (int i = length - 1; i < parameters.Length; ++i)
                            {
                                param.Add(Util.ChangeType(parameters[i], method.ParamType));
                            }
                            objs[length - 1] = param.ToArray();
                            return method.Invoke(obj, m_Type);
                        }
                    }
                }
                throw new ScriptException("Type[" + m_Type.ToString() + "] 找不到合适的函数 [" + MethodName + "]");
            }
        }
    }
}
