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
        public ScriptString() : base() { }
        public ScriptString(string value) : base(value) { }
        protected override void Initialize_impl()
        {
            
        }
        public override void Assign(ScriptObject obj)
        {
            m_Value = obj.ToString();
        }
        public override ScriptObject Plus(ScriptObject obj)
        {
            m_Value += obj.ToString();
            return this;
        }
    }
}
