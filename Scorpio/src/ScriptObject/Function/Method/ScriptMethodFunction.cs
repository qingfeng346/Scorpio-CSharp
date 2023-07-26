using Scorpio.Userdata;
namespace Scorpio.Function {
    public abstract class ScriptMethodFunction : ScriptObject {
        public UserdataMethod Method;
        public string MethodName;
        public ScriptMethodFunction(Script script) : base(script, ObjectType.Function) { }
        public ScriptMethodFunction Set(string methodName, UserdataMethod method) {
            MethodName = methodName;
            Method = method;
            return this;
        }
        public ScriptInstanceBindMethodFunction Bind(ScriptValue scriptValue) {
            return m_Script.NewInstanceBindMethod().Set(MethodName, Method, scriptValue.GetValue);
        }
        public override void gc() { }
    }
}