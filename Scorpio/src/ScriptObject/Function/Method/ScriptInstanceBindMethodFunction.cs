using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceBindMethodFunction : ScriptMethodFunction {
        private object m_Instance;
        public ScriptInstanceBindMethodFunction(UserdataMethod method, object instance) : base(method) {
            m_Instance = instance;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(Method.Call(false, m_Instance, parameters, length));
        }
        public override string ToString() {
            return $"实例Bind函数 {Method}";
        }
    }
}
