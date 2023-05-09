using System;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptNamespace : ScriptObject {
        private ScorpioStringDictionary<ScriptValue> m_Objects;
        private string m_Value;
        public override Type ValueType { get { return ScorpioUtil.TYPE_STRING; } } //值类型，如果是Type则返回 typeof(Type)
        public override Type Type { get { return ScorpioUtil.TYPE_STRING; } }      //获取类型
        public override object Value { get { return m_Value; } }            //值
        public ScriptNamespace(string name) : base(ObjectType.Namespace) {
            m_Value = name;
            m_Objects = new ScorpioStringDictionary<ScriptValue>();
        }
        public override ScriptValue GetValue(string key) {
            if (m_Objects.TryGetValue(key, out var value))
                return value;
            var name = $"{m_Value}.{key}";
            var type = ScorpioTypeManager.LoadType(name);
            return m_Objects[name] = type == null ? new ScriptValue(new ScriptNamespace(name)) : ScorpioTypeManager.GetUserdataType(type);
        }
        public override string ToString() {
            return $"Namespace<{m_Value}>";
        }
    }
}
