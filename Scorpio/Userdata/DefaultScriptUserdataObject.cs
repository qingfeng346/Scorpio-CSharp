using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 普通Object类型 </summary>
    public class DefaultScriptUserdataObject : ScriptUserdata
    {
        private UserdataType m_Type;
        public DefaultScriptUserdataObject(Script script, object value) : base(script)
        {
            this.Value = value;
            this.ValueType = (Value is Type) ? (Type)Value : Value.GetType();
            m_Type = script.GetScorpioType(ValueType);
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            return Script.CreateObject(m_Type.CreateInstance(parameters));
        }
        public override ScriptObject GetValue(string strName)
        {
            return Script.CreateObject(m_Type.GetValue(Value, strName));
        }
        public override void SetValue(string strName, ScriptObject value)
        {
            m_Type.SetValue(Value, strName, value);
        }
    }
}
