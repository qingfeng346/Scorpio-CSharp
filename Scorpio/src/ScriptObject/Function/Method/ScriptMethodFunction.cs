using Scorpio.Userdata;
namespace Scorpio.Function {
    public abstract class ScriptMethodFunction : ScriptObject {
        public UserdataMethod Method;
        public ScriptMethodFunction(UserdataMethod method)
#if SCORPIO_DEBUG
            : base()
#endif
            {
            Method = method;
        }
        public ScriptInstanceBindMethodFunction Bind(ScriptValue scriptValue) {
            return new ScriptInstanceBindMethodFunction(Method, scriptValue.Value);
        }
    }
}