namespace Scorpio.Function {
    using Scorpio.Exception;
    public class ScriptHandleFunction : ScriptFunction {
        protected ScorpioHandle m_Handle;                                         //程序函数执行类
        public ScriptHandleFunction(Script script) : base(script) { }
        public ScriptHandleFunction SetHandle(ScorpioHandle handle) {
            m_Handle = handle;
            return this;
        }
        public override void Free() {
            DelRecord();
            Release();
            m_Script.Free(this);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Handle.Call(thisObject, parameters, length);
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            throw new ExecutionException("Handle函数不支持bind");
            //return new ScriptHandleBindFunction(m_Script, m_Handle, obj);
        }
        public override string ToString() { 
            return $"HandleFunction<{m_Handle.GetType().FullName}>"; 
        }
    }
    //public class ScriptHandleBindFunction : ScriptHandleFunction {
    //    private ScriptValue m_BindObject = ScriptValue.Null;
    //    public ScriptHandleBindFunction(Script script, ScorpioHandle handle, ScriptValue bindObject) : base(script, handle) {
    //        m_BindObject.CopyFrom(bindObject);
    //    }
    //    public override void Free() {
    //        Release();
    //        m_BindObject.Free();
    //    }
    //    public override ScriptValue BindObject => m_BindObject;
    //    public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
    //        return m_Handle.Call(m_BindObject, parameters, length);
    //    }
    //    public override bool Equals(ScriptValue obj) {
    //        var func = obj.Get<ScriptHandleBindFunction>();
    //        return func != null && (m_Handle == func.m_Handle && m_BindObject.Equals(func.m_BindObject));
    //    }
    //}
}
