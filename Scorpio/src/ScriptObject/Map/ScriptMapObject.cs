using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Tools;
namespace Scorpio {
    //脚本map类型
    public class ScriptMapObject : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        private Dictionary<object, ScriptValue> m_Objects = new Dictionary<object, ScriptValue>();  //所有的数据(函数和数据都在一个数组)
        public ScriptMapObject(Script script) : base(script) { }
        internal ScriptMapObject(Script script, ScriptValue[] parameters, int length) : base(script) { }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Objects.GetEnumerator(); }

        public override ScriptValue GetValue(string key) {
            return m_Objects.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override ScriptValue GetValue(object key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public override void SetValue(object key, ScriptValue value) {
            m_Objects[key] = value;
        }

        public override bool HasValue(string key) {
            return m_Objects.ContainsKey(key);
        }
        public override bool ContainsKey(object key) {
            if (key == null) return false;
            return m_Objects.ContainsKey(key);
        }
        public override bool ContainsValue(ScriptValue value) {
            return m_Objects.ContainsValue(value);
        }
        public override int Count() {
            return m_Objects.Count;
        }
        public override void Clear() {
            m_Objects.Clear();
        }
        public override void Remove(object key) {
            m_Objects.Remove(key);
        }
        public override ScriptArray GetKeys() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Objects) {
                ret.Add(ScriptValue.CreateValue(pair.Key));
            }
            return ret;
        }
        public override ScriptArray GetValues() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Objects) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public override ScriptMap NewCopy() {
            var ret = new ScriptMapObject(m_Script);
            foreach (var pair in m_Objects) {
                ret.m_Objects[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapObject(m_Script);
            if (deep) {
                foreach (var pair in m_Objects) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Objects[pair.Key] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Objects[pair.Key] = value;
                        }
                    } else {
                        ret.m_Objects[pair.Key] = value;
                    }
                }
            } else {
                foreach (var pair in m_Objects) {
                    ret.m_Objects[pair.Key] = pair.Value;
                }
            }
            return ret;
        }
        internal override void ToJson(JsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Objects) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(pair.Key.ToString());
                builder.Append(":");
                jsonSerializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
    }
}
