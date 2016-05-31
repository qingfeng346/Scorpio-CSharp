using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本字符串类型
    public class ScriptString : ScriptObject
    {
        private string m_Value;
        public ScriptString(Script script, string value) : base(script) {
            this.m_Value = value;
        }
        public string Value { get { return m_Value; } }
        public override ObjectType Type { get { return ObjectType.String; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public override ScriptObject Assign() { return m_Script.CreateString(m_Value); }
        public override ScriptObject GetValue(object index) {
            if (!(index is double || index is int || index is long)) throw new ExecutionException(m_Script, "String GetValue只支持Number类型");
            return m_Script.CreateString(m_Value[Util.ToInt32(index)].ToString());
        }
        public override bool Compare(TokenType type, ScriptObject obj) {
            ScriptString val = obj as ScriptString;
            if (val == null) throw new ExecutionException(m_Script, "字符串比较 右边必须为字符串类型");
            switch (type) {
                case TokenType.Greater:
                    return string.Compare(m_Value, val.m_Value) > 0;
                case TokenType.GreaterOrEqual:
                    return string.Compare(m_Value, val.m_Value) >= 0;
                case TokenType.Less:
                    return string.Compare(m_Value, val.m_Value) < 0;
                case TokenType.LessOrEqual:
                    return string.Compare(m_Value, val.m_Value) <= 0;
                default:
                    throw new ExecutionException(m_Script, "String类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj) {
            if (type == TokenType.AssignPlus) {
                m_Value += obj.ToString();
                return this;
            }
            throw new ExecutionException(m_Script, "String类型 操作符[" + type + "]不支持");
        }
        public override ScriptObject Clone() {
            return m_Script.CreateString(m_Value);
        }
        public override string ToJson() {
            return "\"" + m_Value.Replace("\"", "\\\"") + "\"";
        }
    }
}
