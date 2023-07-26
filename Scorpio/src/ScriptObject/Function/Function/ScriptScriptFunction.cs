using Scorpio.Runtime;
namespace Scorpio.Function {
    public class ScriptScriptFunction : ScriptScriptFunctionBase {
        public ScriptScriptFunction(ScriptContext context) : base(context) { }
        public override ScriptFunction SetBindObject(ScriptValue bindObject) {
            return new ScriptScriptBindFunction(m_Context, bindObject);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues);
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(thisObject, parameters, length, m_internalValues, baseType);
        }
        public override string ToString() { return $"Function<{MethodName}>"; }
    }
    public class ScriptScriptBindFunction : ScriptScriptBindFunctionBase {
        public ScriptScriptBindFunction(ScriptContext context, ScriptValue bindObject) : base(context, bindObject) { }
        public override ScriptFunction SetBindObject(ScriptValue bindObject) {
            return new ScriptScriptBindFunction(m_Context, bindObject);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues);
        }
        internal override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length, ScriptType baseType) {
            return m_Context.Execute(m_BindObject, parameters, length, m_internalValues, baseType);
        }
        public override string ToString() { return $"BindFunction<{MethodName}>"; }
    }
}
