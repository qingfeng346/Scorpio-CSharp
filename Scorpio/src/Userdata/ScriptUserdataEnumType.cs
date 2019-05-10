using System;
using System.Collections.Generic;
using Scorpio.Commons;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    /// <summary> 枚举 Type </summary>
    public class ScriptUserdataEnumType : ScriptUserdata {
        private ScorpioDictionaryString<ScriptValue> m_Enums = new ScorpioDictionaryString<ScriptValue>();     //所有枚举的值
        public ScriptUserdataEnumType(Script script, Type value) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
            var names = Enum.GetNames(value);
            foreach (var name in names) {
                m_Enums[name] = new ScriptValue(Enum.Parse(m_ValueType, name));
            }
        }
        public override Type ValueType { get { return Util.TYPE_TYPE; } }
        public override string ToString() { return m_ValueType.Name; }
        public override ScriptValue GetValue(string key) {
            if (m_Enums.ContainsKey(key))
                return m_Enums[key];
            throw new ExecutionException("枚举[" + m_ValueType.ToString() + "] 元素[" + key + "] 不存在");
        }
    }
}
