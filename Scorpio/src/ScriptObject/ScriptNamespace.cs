using System;
using Scorpio.Tools;
using System.Collections.Generic;

namespace Scorpio
{
    public class ScriptNamespace : ScriptObject {
        private Dictionary<string, ScriptValue> m_Objects = new Dictionary<string, ScriptValue>();
        private string m_Value;
        public override Type ValueType => ScorpioUtil.TYPE_STRING;  //值类型，如果是Type则返回 typeof(Type)
        public override Type Type => ScorpioUtil.TYPE_STRING;       //获取类型
        public override object Value => m_Value;                    //值
        public ScriptNamespace(Script script, string name) : base(script, ObjectType.Namespace) {
            m_Value = name;
        }
        public override void Free() {
            m_Objects.Free();
        }
        public override void gc() {
            m_Objects.Free();
        }
        public override ScriptValue GetValue(string key) {
            if (m_Objects.TryGetValue(key, out var value))
                return value;
            var name = $"{m_Value}.{key}";
            var type = m_Script.LoadType(name);
            if (type == null) {
                return m_Objects[name] = new ScriptValue(new ScriptNamespace(m_Script, name));
            } else {
                return m_Objects[name] = new ScriptValue(m_Script.GetUserdataTypeValue(type));
            }
        }
        public override string ToString() {
            return $"Namespace<{m_Value}>";
        }
    }
}
