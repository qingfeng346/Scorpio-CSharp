using System.Collections;
using System.Collections.Generic;

namespace Scorpio {
    //脚本map类型
    public class ScriptMapString : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        //数组迭代器
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            private IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapString map) {
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
        public ScriptMapString(Script script) : base(script) { }
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
            var ret = m_Script.NewMapString();
            foreach (var pair in m_Values) {
                ret.SetValue(pair.Key, pair.Value);
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = m_Script.NewMapString();
            if (deep) {
                foreach (var pair in m_Values) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Values[pair.Key] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.SetValue(pair.Key, pair.Value);
                        }
                    } else {
                        ret.SetValue(pair.Key, pair.Value);
                    }
                }
            } else {
                foreach (var pair in m_Values) {
                    ret.SetValue(pair.Key, pair.Value);
                }
            }
            return ret;
        }
    }
}
