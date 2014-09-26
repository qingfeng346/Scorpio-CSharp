using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio
{
    //语言数据
    public abstract class ScriptUserdata : ScriptObject
    {
        public override object ObjectValue { get { return Value; } }
        public override ObjectType Type { get { return ObjectType.UserData; } }

        protected Script m_Script;
        public object Value { get; protected set; }
        public Type ValueType { get; protected set; }
        public abstract ScriptObject GetValue(string strName);
        public abstract void SetValue(string strName, ScriptObject value);
        public override string ToString() { return Value.ToString(); }
    }
}
