using System;
using System.Collections.Generic;

namespace Scorpio.Userdata {
    public class UserdataMethodFastReflect : UserdataMethod {
        public UserdataMethodFastReflect(Script script, Type type, string methodName, ScorpioFastReflectMethodInfo[] methods, ScorpioFastReflectMethod fastMethod):
            base(type, methodName) {
            Initialize(methods, fastMethod);
        }
        void Initialize(ScorpioFastReflectMethodInfo[] methods, ScorpioFastReflectMethod fastMethod) {
            var functionMethod = new List<FunctionData>();
            var functionStaticMethod = new List<FunctionData>();
            foreach (var method in methods) {
                if (method.IsStatic) {
                    functionStaticMethod.Add(new FunctionDataFastMethod(fastMethod, method.ParameterType, method.ParamType, method.MethodIndex));
                } else {
                    functionMethod.Add(new FunctionDataFastMethod(fastMethod, method.ParameterType, method.ParamType, method.MethodIndex));
                }
            }
            m_Methods = functionMethod.ToArray();
            m_StaticMethods = functionStaticMethod.ToArray();
        }
    }
}
