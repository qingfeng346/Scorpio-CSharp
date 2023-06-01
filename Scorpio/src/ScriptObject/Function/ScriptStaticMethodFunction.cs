using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptStaticMethodFunction : ScriptMethodFunction {
        public ScriptStaticMethodFunction(Script script) : base(script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Method.Call(true, null, parameters, length));
        }
        public override void Free() {
            m_Script.Free(this);
        }
    }
}
