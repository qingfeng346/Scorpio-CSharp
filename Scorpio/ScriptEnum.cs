using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    public class ScriptEnum : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Enum; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public Type EnumType { get; private set; }
        public object m_Value;
        public ScriptEnum(Script script, object obj) : base(script)
        {
            m_Value = obj;
            EnumType = m_Value.GetType();
        }
    }
}
