using System;
using System.Reflection;

namespace Scorpio.Userdata {
    //反射构造函数
    public class FunctionDataConstructor : FunctionData {
        public ConstructorInfo Constructor;         //构造函数对象
        public FunctionDataConstructor(ConstructorInfo constructor, Type[] parameterType, object[] defaultParameter, Type paramType) :
            base(parameterType, defaultParameter, paramType) {
            this.IsGeneric = false;
            this.IsStatic = false;
            this.Constructor = constructor;
        }
        public override object Invoke(object obj) {
            return Constructor.Invoke(Args);
        }
    }
}
