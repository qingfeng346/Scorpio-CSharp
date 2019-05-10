using Scorpio.Runtime;
namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptFunction {
        private ScriptContext m_Context;
        private InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptFunction(ScriptContext context) : base(context.m_script, "") {
            m_Context = context;
            m_internalValues = new InternalValue[context.internalCount];
        }
        public void SetInternal(int index, InternalValue value) {
            m_internalValues[index] = value;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues);
        }
        public override ScriptObject Clone() {
            return new ScriptScriptFunction(m_Context);
        }
    }
}
