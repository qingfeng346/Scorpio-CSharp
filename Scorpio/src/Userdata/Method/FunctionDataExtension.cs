using System;
using System.Reflection;

namespace Scorpio.Userdata {
    //类扩展函数
    public class FunctionDataExtension : FunctionData {
        private object[] FinishArgs;
        public FunctionDataExtension(MethodInfo method, Type[] parameterType, object[] defaultParameter, Type paramType) :
            base(parameterType, defaultParameter, paramType) {
            this.IsStatic = false;
            this.FinishArgs = new object[Args.Length + 1];
            this.Method = method;
            this.IsGeneric = method.IsGenericMethod && method.ContainsGenericParameters;
        }
        public override object Invoke(object obj) {
            FinishArgs[0] = obj;
            Array.Copy(Args, 0, FinishArgs, 1, Args.Length);
            return Method.Invoke(null, FinishArgs);
        }
    }
}
