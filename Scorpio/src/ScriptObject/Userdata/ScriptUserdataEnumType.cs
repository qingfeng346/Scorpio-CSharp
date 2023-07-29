using System;
using Scorpio.Tools;
using Scorpio.Exception;

namespace Scorpio.Userdata {
    /// <summary> 枚举 Type </summary>
    public class ScriptUserdataEnumType : ScriptUserdata {
        private ScorpioStringDictionary<ScriptValue> m_Enums = new ScorpioStringDictionary<ScriptValue>();     //所有枚举的值
        public ScriptUserdataEnumType(Type value) {
            this.m_Value = value;
            this.m_ValueType = value;
            var names = Enum.GetNames(value);
            m_Enums.SetCapacity(names.Length);
            foreach (var name in names) {
                m_Enums[string.Intern(name)] = ScriptValue.CreateValue(Enum.Parse(m_ValueType, name));
            }
        }
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override string ToString() { return m_ValueType.Name; }
        public override ScriptValue GetValue(string key) {
            if (m_Enums.TryGetValue(key, out var value))
                return value;
            throw new ExecutionException($"枚举[{m_ValueType}]不存在[{key}");
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (parameters[0].valueType == ScriptValue.stringValueType) {
                var ignoreCase = length > 1 ? parameters[1].valueType == ScriptValue.trueValueType : false;
                return ScriptValue.CreateValue(Enum.Parse(m_ValueType, parameters[0].stringValue, ignoreCase));
            } else {
                return ScriptValue.CreateValue(Enum.ToObject(m_ValueType, parameters[0].ToInt32()));
            }
        }
    }
}
