using System.Collections;
using System.Collections.Generic;
using Scorpio.Library;
using Scorpio.Tools;
namespace Scorpio {
    //脚本map类型
    public class ScriptMapPolling : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        //数组迭代器
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            readonly IEnumerator<ScorpioKeyValue<object, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapPolling scriptMap) {
                m_Enumerator = scriptMap.m_Objects.GetEnumerator();
            }
            public bool MoveNext() {
                return m_Enumerator.MoveNext();
            }
            public KeyValuePair<object, ScriptValue> Current => new KeyValuePair<object, ScriptValue>(m_Enumerator.Current.Key, m_Enumerator.Current.Value);
            object IEnumerator.Current => this.Current;
            public void Reset() { m_Enumerator.Reset(); }
            public void Dispose() { m_Enumerator.Dispose(); }
        }

        private ScorpioDictionary<object, ScriptValue> m_Objects;  //所有的数据(函数和数据都在一个数组)
        private bool m_Reference;
        public ScriptMapPolling(Script script, int capacity, bool reference) : base(script) {
            m_Reference = reference;
            if (reference) {
                m_Objects = new ScorpioDictionaryReference<object, ScriptValue>(capacity);
            } else {
                m_Objects = new ScorpioDictionary<object, ScriptValue>(capacity);
            }
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }

        public override ScriptValue GetValue(string key) {
            return m_Objects.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override ScriptValue GetValue(double key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override ScriptValue GetValue(long key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override ScriptValue GetValue(object key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public override void SetValue(double key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public override void SetValue(long key, ScriptValue value) {
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
            var ret = new ScriptMapPolling(m_Script, m_Objects.Count, m_Reference);
            foreach (var pair in m_Objects) {
                ret.m_Objects[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapPolling(m_Script, m_Objects.Count, m_Reference);
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
        internal override void ToJson(ScorpioJsonSerializer jsonSerializer) {
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
