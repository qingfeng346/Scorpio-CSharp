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
    public class DefaultScriptUserdataObject : ScriptUserdata
    {
        protected UserdataType m_UserdataType;
        public DefaultScriptUserdataObject(Script script, object value, UserdataType type) : base(script)
        {
            this.Value = value;
            this.ValueType = value.GetType();
            this.m_UserdataType = type;
        }
        public override ScriptObject GetValue(object key)
        {
            if (!(key is string)) throw new ExecutionException(Script, "Object GetValue只支持String类型");
            return Script.CreateObject(m_UserdataType.GetValue(Value, (string)key));
        }
        public override void SetValue(object key, ScriptObject value)
        {
            if (!(key is string)) throw new ExecutionException(Script, "Object SetValue只支持String类型");
            m_UserdataType.SetValue(Value, (string)key, value);
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScorpioMethod method = m_UserdataType.GetComputeMethod(type);
            if (method == null) throw new ExecutionException(Script, "找不到运算符重载 " + type);
            return Script.CreateObject (method.Call(new ScriptObject[] { this, obj }));
        }
    }
}
