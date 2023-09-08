using System;
using Scorpio.Tools;
using System.Collections.Generic;
namespace Scorpio {
    public class ScriptNamespace : ScriptObject {
        private Dictionary<string, ScriptValue> m_Objects = new Dictionary<string, ScriptValue>();
        private string m_Value;
        public override Type ValueType { get { return ScorpioUtil.TYPE_STRING; } } //值类型，如果是Type则返回 typeof(Type)
        public override Type Type { get { return ScorpioUtil.TYPE_STRING; } }      //获取类型
        public override object Value { get { return m_Value; } }            //值
        public ScriptNamespace(string name) {
            m_Value = name;
        }
        public override ScriptValue GetValue(string key) {
            if (m_Objects.TryGetValue(key, out var value))
                return value;
            var name = $"{m_Value}.{key}";
            var type = ScorpioTypeManager.LoadType(name);
            return m_Objects[key] = type == null ? new ScriptValue(new ScriptNamespace(name)) : ScorpioTypeManager.GetUserdataType(type);
        }
        public override string ToString() {
            return $"Namespace<{m_Value}>";
        }
    }
}
