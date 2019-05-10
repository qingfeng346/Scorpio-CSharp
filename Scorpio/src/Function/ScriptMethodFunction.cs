using Scorpio.Userdata;
namespace Scorpio.Function {
    public abstract class ScriptMethodFunction : ScriptFunction {
        public UserdataMethod Method;
        public ScriptMethodFunction(Script script, UserdataMethod method) : base(script, method.MethodName) {
            Method = method;
        }
    }
}