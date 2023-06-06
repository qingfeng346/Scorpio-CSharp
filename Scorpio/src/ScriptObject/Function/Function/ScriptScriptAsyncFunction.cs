using Scorpio.Runtime;
using Scorpio.Tools;
namespace Scorpio.Function {
    public class ScriptScriptAsyncFunction : ScriptScriptFunctionBase {
        public ScriptScriptAsyncFunction(Script script) : base(script) { }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
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
    public class ScriptScriptAsyncLambdaFunction : ScriptScriptBindFunctionBase {
        public ScriptScriptAsyncLambdaFunction(Script script) : base(script) {
        }
        public override void Free() {
            base.Free();
            m_Script.Free(this);
        }
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
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            return m_Script.NewAsyncLambdaFunction().SetContext(m_Context, obj);
        }
    }
}
