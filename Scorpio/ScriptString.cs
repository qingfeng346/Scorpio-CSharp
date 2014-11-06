using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;

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
        public override ScriptObject Clone()
        {
            return Script.CreateString(Value);
        }
    }
}
