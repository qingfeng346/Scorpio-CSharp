using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScorpioMethod
    {
        private class FunctionMethod
        {
            public bool Params;
            public MethodBase Method;
            public Type[] ParameterType;
            public Type ParamType;
            public FunctionMethod(MethodBase Method, Type[] ParameterType, Type ParamType, bool Params)
            {
                this.Method = Method;
                this.ParameterType = ParameterType;
                this.ParamType = ParamType;
                this.Params = Params;
            }
            public object Invoke(object obj, object[] parameters)
            {
                if (Method is ConstructorInfo)
                    return ((ConstructorInfo)Method).Invoke(parameters);
                return Method.Invoke(obj, parameters);
            }
        }
        private object m_Object;
        private int m_Count;
        private FunctionMethod[] m_Methods;
        public string MethodName { get; private set; }
        public ScorpioMethod(Type type, string methodName) : this(type, methodName, null) { }
        public ScorpioMethod(Type type, string methodName, object obj)
        {
            m_Object = obj;
            MethodName = methodName;
            List<MethodBase> methodBases = new List<MethodBase>();
            MethodInfo[] methods = type.GetMethods(Script.BindingFlag);
            foreach (var method in methods) {
                if (method.Name.Equals(methodName))
                    methodBases.Add(method);
            }
            Initialize(methodBases);
        }
        public ScorpioMethod(string typeName, ConstructorInfo[] cons)
        {
            m_Object = null;
            MethodName = typeName;
            List<MethodBase> methods = new List<MethodBase>();
            methods.AddRange(cons);
            Initialize(methods);
        }
        private void Initialize(List<MethodBase> methods)
        {
            List<FunctionMethod> functionMethod = new List<FunctionMethod>();
            bool Params = false;
            Type ParamType = null;
            MethodBase method = null;
            List<Type> parameters = new List<Type>();
            int length = methods.Count;
            for (int i = 0; i < length; ++i) {
                Params = false;
                ParamType = null;
                parameters.Clear();
                method = methods[i];
                var pars = method.GetParameters();
                foreach (var par in pars) {
                    parameters.Add(par.ParameterType);
                    Params = Util.IsParamArray(par);
                    if (Params) ParamType = par.ParameterType.GetElementType();
                }
                functionMethod.Add(new FunctionMethod(method, parameters.ToArray(), ParamType, Params));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public object Call(ScriptObject[] parameters)
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
                int length = methodInfo.ParameterType.Length;
                object[] objs = new object[length];
                for (int i = 0; i < length; i++) {
                    objs[i] = Util.ChangeType(parameters[i], methodInfo.ParameterType[i]);
                }
                return methodInfo.Invoke(m_Object, objs);
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
                            object[] objs = new object[length];
                            for (int i = 0; i < length - 1; ++i) {
                                objs[i] = Util.ChangeType(parameters[i], method.ParameterType[i]);
                            }
                            List<object> param = new List<object>();
                            for (int i = length - 1; i < parameters.Length; ++i) {
                                param.Add(Util.ChangeType(parameters[i], method.ParamType));
                            }
                            objs[length -1] = param.ToArray();
                            return method.Invoke(m_Object, objs);
                        }
                    }
                }
                throw new ScriptException("找不到合适的函数 [" + MethodName + "]");
            }
        }
    }
}
