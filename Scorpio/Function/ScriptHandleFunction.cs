using System;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptHandleFunction : ScriptFunction {
        private ScorpioHandle m_Handle;                                         //程序函数执行类
        public ScriptHandleFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().FullName, handle) { }
        public ScriptHandleFunction(Script script, String name, ScorpioHandle handle) : base(script, name) {
            this.m_Handle = handle;
        }
        public override object Call(ScriptObject[] parameters) {
            try {
                return m_Handle.Call(parameters);
            } catch (System.Exception ex) {
                throw new ExecutionException(m_Script, "CallFunction [" + Name + "] is error : " + ex.ToString());
            }
        }
    }
}
