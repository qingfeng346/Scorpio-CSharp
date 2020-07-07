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
        public virtual ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues, baseType);
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return new ScriptScriptBindFunction(m_Context, obj);
        }
    }
    public class ScriptScriptBindFunction : ScriptScriptFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptScriptBindFunction(ScriptContext context, ScriptValue bindObject) : base(context) {
            m_BindObject = bindObject;
        }
        public override ScriptValue BindObject { get { return m_BindObject; } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues, baseType);
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptBindFunction>();
            return func != null && (m_Context == func.m_Context && m_BindObject.Equals(func.m_BindObject));
        }
    }
}
