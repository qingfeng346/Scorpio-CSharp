using Scorpio.Runtime;
namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
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
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            var ret = new ScriptScriptBindFunction(m_Context);
            ret.m_BindObject = obj;
            return ret;
        }
    }
    public class ScriptScriptBindFunction : ScriptScriptFunction {
        public ScriptScriptBindFunction(ScriptContext context) : base(context) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(m_BindObject.valueType == ScriptValue.nullValueType ? thisObject : m_BindObject, parameters, length, m_internalValues);
        }
    }
}
