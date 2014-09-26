using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    public class DefaultScriptUserdataEnum : ScriptUserdata
    {
        private Dictionary<string, ScriptEnum> m_Enums;                 //如果是枚举的话 所有枚举的值
        public DefaultScriptUserdataEnum(Script script, object value)
        {
            this.m_Script = script;
            this.Value = value;
            this.ValueType = (Value is Type) ? (Type)value : value.GetType();
            m_Enums = new Dictionary<string, ScriptEnum>();
            Array values = Enum.GetValues(ValueType);
            foreach (var v in values) {
                m_Enums[v.ToString()] = script.CreateEnum(v);
            }
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            throw new ScriptException("枚举类型不支持实例化");
        }
        public override ScriptObject GetValue(string strName)
        {
            if (!m_Enums.ContainsKey(strName)) throw new ScriptException("枚举[" + ValueType.ToString() + "] 元素[" + strName + "] 不存在");
            return m_Enums[strName];
        }
        public override void SetValue(string strName, ScriptObject value)
        {
            throw new ScriptException("枚举不支持赋值");
        }
    }
}
