using System;
using System.Reflection;
namespace Scorpio.Userdata {
    //实例化过后的模板函数
    public class UserdataMethodGeneric : UserdataMethodReflect {
        public bool IsStatic { get; private set; }            //是否是静态函数
        public UserdataMethodGeneric(Type type, string methodName, MethodInfo method) :
            base(type, methodName, new MethodInfo[] { method }) {
            IsStatic = method.IsStatic;
        }
    }
}
