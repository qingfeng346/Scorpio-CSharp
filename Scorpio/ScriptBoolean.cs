namespace Scorpio
{
    //脚本bool类型
    public class ScriptBoolean : ScriptObject
    {
        private bool m_Value;
        public ScriptBoolean(Script script, bool value) : base(script) {
            this.m_Value = value;
        }
        public bool Value { get { return m_Value; } }
        public override ObjectType Type { get { return ObjectType.Boolean; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public override bool LogicOperation() {
            return m_Value;
        }
        public override string ToJson() {
            return m_Value ? "true" : "false";
        }
        public override string ToString() {
            return m_Value ? "true" : "false";
        }
        public ScriptBoolean Inverse()
        {
            return m_Value ? m_Script.False : m_Script.True;
        }
    }
}
