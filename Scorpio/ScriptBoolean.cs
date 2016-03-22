namespace Scorpio
{
    //脚本bool类型
    public class ScriptBoolean : ScriptObject
    {
        public ScriptBoolean(Script script, bool value) : base(script) {
            this.Value = value;
        }
        public bool Value { get; private set; }
        public override ObjectType Type { get { return ObjectType.Boolean; } }
        public override object ObjectValue { get { return Value; } }
        public override bool LogicOperation() {
            return Value;
        }
        public override string ToJson() {
            return Value ? "true" : "false";
        }
        public ScriptBoolean Inverse()
        {
            return Value ? Script.False : Script.True;
        }
    }
}
