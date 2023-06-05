namespace Scorpio.Function {
    public class ScriptStaticMethodFunction : ScriptMethodFunction {
        public ScriptStaticMethodFunction(Script script) : base(script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, true, null, parameters, length));
        }
        public override void Free() {
            m_Script.Free(this);
        }
    }
}
