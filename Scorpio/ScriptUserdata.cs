using System;
namespace Scorpio
{
    //语言数据
    public abstract class ScriptUserdata : ScriptObject
    {
        protected object m_Value;
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public override ObjectType Type { get { return ObjectType.UserData; } }
        public object Value { get { return m_Value; } }
        public Type ValueType { get; protected set; }
        public ScriptUserdata(Script script) : base(script) { }
    }
}
