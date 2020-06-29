using System;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptHandleFunction : ScriptFunction {
        protected ScorpioHandle m_Handle;                                         //程序函数执行类
        public ScriptHandleFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().FullName, handle) { }
        public ScriptHandleFunction(Script script, string name, ScorpioHandle handle) : base(script, name) {
            m_Handle = handle;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            try {
                return m_Handle.Call(thisObject, parameters, length);
            } catch (System.Exception ex) {
                throw new ExecutionException("CallFunction [" + FunctionName + "] is error : " + ex.ToString());
            }
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return new ScriptHandleBindFunction(m_Script, FunctionName, m_Handle, obj);
        }
    }
    public class ScriptHandleBindFunction : ScriptHandleFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptHandleBindFunction(Script script, string name, ScorpioHandle handle, ScriptValue bindObject) : base(script, name, handle) {
            m_BindObject = bindObject;
        }
        public override ScriptValue BindObject { get { return m_BindObject; } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            try {
                return m_Handle.Call(m_BindObject, parameters, length);
            } catch (System.Exception ex) {
                throw new ExecutionException("CallFunction [" + FunctionName + "] is error : " + ex.ToString());
            }
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptHandleBindFunction>();
            return func != null && (m_Handle == func.m_Handle && m_BindObject.Equals(func.m_BindObject));
        }
    }
}
