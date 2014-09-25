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
        protected Script Script { get; set; }
        public abstract object Value { get; protected set; }
        public override object ObjectValue { get { return Value; } }
        public abstract Type ValueType { get; protected set; }
        public abstract ScriptObject GetValue(string strName);
        public abstract void SetValue(string strName, ScriptObject value);
        public override ObjectType Type { get { return ObjectType.UserData; } }
    }
}
