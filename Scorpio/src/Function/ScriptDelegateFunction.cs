using System;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptDelegateFunction : ScriptFunction {
        private ScorpioFunction m_Function;
        public ScriptDelegateFunction(Script script, ScorpioFunction function) : this(script, function.ToString(), function) { }
        public ScriptDelegateFunction(Script script, String name, ScorpioFunction function) : base(script, name) {
            this.m_Function = function;
        }
        public override object Call(ScriptObject[] parameters) {
            try {
                return m_Function(m_Script, parameters);
            } catch (System.Exception ex) {
                throw new ExecutionException(m_Script, "CallFunction [" + Name + "] is error : " + ex.ToString());
            }
        }
    }
}
