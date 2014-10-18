using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;

namespace Scorpio
{
    //脚本字符串类型
    public class ScriptString : ScriptPrimitiveObject<string>
    {
        public override ObjectType Type { get { return ObjectType.String; } }
        public ScriptString(Script script, string value) : base(script, value) { }
        public override ScriptObject Assign()
        {
            return Script.CreateString(Value);
        }
        public override ScriptObject Clone()
        {
            return Script.CreateString(Value);
        }
    }
}
