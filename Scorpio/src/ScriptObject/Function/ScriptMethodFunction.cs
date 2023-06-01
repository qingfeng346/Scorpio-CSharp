using Scorpio.Userdata;
namespace Scorpio.Function {
    public abstract class ScriptMethodFunction : ScriptObject {
        public UserdataMethod Method;
        public string MethodName;
        public ScriptMethodFunction(Script script) : base(script, ObjectType.Function) { }
        public void Set(string methodName, UserdataMethod method) {
            MethodName = methodName;
            Method = method;
        }
    }
}