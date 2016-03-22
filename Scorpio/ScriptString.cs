using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本字符串类型
    public class ScriptString : ScriptObject
    {
        public ScriptString(Script script, string value) : base(script) {
            this.Value = value;
        }
        public string Value { get; set; }
        public override ObjectType Type { get { return ObjectType.String; } }
        public override object ObjectValue { get { return Value; } }
        public override ScriptObject Assign() { return Script.CreateString(Value); }
        public override ScriptObject GetValue(object index) {
            if (!(index is double || index is int || index is long)) throw new ExecutionException(Script, "String GetValue只支持Number类型");
            return Script.CreateString(Value[Util.ToInt32(index)].ToString());
        }
        public override bool Compare(TokenType type, ScriptObject obj) {
            ScriptString val = obj as ScriptString;
            if (val == null) throw new ExecutionException(Script, "字符串比较 右边必须为字符串类型");
            switch (type) {
                case TokenType.Greater:
                    return string.Compare(Value, val.Value) < 0;
                case TokenType.GreaterOrEqual:
                    return string.Compare(Value, val.Value) <= 0;
                case TokenType.Less:
                    return string.Compare(Value, val.Value) > 0;
                case TokenType.LessOrEqual:
                    return string.Compare(Value, val.Value) >= 0;
                default:
                    throw new ExecutionException(Script, "String类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj) {
            if (type == TokenType.AssignPlus) {
                Value += obj.ToString();
                return this;
            }
            throw new ExecutionException(Script, "String类型 操作符[" + type + "]不支持");
        }
        public override ScriptObject Clone() {
            return Script.CreateString(Value);
        }
        public override string ToJson() {
            return "\"" + Value + "\"";
        }
    }
}
