using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        public ScriptInstanceMethodFunction(UserdataMethod method) : base(method) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Method.Call(false, thisObject.Value, parameters, length));
        }
        public override string ToString() {
            return $"实例函数 {Method}";
        }
    }
}
