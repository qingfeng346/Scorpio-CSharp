using System;
using Scorpio.Tools;
using Scorpio.Exception;
using System.Collections.Generic;
namespace Scorpio.Userdata {
    /// <summary> 枚举 Type </summary>
    public class ScriptUserdataEnumType : ScriptUserdata {
        private bool init = false;
        private Dictionary<string, ScriptValue> m_Enums = new Dictionary<string, ScriptValue>();     //所有枚举的值
        public ScriptUserdataEnumType(Script script, Type value) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
            
        }
        public override void Free() { }
        public override void gc() { }
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override string ToString() { return m_ValueType.Name; }
        public override ScriptValue GetValue(string key) {
            if (!init) {
                init = true;
                foreach (var name in Enum.GetNames(m_ValueType)) {
                    m_Enums[string.Intern(name)] = ScriptValue.CreateValue(script, Enum.Parse(m_ValueType, name));
                }
            }
            if (m_Enums.TryGetValue(key, out var value))
                return value;
            throw new ExecutionException($"枚举[{m_ValueType}]不存在[{key}");
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (parameters[0].valueType == ScriptValue.stringValueType) {
                var ignoreCase = length > 1 ? parameters[1].valueType == ScriptValue.trueValueType : false;
                return ScriptValue.CreateValue(m_Script, Enum.Parse(m_ValueType, parameters[0].stringValue, ignoreCase));
            } else {
                return ScriptValue.CreateValue(m_Script, Enum.ToObject(m_ValueType, parameters[0].ToInt32()));
            }
        }
    }
}
