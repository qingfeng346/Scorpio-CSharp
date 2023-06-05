using Scorpio.Runtime;
namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptFunction(Script script) : base(script, "") { }
        public ScriptScriptFunction SetContext(ScriptContext context) {
            m_Context = context;
            m_internalValues = new InternalValue[context.internalCount];
            return this;
        }
        public override void Free() {
            Release();
            m_Script.Free(this);
        }
        public void SetInternal(int index, InternalValue value) {
            m_internalValues[index] = value;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues);
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues, baseType);
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewLambdaFunction().SetContext(m_Context, obj);
        }
    }
    public class ScriptScriptLambdaFunction : ScriptScriptFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptScriptLambdaFunction(Script script) : base(script) {
        }
        public ScriptScriptLambdaFunction SetContext(ScriptContext context, ScriptValue bindObject) {
            SetContext(context);
            m_BindObject.CopyFrom(bindObject);
            return this;
        }
        public override void Free() {
            Release();
            m_BindObject.Free();
            m_Script.Free(this);
        }
        public override ScriptValue BindObject => m_BindObject;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues);
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues, baseType);
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptLambdaFunction>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
    }
}
