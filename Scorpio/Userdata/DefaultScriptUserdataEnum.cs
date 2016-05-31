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
            this.m_Value = value;
            this.m_ValueType = value;
            m_Enums = new Dictionary<string, ScriptEnum>();
            //此处获取枚举列表不能使用 Enum.GetValues 此函数在UWP平台下的master模式会报错
            string[] names = Enum.GetNames(ValueType);
            foreach (var name in names) {
                m_Enums[name] = script.CreateEnum(Enum.Parse(ValueType, name));
            }
        }
        public override object Call(ScriptObject[] parameters)
        {
            throw new ExecutionException(m_Script, "枚举类型不支持实例化");
        }
        public override ScriptObject GetValue(object key)
        {
            if (!(key is string))
                throw new ExecutionException(m_Script, "Enum GetValue只支持String类型");
            string name = (string)key;
            if (m_Enums.ContainsKey(name))
                return m_Enums[name];
            throw new ExecutionException(m_Script, "枚举[" + ValueType.ToString() + "] 元素[" + name + "] 不存在");
        }
    }
}
