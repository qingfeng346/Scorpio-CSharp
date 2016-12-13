namespace Scorpio.Variable {
    public class ScriptNumberULong : ScriptNumber {
        private ulong m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeULong; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public ulong Value { get { return m_Value; } }
        public ScriptNumberULong(Script script, ulong value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberULong(m_Script, m_Value);
        }
    }
}
