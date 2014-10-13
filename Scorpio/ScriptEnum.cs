using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    public class ScriptEnum : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Enum; } }
        public override object ObjectValue { get { return m_Object; } }
        public Type EnumType { get; private set; }
        public object m_Object;
        public ScriptEnum(Script script, object obj) : base(script)
        {
            m_Object = obj;
            EnumType = m_Object.GetType();
        }
    }
}
