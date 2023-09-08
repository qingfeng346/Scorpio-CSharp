using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    //脚本map类型
    public class ScriptMapStringPolling : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        protected ScorpioStringDictionary<ScriptValue> m_Values = new ScorpioStringDictionary<ScriptValue>(); //所有的数据(函数和数据都在一个数组)
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            private IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapStringPolling map) {
                m_Enumerator = map.m_Values.GetEnumerator();
            }
            public bool MoveNext() {
                return m_Enumerator.MoveNext();
            }
            public KeyValuePair<object, ScriptValue> Current => new KeyValuePair<object, ScriptValue>(m_Enumerator.Current.Key, m_Enumerator.Current.Value);
            object IEnumerator.Current => Current;
            public void Reset() { m_Enumerator.Reset(); }
            public void Dispose() { m_Enumerator.Dispose(); m_Enumerator = null; }
        }
        public ScriptMapStringPolling(Script script) : base(script) { }
        public ScriptMapStringPolling(Script script, int capacity) : base(script) {
            SetCapacity(capacity);
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        #region GetValue SetValue重载
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
        }
        #endregion
        public void SetCapacity(int capacity) {
            m_Values.SetCapacity(capacity);
        }
        public override bool ContainsKey(object key) {
            if (!(key is string)) return false;
            return m_Values.ContainsKey(key as string);
        }
        public override bool ContainsValue(ScriptValue value) {
            return m_Values.ContainsValue(value);
        }
        public override int Count() {
            return m_Values.Count;
        }
        public override void Clear() {
            m_Values.Clear();
        }
        public override void Remove(object key) {
            m_Values.Remove(key as string);
        }
        public override bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public override void DelValue(string key) {
            m_Values.Remove(key);
        }
        public override void ClearVariables() {
            m_Values.Clear();
        }
        public override ScriptArray GetKeys() {
            var ret = new ScriptArray(script);
            foreach (var pair in m_Values) {
                ret.Add(new ScriptValue(pair.Key));
            }
            return ret;
        }
        public override ScriptArray GetValues() {
            var ret = new ScriptArray(script);
            foreach (var pair in m_Values) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public override ScriptMap NewCopy() {
            var ret = new ScriptMapStringPolling(script);
            foreach (var pair in m_Values) {
                ret.m_Values[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapStringPolling(script);
            if (deep) {
                foreach (var pair in m_Values) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Values[pair.Key] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Values[pair.Key] = value;
                        }
                    } else {
                        ret.m_Values[pair.Key] = value;
                    }
                }
            } else {
                foreach (var pair in m_Values) {
                    ret.m_Values[pair.Key] = pair.Value;
                }
            }
            return ret;
        }
    }
}
