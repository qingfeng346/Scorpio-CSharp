using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptStaticMethodFunction : ScriptMethodFunction {
        public ScriptStaticMethodFunction(Script script, UserdataMethod method):base(script, method) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Script.CreateObject(Method.Call(true, null, parameters, length));
        }
    }
}
