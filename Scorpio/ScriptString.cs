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
        private Script m_Script;
        public ScriptString(Script script, string value) : base(value)
        {
            m_Script = script;
        }
        public override ScriptObject Assign()
        {
            return m_Script.CreateString(Value);
        }
        public override ScriptObject Plus(ScriptObject obj)
        {
            return m_Script.CreateString(Value + obj.ToString());
        }
        public override ScriptObject Clone()
        {
            return m_Script.CreateString(Value);
        }
    }
}
