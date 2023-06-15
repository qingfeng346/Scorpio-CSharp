using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        private object m_Object;
        public ScriptInstanceMethodFunction(Script script) : base(script) { }
        public ScriptInstanceMethodFunction Set(string methodName, UserdataMethod method, object obj) {
            m_Object = obj;
            Set(methodName, method);
            return this;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, false, m_Object, parameters, length));
        }
        public override void Free() {
            MethodName = null;
            Method = null;
            m_Object = null;
            m_Script.Free(this);
        }
    }
}
