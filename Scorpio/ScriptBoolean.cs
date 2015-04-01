using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;

namespace Scorpio
{
    //脚本bool类型
    public class ScriptBoolean : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Boolean; } }
        public override object ObjectValue { get { return Value; } }
        public bool Value { get; private set; }
        public ScriptBoolean(Script script, bool value) : base(script)
        {
            this.Value = value;
        }
        public ScriptBoolean Inverse()
        {
            return Value ? Script.False : Script.True;
        }
        public override string ToJson()
        {
            return Value ? "true" : "false";
        }
    }
}
