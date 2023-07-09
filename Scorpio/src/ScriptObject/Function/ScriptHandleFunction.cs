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
            return new ScriptHandleBindFunction(m_Script, m_Handle, obj);
        }
    }
    public class ScriptHandleBindFunction : ScriptHandleFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptHandleBindFunction(Script script, ScorpioHandle handle, ScriptValue bindObject) : base(script, handle) {
            m_BindObject = bindObject;
        }
        public override ScriptValue BindObject { get { return m_BindObject; } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Handle.Call(m_BindObject, parameters, length);
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptHandleBindFunction>();
            return func != null && (m_Handle == func.m_Handle && m_BindObject.Equals(func.m_BindObject));
        }
    }
}
