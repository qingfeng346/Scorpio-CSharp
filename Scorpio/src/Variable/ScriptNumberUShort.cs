using Scorpio;
namespace Scorpio.Variable {
    public class ScriptNumberUShort : ScriptNumber {
        private ushort m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeUShort; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public ushort Value { get { return m_Value; } }
        public ScriptNumberUShort(Script script, ushort value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberUShort(m_Script, m_Value);
        }
    }
}
