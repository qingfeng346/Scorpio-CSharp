using System;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //快速反射函数
    public class FunctionDataFastMethod : FunctionData {
        protected ScorpioFastReflectMethod FastMethod;
        protected int MethodIndex;                     //函数索引(去反射使用)
        public FunctionDataFastMethod(ScorpioFastReflectMethod method, Type[] parameterType, bool[] refOut, Type paramType, int methodIndex) :
            base(parameterType, null, refOut, parameterType.Length, paramType) {
            FastMethod = method;
            MethodIndex = methodIndex;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            return FastMethod.Call(obj, MethodIndex, Args);
        }
    }
    //带ref out 的快速反射函数
    public class FunctionDataFastMethodWithRefOut : FunctionDataWithRefOut {
        protected ScorpioFastReflectMethod FastMethod;
        protected int MethodIndex;                     //函数索引(去反射使用)
        public FunctionDataFastMethodWithRefOut(ScorpioFastReflectMethod method, Type[] parameterType, bool[] refOut, Type paramType, int methodIndex) :
            base(parameterType, null, refOut, parameterType.Length, paramType) {
            FastMethod = method;
            MethodIndex = methodIndex;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            var ret = FastMethod.Call(obj, MethodIndex, Args);
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
