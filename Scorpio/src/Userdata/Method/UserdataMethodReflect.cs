using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Commons;
namespace Scorpio.Userdata {
    public class UserdataMethodReflect : UserdataMethod {
        //构造函数
        public UserdataMethodReflect(Script script, Type type, string methodName, ConstructorInfo[] methods) :
            base(script, type, methodName) {
            Initialize(new List<MethodBase>(methods));
        }
        //普通函数
        public UserdataMethodReflect(Script script, Type type, string methodName, MethodInfo[] methods) :
            base(script, type, methodName) {
            IsStructConstructor = type.IsValueType;
            Initialize(new List<MethodBase>(methods));
        }
        protected void Initialize(List<MethodBase> methods) {
            var functionMethod = new List<FunctionData>();
            var functionStaticMethod = new List<FunctionData>();
            var genericMethods = new List<FunctionData>();
            var parameterTypes = new List<Type>();                  //参数类型
            var defaultParameter = new List<object>();              //默认参数
            var length = methods.Count;                             //函数数量
            FunctionData functionData;
            for (int i = 0; i < length; ++i) {
                var method = methods[i];
                var parameters = method.GetParameters();            //所有参数
                parameterTypes.Clear();
                defaultParameter.Clear();
                if (Util.IsExtensionMethod(method)) {
                    //返回不定参类型
                    var paramType = ParseParameters(parameters, 1, parameterTypes, defaultParameter);
                    functionData = new FunctionDataExtension(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), paramType);
                } else {
                    //返回不定参类型
                    var paramType = ParseParameters(parameters, 0, parameterTypes, defaultParameter);
                    if (method is ConstructorInfo) {
                        functionData = new FunctionDataConstructor(method as ConstructorInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), paramType);
                    } else {
                        functionData = new FunstionDataMethod(method as MethodInfo, parameterTypes.ToArray(), defaultParameter.ToArray(), paramType);
                    }
                }
                if (functionData.IsGeneric)
                    genericMethods.Add(functionData);
                else if (functionData.IsStatic)
                    functionStaticMethod.Add(functionData);
                else
                    functionMethod.Add(functionData);
            }
            m_Methods = functionMethod.ToArray();
            m_StaticMethods = functionStaticMethod.ToArray();
            m_GenericMethods = genericMethods.ToArray();
            m_GenericMethodCount = m_GenericMethods.Length;
        }
        Type ParseParameters(ParameterInfo[] parameters, int begin, List<Type> parameterTypes, List<object> defaultParameter) {
            for (int i = begin; i < parameters.Length; ++i) {
                var parameter = parameters[i];
                parameterTypes.Add(parameter.ParameterType);
                if (parameter.DefaultValue != DBNull.Value) { defaultParameter.Add(parameter.DefaultValue); }
                if (Util.IsParams(parameter)) return parameter.ParameterType.GetElementType();
            }
            return null;
        }
    }
}
