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
            public MethodBase Method;
            public Type[] ParameterType;
            public FunctionMethod(MethodBase Method, Type[] ParameterType)
            {
                this.Method = Method;
                this.ParameterType = ParameterType;
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
            for (int i = 0; i < length;++i ) {
                MethodInfo method = methods[i];
                if (method.Name.Equals(methodName)) {
                    parameters.Clear();
                    var pars = methods[i].GetParameters();
                    foreach (var par in pars) {
                        parameters.Add(par.ParameterType);
                    }
                    functionMethod.Add(new FunctionMethod(method, parameters.ToArray()));
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
            for (int i = 0; i < length; ++i)
            {
                var method = methods[i];
                parameters.Clear();
                var pars = methods[i].GetParameters();
                foreach (var par in pars) {
                    parameters.Add(par.ParameterType);
                }
                functionMethod.Add(new FunctionMethod(method, parameters.ToArray()));
            }
            m_Methods = functionMethod.ToArray();
            m_Count = m_Methods.Length;
        }
        public object Call(ScriptObject[] parameters)
        {
            if (m_Count == 0) throw new ScriptException("Method [" + MethodName + "] is cannot find");
            FunctionMethod methodInfo = null;
            if (m_Count == 1) {
                methodInfo = m_Methods[0];
                if (parameters.Length != methodInfo.ParameterType.Length) throw new ScriptException("Method [" + MethodName + "] is cannot find fit");
            } else {
                for (int i = 0; i < m_Methods.Length; ++i) {
                    FunctionMethod method = m_Methods[i];
                    if (Util.CanChangeType(parameters, method.ParameterType)) {
                        methodInfo = method;
                        break;
                    }
                }
                if (methodInfo == null) throw new ScriptException("Method [" + MethodName + "] is cannot find fit");
            }
            int length = methodInfo.ParameterType.Length;
            object[] objs = new object[length];
            for (int i = 0; i < length; i++) {
                objs[i] = Util.ChangeType(parameters[i], methodInfo.ParameterType[i]);
            }
            return methodInfo.Invoke(m_Object, objs);
        }
    }
}
