using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
using Scorpio.Compiler;

namespace Scorpio.Userdata
{
    /// <summary> 普通Object类型 </summary>
    public class ScriptUserdataObject : ScriptUserdata
    {
        protected UserdataType m_UserdataType;
        public ScriptUserdataObject(Script script, object value, UserdataType type) : base(script)
        {
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            this.m_UserdataType = type;
        }
        public override ScriptObject GetValue(object key)
        {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "Object GetValue只支持String类型");
            return m_Script.CreateObject(m_UserdataType.GetValue(m_Value, name));
        }
        public override void SetValue(object key, ScriptObject value)
        {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "Object SetValue只支持String类型");
            m_UserdataType.SetValue(m_Value, name, value);
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScorpioMethod method = m_UserdataType.GetComputeMethod(type);
            if (method == null) throw new ExecutionException(m_Script, "找不到运算符重载 " + type);
            return m_Script.CreateObject (method.Call(new ScriptObject[] { this, obj }));
        }
    }
}
