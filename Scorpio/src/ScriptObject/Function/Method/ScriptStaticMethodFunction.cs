using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptStaticMethodFunction : ScriptMethodFunction {
        public ScriptStaticMethodFunction(UserdataMethod method) : base(method) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Method.Call(true, null, parameters, length));
        }
        public override string ToString() {
            return $"静态函数 {Method}";
        }
    }
}
