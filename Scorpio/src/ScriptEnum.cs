using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    public class ScriptEnum : ScriptObject
    {
        private object m_Value;
        private Type m_EnumType;
        public override ObjectType Type { get { return ObjectType.Enum; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public Type EnumType { get { return m_EnumType; } }
        public ScriptEnum(Script script, object obj) : base(script)
        {
            m_Value = obj;
            m_EnumType = m_Value.GetType();
        }
    }
}
