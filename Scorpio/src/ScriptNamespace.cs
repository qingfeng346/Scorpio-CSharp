using System;
using Scorpio.Tools;
using Scorpio.Userdata;
namespace Scorpio {
    public class ScriptNamespace : ScriptObject {
        private ScorpioDictionaryString<ScriptValue> m_Objects = new ScorpioDictionaryString<ScriptValue>();
        private string m_Value;
        public override Type ValueType { get { return Util.TYPE_STRING; } } //值类型，如果是Type则返回 typeof(Type)
        public override Type Type { get { return Util.TYPE_STRING; } }      //获取类型
        public override object Value { get { return m_Value; } }            //值
        public ScriptNamespace(string name) : base(ObjectType.Namespace) {
            m_Value = name;
        }
        public override ScriptValue GetValue(string key) {
            var name = $"{m_Value}.{key}";
            ScriptValue value;
            if (m_Objects.TryGetValue(key, out value))
                return value;
            var type = TypeManager.LoadType(name);
            return m_Objects[name] = type == null ? new ScriptValue(new ScriptNamespace(name)) : TypeManager.GetUserdataType(type);
        }
        public override string ToString() {
            return $"Namespace<{m_Value}>";
        }
    }
}
