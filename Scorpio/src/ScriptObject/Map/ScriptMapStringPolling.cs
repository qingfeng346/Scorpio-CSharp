using System.Collections;
using System.Collections.Generic;

namespace Scorpio {
    //脚本map类型
    public class ScriptMapStringPolling : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        //数组迭代器
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
        public override void Free() {
            Clear();
            Release();
            m_Script.Free(this);
        }
        public override void gc() {
            Clear();
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
        public override bool ContainsKey(object key) {
            if (!(key is string)) return false;
            return m_Values.ContainsKey((string)key);
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
            if (key is string)
                m_Values.Remove((string)key);
        }
        public override ScriptArray GetKeys() {
            var ret = m_Script.NewArray();
            foreach (var pair in m_Values) {
                ret.AddNoReference(new ScriptValue(pair.Key));
            }
            return ret;
        }
        public override ScriptArray GetValues() {
            var ret = m_Script.NewArray();
            foreach (var pair in m_Values) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public override ScriptMap NewCopy() {
            var ret = m_Script.NewMapStringPolling();
            ret.SetCapacity(m_Values.mSize);
            for (var i = 0; i < m_Values.mSize; ++i) {
                ret.m_Values.mKeys[i] = m_Values.mKeys[i];
                ret.m_Values.mValues[i] = m_Values.mValues[i].Reference();
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = m_Script.NewMapStringPolling();
            ret.SetCapacity(m_Values.mSize);
            if (deep) {
                for (var i = 0; i < m_Values.mSize; ++i) {
                    var value = m_Values.mValues[i];
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Values.mKeys[i] = m_Values.mKeys[i];
                            ret.m_Values.mValues[i] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Values.mKeys[i] = m_Values.mKeys[i];
                            ret.m_Values.mValues[i] = m_Values.mValues[i].Reference();
                        }
                    } else {
                        ret.m_Values.mKeys[i] = m_Values.mKeys[i];
                        ret.m_Values.mValues[i] = m_Values.mValues[i].Reference();
                    }
                }
            } else {
                for (var i = 0; i < m_Values.mSize; ++i) {
                    ret.m_Values.mKeys[i] = m_Values.mKeys[i];
                    ret.m_Values.mValues[i] = m_Values.mValues[i].Reference();
                }
            }
            return ret;
        }
    }
}
