using Scorpio;
namespace Scorpio.Variable {
    public class ScriptNumberShort : ScriptNumber {
        private short m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeShort; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public short Value { get { return m_Value; } }
        public ScriptNumberShort(Script script, short value) : base(script) {
            m_Value = value;
        }
        public override ScriptObject Clone() {
            return new ScriptNumberShort(m_Script, m_Value);
        }
    }
}
