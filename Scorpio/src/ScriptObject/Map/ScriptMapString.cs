using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;

namespace Scorpio {
    //脚本map类型
    public class ScriptMapString : ScriptMap {
        //数组迭代器
        private struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            readonly IEnumerator<ScorpioKeyValue<string, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapString map) {
                m_Enumerator = map.m_Values.GetEnumerator();
            }
            public bool MoveNext() {
                return m_Enumerator.MoveNext();
            }
            public KeyValuePair<object, ScriptValue> Current => new KeyValuePair<object, ScriptValue>(m_Enumerator.Current.Key, m_Enumerator.Current.Value);
            object IEnumerator.Current => Current;
            public void Reset() { m_Enumerator.Reset(); }
            public void Dispose() { m_Enumerator.Dispose(); }
        }
        public ScriptMapString(Script script) : base(script) { }
        public override void Alloc() {
            SetPrototypeValue(script.TypeMapValue);
        }
        public override void Free() {
            Release();
            Clear();
            m_Script.Free(this);
        }
        public override void gc() {
            Clear();
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
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
            m_Values.Free();
        }
        public override void Remove(object key) {
            if (m_Values.TryGetValue((string)key, out var result)) {
                result.Free();
                m_Values.Remove((string)key);
            }
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
