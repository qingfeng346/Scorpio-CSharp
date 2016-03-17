using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Variable;
using Scorpio.Exception;
using Scorpio.Userdata;
namespace Scorpio
{
    //语言数据
    public abstract class ScriptUserdata : ScriptObject
    {
        protected UserdataType m_UserdataType;
        public override object ObjectValue { get { return Value; } }
        public override ObjectType Type { get { return ObjectType.UserData; } }
        public virtual UserdataType UserdataType { get { return m_UserdataType; } }
        public object Value { get; protected set; }
        public Type ValueType { get; protected set; }
        public ScriptUserdata(Script script) : base(script) { }
    }
}
