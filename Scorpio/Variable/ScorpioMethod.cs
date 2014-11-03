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
            List<FunctionMethod> functionMethod = new List<FunctionMethod>();
            MethodInfo[] methods = type.GetMethods(Script.BindingFlag);
            int length = methods.Length;
            List<Type> parameters = new List<Type>();
            bool Params = false;
            Type ParamType = null;
            for (int i = 0; i < length;++i ) {
                MethodInfo method = methods[i];
                if (method.Name.Equals(methodName)) {
                    Params = false;
                    ParamType = null;
                    parameters.Clear();
                    var pars = methods[i].GetParameters();
                    foreach (var par in pars) {
                        parameters.Add(par.ParameterType);
                        Params = Util.IsParamArray(par);
                        if (Params) ParamType = par.ParameterType.GetElementType();
                    }
                    functionMethod.Add(new FunctionMethod(method, parameters.ToArray(), ParamType, Params));
                }
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public ScorpioMethod(string typeName, ConstructorInfo[] methods)
        {
            MethodName = typeName;
            List<FunctionMethod> functionMethod = new List<FunctionMethod>();
            int length = methods.Length;
            List<Type> parameters = new List<Type>();
            bool Params = false;
            Type ParamType = null;
            for (int i = 0; i < length; ++i)
            {
                Params = false;
                ParamType = null;
                var method = methods[i];
                parameters.Clear();
                var pars = methods[i].GetParameters();
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
                for (int i = 0; i < m_Methods.Length; ++i) {
                    FunctionMethod method = m_Methods[i];
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
                for (int i = 0; i < m_Methods.Length; ++i) {
                    FunctionMethod method = m_Methods[i];
                    int length = method.ParameterType.Length;
                    if (method.Params && parameters.Length >= length - 1) {
                        bool fit = true;
                        for (int j = 0; j < parameters.Length; ++j) {
                            if (!Util.CanChangeType(parameters[j], j >= length - 1 ? method.ParamType : method.ParameterType[j])) {
                                fit = false;
                                break;
                            }
                        }
                        if (fit) {
                            object[] objs = new object[length];
                            for (int j = 0; j < length - 1; ++j) {
                                objs[j] = Util.ChangeType(parameters[j], method.ParameterType[j]);
                            }
                            List<object> param = new List<object>();
                            for (int j = length - 1; j < parameters.Length; ++j) {
                                param.Add(Util.ChangeType(parameters[j], method.ParamType));
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
