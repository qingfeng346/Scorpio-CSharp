using System.Collections;
using System.Collections.Generic;
using Scorpio.Exception;
namespace Scorpio {
    //脚本map类型
    public class ScriptMapPolling : ScriptMap {
        //数组迭代器
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            readonly IEnumerator<ScorpioKeyValue<object, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapPolling map) {
                m_Enumerator = map.m_Objects.GetEnumerator();
            }
            public bool MoveNext() {
                return m_Enumerator.MoveNext();
            }
            public KeyValuePair<object, ScriptValue> Current => new KeyValuePair<object, ScriptValue>(m_Enumerator.Current.Key, m_Enumerator.Current.Value);
            object IEnumerator.Current => Current;
            public void Reset() { m_Enumerator.Reset(); }
            public void Dispose() { m_Enumerator.Dispose(); }
        }

        private ScorpioObjectDictionary<ScriptValue> m_Objects;  //所有的数据(函数和数据都在一个数组)
        public ScriptMapPolling(Script script, int capacity) : base(script) {
            m_Objects = new ScorpioObjectDictionary<ScriptValue>(capacity);
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() => new Enumerator(this);

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
            throw new ExecutionException("MapPolling 不支持 Remove, 请使用普通 Map");
        }
        public override void Trim() {
            m_Objects.TrimCapacity();
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
            var ret = new ScriptMapPolling(m_Script, m_Objects.Count);
            foreach (var pair in m_Objects) {
                ret.m_Objects[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapPolling(m_Script, m_Objects.Count);
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
    }
}
