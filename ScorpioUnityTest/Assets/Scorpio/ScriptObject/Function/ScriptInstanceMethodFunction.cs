using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        private object m_Object;
        public ScriptInstanceMethodFunction(UserdataMethod method, object obj, string methodName) : base(method, methodName) {
            m_Object = obj;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Method.Call(false, m_Object, parameters, length));
        }
    }
}
