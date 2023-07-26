using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptHandleFunction : ScriptFunction {
        protected ScorpioHandle m_Handle;                                         //程序函数执行类
        public ScriptHandleFunction(Script script, ScorpioHandle handle) : base(script) {
            m_Handle = handle;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Handle.Call(thisObject, parameters, length);
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            throw new ExecutionException("Handle函数不支持bind");
        }
        public override string ToString() {
            return $"HandleFunction<{m_Handle.GetType().FullName}>";
        }
    }
}
