using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 枚举类型 </summary>
    public class DefaultScriptUserdataEnum : ScriptUserdata
    {
        private Dictionary<string, ScriptEnum> m_Enums;                 //如果是枚举的话 所有枚举的值
        public DefaultScriptUserdataEnum(Script script, Type value) : base(script)
        {
            this.Value = value;
            this.ValueType = value;
            m_Enums = new Dictionary<string, ScriptEnum>();
            Array values = Enum.GetValues(ValueType);
            foreach (var v in values) {
                m_Enums[v.ToString()] = script.CreateEnum(v);
            }
        }
        public override object Call(ScriptObject[] parameters)
        {
            throw new ExecutionException(Script, "枚举类型不支持实例化");
        }
        public override ScriptObject GetValue(object key)
        {
            if (!(key is string))
                throw new ExecutionException(Script, "Enum GetValue只支持String类型");
            string name = (string)key;
            if (m_Enums.ContainsKey(name))
                return m_Enums[name];
            throw new ExecutionException(Script, "枚举[" + ValueType.ToString() + "] 元素[" + name + "] 不存在");
        }
    }
}
