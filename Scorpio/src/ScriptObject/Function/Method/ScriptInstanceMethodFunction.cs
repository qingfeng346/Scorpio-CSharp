namespace Scorpio.Function {
    public class ScriptInstanceMethodFunction : ScriptMethodFunction {
        public ScriptInstanceMethodFunction(Script script) : base(script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, false, thisObject.GetValue, parameters, length));
        }
        public override void Free() {
            DelRecord();
            MethodName = null;
            Method = null;
            m_Script.Free(this);
        }
        public override string ToString() {
            return $"实例函数 {Method}";
        }
    }
}
