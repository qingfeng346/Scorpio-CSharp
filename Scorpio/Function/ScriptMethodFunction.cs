using System;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptMethodFunction : ScriptFunction {
        private ScorpioMethod m_Method;                                         //程序函数
        public ScorpioMethod Method { get { return m_Method; } }                //返回程序函数对象
        public ScriptMethodFunction(Script script, ScorpioMethod method) : this(script, method.MethodName, method) { }
        public ScriptMethodFunction(Script script, String name, ScorpioMethod method) : base(script, name) {
            this.m_Method = method;
        }
        public override object Call(ScriptObject[] parameters) {
            try {
                return m_Method.Call(parameters);
            } catch (System.Exception ex) {
                throw new ExecutionException(m_Script, "CallFunction [" + Name + "] is error : " + ex.ToString());
            }
        }
    }
}
