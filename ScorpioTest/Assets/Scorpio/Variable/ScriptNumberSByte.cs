using Scorpio;
namespace Scorpio.Variable {
    public class ScriptNumberSByte : ScriptNumber {
        private sbyte m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeSByte; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public sbyte Value { get { return m_Value; } }
        public ScriptNumberSByte(Script script, sbyte value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberSByte(m_Script, m_Value);
        }
    }
}
