namespace Scorpio.Variable {
    public class ScriptNumberUInt : ScriptNumber {
        private uint m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeUInt; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public uint Value { get { return m_Value; } }
        public ScriptNumberUInt(Script script, uint value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberUInt(m_Script, m_Value);
        }
    }
}
