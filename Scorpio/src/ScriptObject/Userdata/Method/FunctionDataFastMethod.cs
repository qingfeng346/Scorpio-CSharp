using System;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    //快速反射函数
    public class FunctionDataFastMethod : FunctionData {
        protected IScorpioFastReflectMethod FastMethod;
        protected int MethodIndex;                     //函数索引(去反射使用)
        public FunctionDataFastMethod(IScorpioFastReflectMethod method, Type[] parameterType, bool[] refOut, Type paramType, int methodIndex) :
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
        protected IScorpioFastReflectMethod FastMethod;
        protected int MethodIndex;                     //函数索引(去反射使用)
        public FunctionDataFastMethodWithRefOut(IScorpioFastReflectMethod method, Type[] parameterType, bool[] refOut, Type paramType, int methodIndex) :
            base(parameterType, null, refOut, parameterType.Length, paramType) {
            FastMethod = method;
            MethodIndex = methodIndex;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            var ret = FastMethod.Call(obj, MethodIndex, Args);
            for (var i = 0; i < RequiredNumber; ++i) {
                if (RefOuts[i]) {
                    parameters[i].Get<ScriptInstanceBase>().SetValue(RefOutValue, ScriptValue.CreateValue(Args[i]));
                }
            }
            return ret;
        }
    }
}
