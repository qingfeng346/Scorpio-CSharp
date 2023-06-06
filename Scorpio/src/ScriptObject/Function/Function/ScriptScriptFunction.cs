namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptScriptFunctionBase {
        public ScriptScriptFunction(Script script) : base(script) { }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
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
    public class ScriptScriptLambdaFunction : ScriptScriptBindFunctionBase {
        public ScriptScriptLambdaFunction(Script script) : base(script) {
        }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
        }
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
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewLambdaFunction().SetContext(m_Context, obj);
        }
    }
}
