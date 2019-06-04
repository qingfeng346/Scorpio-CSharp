using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    public class UserdataMethodGeneric : UserdataMethodReflect {
        public bool IsStatic { get; private set; }            //是否是静态函数，只有UserdataMethodGeneric类此值有效 
        public UserdataMethodGeneric(Type type, string methodName, MethodInfo method) :
            base(type, methodName, new MethodInfo[] { method }) {
            IsStatic = method.IsStatic;
        }
    }
}
