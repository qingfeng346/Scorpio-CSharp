namespace Scorpio.Variable {
    public class ScriptNumberFloat : ScriptNumber {
        private float m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeFloat; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public float Value { get { return m_Value; } }
        public ScriptNumberFloat(Script script, float value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberFloat(m_Script, m_Value);
        }
    }
}
