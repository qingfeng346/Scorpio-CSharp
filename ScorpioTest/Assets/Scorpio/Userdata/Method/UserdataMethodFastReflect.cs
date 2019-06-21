using System;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    public class UserdataMethodFastReflect : UserdataMethod {
        public UserdataMethodFastReflect(Type type, string methodName, ScorpioFastReflectMethodInfo[] methods, ScorpioFastReflectMethod fastMethod):
            base(type, methodName) {
            Initialize(methods, fastMethod, methodName);
        }
        void Initialize(ScorpioFastReflectMethodInfo[] methods, ScorpioFastReflectMethod fastMethod, string methodName) {
            var functionMethod = new List<FunctionData>();          //实例函数
            var functionStaticMethod = new List<FunctionData>();    //静态函数
            var genericMethods = new List<FunctionDataGeneric>();   //模板函数
            foreach (var method in methods) {
                var hasRefOut = Array.IndexOf(method.RefOut, true) != -1;
                var functionData = hasRefOut ?
                                    new FunctionDataFastMethodWithRefOut(fastMethod, method.ParameterType, method.RefOut, method.ParamType, method.MethodIndex) :
                                    new FunctionDataFastMethod(fastMethod, method.ParameterType, method.RefOut, method.ParamType, method.MethodIndex);
                (method.IsStatic ? functionStaticMethod : functionMethod).Add(functionData);
            }
            var methodInfos = m_Type.GetTypeInfo().GetMethods(Script.BindingFlag);
            Array.ForEach(methodInfos, (methodInfo) => {
                if (methodInfo.Name.Equals(methodName) && Util.IsGenericMethod(methodInfo)) {
                    genericMethods.Add(new FunctionDataGeneric(methodInfo));
                }
            });
            m_Methods = functionMethod.ToArray();
            m_StaticMethods = functionStaticMethod.ToArray();
            m_GenericMethods = genericMethods.ToArray();
            m_GenericMethodCount = m_GenericMethods.Length;
        }
    }
}
