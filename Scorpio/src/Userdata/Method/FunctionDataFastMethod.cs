using System;
namespace Scorpio.Userdata {
    //快速反射函数
    public class FunctionDataFastMethod : FunctionData {
        private ScorpioFastReflectMethod FastMethod;
        private int MethodIndex;                     //函数索引(去反射使用)
        public FunctionDataFastMethod(ScorpioFastReflectMethod method, Type[] parameterType, Type paramType, int methodIndex) :
            base(parameterType, null, new bool[parameterType.Length], parameterType.Length, paramType) {
            FastMethod = method;
            MethodIndex = methodIndex;
        }
        public override object Invoke(object obj) {
            return FastMethod.Call(obj, MethodIndex, Args);
        }
    }
}
