using System;
using Scorpio.Tools;
using Scorpio.Exception;
using System.Collections.Generic;
namespace Scorpio.Userdata {
    /// <summary> 枚举 Type </summary>
    public class ScriptUserdataEnumType : ScriptUserdata {
        private Dictionary<string, ScriptValue> m_Enums = new Dictionary<string, ScriptValue>();     //所有枚举的值
        public ScriptUserdataEnumType(Type value) {
            this.m_Value = value;
            this.m_ValueType = value;
            var names = Enum.GetNames(value);
            foreach (var name in names) {
                m_Enums[string.Intern(name)] = new ScriptValue(Enum.Parse(m_ValueType, name));
            }
        }
        public override Type ValueType { get { return Util.TYPE_TYPE; } }
        public override string ToString() { return m_ValueType.Name; }
        public override ScriptValue GetValue(string key) {
            if (m_Enums.TryGetValue(key, out var value))
                return value;
            throw new ExecutionException($"枚举[{m_ValueType}]不存在[{key}");
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            if (parameters[0].valueType == ScriptValue.stringValueType) {
                var ignoreCase = length > 1 ? parameters[1].valueType == ScriptValue.trueValueType : false;
                return new ScriptValue(Enum.Parse(m_ValueType, parameters[0].stringValue, ignoreCase));
            } else {
                return new ScriptValue(Enum.ToObject(m_ValueType, parameters[0].ToInt32()));
            }
        }
    }
}
