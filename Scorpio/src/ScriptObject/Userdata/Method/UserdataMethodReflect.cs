using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    public class UserdataMethodReflect : UserdataMethod {
        //构造函数
        public UserdataMethodReflect(Type type, string methodName, ConstructorInfo[] methods) :
            base(type, methodName) {
            Initialize(new List<MethodBase>(methods), true);
        }
        //普通函数
        public UserdataMethodReflect(Type type, string methodName, MethodInfo[] methods) :
            base(type, methodName) {
            Initialize(new List<MethodBase>(methods), false);
        }
        //空白扩展函数
        public UserdataMethodReflect(Type type, string methodName) : base(type, methodName) {
            m_Methods = new FunctionData[0];
        }
        protected void Initialize(List<MethodBase> methods, bool isConstructor) {
            var functionMethod = new List<FunctionData>();          //实例函数
            var functionStaticMethod = new List<FunctionData>();    //静态函数
            var genericMethods = new List<FunctionDataGeneric>();   //模板函数
            var parameterTypes = new List<Type>();                  //参数类型
            var defaultParameter = new List<object>();              //默认参数
            var refOut = new List<bool>();                          //是否是 ref out
            var length = methods.Count;                             //函数数量
            var requiredNumber = 0;                                 //必须的参数个数
            var hasRefOut = false;                                  //是否包含 ref out 参数
            Type paramType = null;                                  //变长参数类型
            FunctionData functionData;
            for (int i = 0; i < length; ++i) {
                var method = methods[i];
                if (Util.IsGenericMethod(method)) {
                    genericMethods.Add(new FunctionDataGeneric(method as MethodInfo));
                } else {
                    var parameters = method.GetParameters();            //所有参数
                    ParseParameters(parameters, 0, parameterTypes, defaultParameter, refOut, ref hasRefOut, ref requiredNumber, ref paramType);
                    if (method is ConstructorInfo) {
                        if (hasRefOut) {
                            functionData = new FunctionDataConstructorWithRefOut(method as ConstructorInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
                        } else {
                            functionData = new FunctionDataConstructor(method as ConstructorInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
                        }
                    } else {
                        if (hasRefOut) {
                            functionData = new FunctionDataReflectMethodWithRefOut(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
                        } else {
                            functionData = new FunctionDataReflectMethod(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
                        }
                    }
                    if (functionData.IsStatic)
                        functionStaticMethod.Add(functionData);
                    else
                        functionMethod.Add(functionData);
                }
            }
            if (isConstructor && m_Type.IsValueType) {
                functionMethod.Add(new FunctionDataStructConstructor(m_Type));
            }
            functionMethod.Sort((a, b) => { return a.Priority - b.Priority; });
            functionStaticMethod.Sort((a, b) => { return a.Priority - b.Priority; });
            m_Methods = functionMethod.ToArray();
            m_StaticMethods = functionStaticMethod.ToArray();
            m_GenericMethods = genericMethods.ToArray();
            m_GenericMethodCount = m_GenericMethods.Length;
        }
        //添加一个扩展函数
        public void AddExtensionMethod(MethodInfo method) {
            var length = m_Methods.Length;
            for (var i = 0; i < length; ++i) {
                var methodInfo = m_Methods[i] as FunctionDataExtensionMethod;
                if (methodInfo != null && methodInfo.Method == method) { return; }
            }
            Array.Resize(ref m_Methods, length + 1);
            var parameterTypes = new List<Type>();                  //参数类型
            var defaultParameter = new List<object>();              //默认参数
            var refOut = new List<bool>();                          //是否是 ref out
            var requiredNumber = 0;                                 //必须的参数个数
            var hasRefOut = false;                                  //是否包含 ref out 参数
            Type paramType = null;                                  //变长参数类型
            ParseParameters(method.GetParameters(), 0, parameterTypes, defaultParameter, refOut, ref hasRefOut, ref requiredNumber, ref paramType);
            if (hasRefOut) {
                m_Methods[length] = new FunctionDataExtensionMethodWithRefOut(method, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
            } else {
                m_Methods[length] = new FunctionDataExtensionMethod(method, parameterTypes.ToArray(), defaultParameter.ToArray(), refOut.ToArray(), requiredNumber, paramType);
            }
        }
        void ParseParameters(ParameterInfo[] parameters, int begin, List<Type> parameterTypes, List<object> defaultParameter, List<bool> refOut, ref bool hasRefOut,  ref int requiredNumber, ref Type paramType) {
            var hadDefault = false;
            hasRefOut = false;
            paramType = null;
            parameterTypes.Clear();
            defaultParameter.Clear();
            refOut.Clear();
            for (int i = begin; i < parameters.Length; ++i) {
                var parameter = parameters[i];
                if (Util.IsRetvalOrOut(parameter)) {
                    hasRefOut = true;
                    refOut.Add(true);
                    parameterTypes.Add(parameter.ParameterType.GetElementType());
                } else {
                    refOut.Add(false);
                    parameterTypes.Add(parameter.ParameterType);
                }
                if (parameter.DefaultValue != DBNull.Value) {
                    if (!hadDefault) {
                        hadDefault = true;
                        requiredNumber = i - begin;
                    }
                    defaultParameter.Add(parameter.DefaultValue);
                } else {
                    defaultParameter.Add(null);
                }
                if (Util.IsParams(parameter)) {
                    paramType = parameter.ParameterType.GetElementType();
                    break;
                }
            }
            if (!hadDefault) { requiredNumber = parameterTypes.Count; }
        }
    }
}
