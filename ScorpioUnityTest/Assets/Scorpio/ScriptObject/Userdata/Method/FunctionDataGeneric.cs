using System;
using System.Reflection;

namespace Scorpio.Userdata {
    //无实例的模板函数
    public class FunctionDataGeneric : FunctionData {
        public MethodInfo Method;
        public FunctionDataGeneric(MethodInfo method) : base(EmptyTypes, null, null, 0, null) {
            Method = method;
        }
        public override object Invoke(object obj, ScriptValue[] parameters) {
            throw new NotImplementedException();
        }
    }
}
