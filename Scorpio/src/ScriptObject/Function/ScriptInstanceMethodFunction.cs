using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        private object m_Object;
        public ScriptInstanceMethodFunction(Script script) : base(script) { }
        public void Set(object obj, string methodName, UserdataMethod method) {
            m_Object = obj;
            Set(methodName, method);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Method.Call(false, m_Object, parameters, length));
        }
        public override void Free() {
            m_Script.Free(this);
        }
    }
}
