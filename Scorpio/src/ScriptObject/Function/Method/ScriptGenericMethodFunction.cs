using Scorpio.Userdata;
using System;
namespace Scorpio.Function {
    //生成后的模板实例函数,第一个参数为对象
    public class ScriptGenericMethodFunction : ScriptMethodFunction {
        public ScriptGenericMethodFunction(UserdataMethod method) : base(method) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var thisValue = parameters[0].Value;
            Array.Copy(parameters, 1, parameters, 0, length - 1);
            return ScriptValue.CreateValue(Method.Call(true, thisValue, parameters, length - 1));
        }
    }
}
