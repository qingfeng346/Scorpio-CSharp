using Scorpio.Userdata;
namespace Scorpio.Function {
    public class ScriptInstanceBindMethodFunction : ScriptMethodFunction {
        private object m_Instance;
        public ScriptInstanceBindMethodFunction(Script script) : base(script) { }
        public ScriptInstanceBindMethodFunction Set(string methodName, UserdataMethod method, object instance) {
            MethodName = methodName;
            Method = method;
            m_Instance = instance;
            return this;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, false, m_Instance, parameters, length));
        }
        public override void Free() {
            DelRecord();
            MethodName = null;
            Method = null;
            m_Instance = null;
            m_Script.Free(this);
        }
        public override string ToString() {
            return $"实例Bind函数 {Method}";
        }
    }
}
