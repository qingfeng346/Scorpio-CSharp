using Scorpio.Runtime;
using Scorpio.Tools;
namespace Scorpio.Function {
    public class ScriptScriptAsyncFunction : ScriptFunction {
        protected ScriptContext m_Context;
        protected InternalValue[] m_internalValues;               //父级内部变量
        public ScriptScriptAsyncFunction(Script script) : base(script, "") { }
        public ScriptScriptAsyncFunction SetContext(ScriptContext context) {
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
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Context.ExecuteCoroutine(thisObject, parameters.CloneParameters(length), length, m_internalValues, baseType));
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewAsyncLambdaFunction().SetContext(m_Context, obj);
        }
    }
    public class ScriptScriptAsyncLambdaFunction : ScriptScriptAsyncFunction {
        private ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptScriptAsyncLambdaFunction(Script script) : base(script) {
        }
        public ScriptScriptAsyncLambdaFunction SetContext(ScriptContext context, ScriptValue bindObject) {
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
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues)));
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Script.StartCoroutine(m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues, baseType)));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues));
        }
        internal override ScriptValue CallAsync(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return ScriptValue.CreateValue(m_Script, m_Context.ExecuteCoroutine(m_BindObject, parameters.CloneParameters(length), length, m_internalValues, baseType));
        }
        public override bool Equals(ScriptValue obj) {
            var func = obj.Get<ScriptScriptAsyncLambdaFunction>();
            return func != null && ReferenceEquals(m_Context, func.m_Context) && m_BindObject.Equals(func.m_BindObject);
        }
    }
}
