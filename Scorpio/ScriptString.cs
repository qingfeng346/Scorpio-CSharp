using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本字符串类型
    public class ScriptString : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.String; } }
        public override object ObjectValue { get { return Value; } }
        public string Value { get; set; }
        public ScriptString(Script script, string value) : base(script)
        {
            this.Value = value;
        }
        public override ScriptObject Assign()
        {
            return Script.CreateString(Value);
        }
        public ScriptObject AssignPlus(ScriptObject obj)
        {
            Value += obj.ToString();
            return this;
        }
        public override ScriptObject GetValue(int key)
        {
            return Script.CreateString(Value[key].ToString());
        }
        public bool Compare(TokenType type, ScriptString str)
        {
            switch (type) 
            {
                case TokenType.Greater:
                    return string.Compare(Value, str.Value) < 0;
                case TokenType.GreaterOrEqual:
                    return string.Compare(Value, str.Value) <= 0;
                case TokenType.Less:
                    return string.Compare(Value, str.Value) > 0;
                case TokenType.LessOrEqual:
                    return string.Compare(Value, str.Value) >= 0;
                default:
                    throw new ExecutionException("String类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Clone()
        {
            return Script.CreateString(Value);
        }
        public override string ToJson()
        {
            return "\"" + Value + "\"";
        }
    }
}
