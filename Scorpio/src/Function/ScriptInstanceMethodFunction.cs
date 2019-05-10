using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        private object m_Object;
        public ScriptInstanceMethodFunction(Script script, UserdataMethod method, object obj):base(script, method) {
            m_Object = obj;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Script.CreateObject(Method.Call(false, m_Object, parameters, length));
        }
    }
}
