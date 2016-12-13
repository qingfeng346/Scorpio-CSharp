namespace Scorpio.Variable {
    public class ScriptNumberByte : ScriptNumber {
        private byte m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeByte; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public byte Value { get { return m_Value; } }
        public ScriptNumberByte(Script script, byte value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberByte(m_Script, m_Value);
        }
    }
}
