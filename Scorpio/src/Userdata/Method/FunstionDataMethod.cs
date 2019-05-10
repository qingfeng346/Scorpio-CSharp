using System;
using System.Reflection;

namespace Scorpio.Userdata {
    //普通反射函数
    public class FunstionDataMethod : FunctionData {
        public FunstionDataMethod(MethodInfo method, Type[] parameterType, object[] defaultParameter, Type paramType) :
            base(parameterType, defaultParameter, paramType) {
            this.Method = method;
            this.IsStatic = method.IsStatic;
            this.IsGeneric = method.IsGenericMethod && method.ContainsGenericParameters;
        }
        public override object Invoke(object obj) {
            return Method.Invoke(obj, Args);
        }
    }
}
