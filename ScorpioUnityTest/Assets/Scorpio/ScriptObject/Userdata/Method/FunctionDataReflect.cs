using System;
using System.Reflection;
using Scorpio.Tools;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //普通反射函数
    public class FunctionDataReflectMethod : FunctionData {
        public MethodInfo Method;                   //普通函数对象
        public FunctionDataReflectMethod(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Method = method;
        }
        public override bool IsStatic => Method.IsStatic;
        public override object Invoke(object obj, ScriptValue[] parameters) {
            return Method.Invoke(obj, Args);
        }
    }
    //带 ref out 参数的 普通反射函数
    public class FunctionDataReflectMethodWithRefOut : FunctionDataWithRefOut {
        public MethodInfo Method;                   //普通函数对象
        public FunctionDataReflectMethodWithRefOut(MethodInfo method, Type[] parameterType, object[] defaultParameter, bool[] refOut, int requiredNumber, Type paramType) :
            base(parameterType, defaultParameter, refOut, requiredNumber, paramType) {
            this.Method = method;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            var ret = Method.Invoke(obj, Args);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    var instance = parameters[i].Get<ScriptInstance>();
                    if (instance == null) throw new ExecutionException($"带 ref out 标识的字段,必须传入 map, Index : {i}");
                    instance.SetValue(RefOutValue, ScriptValue.CreateValue(Args[i]));
                }
            }
            return ret;
        }
    }
}
