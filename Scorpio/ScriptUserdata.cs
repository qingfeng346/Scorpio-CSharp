using System;
namespace Scorpio
{
    //语言数据
    public abstract class ScriptUserdata : ScriptObject
    {
        public override object ObjectValue { get { return Value; } }
        public override ObjectType Type { get { return ObjectType.UserData; } }
        public object Value { get; protected set; }
        public Type ValueType { get; protected set; }
        public ScriptUserdata(Script script) : base(script) { }
    }
}
