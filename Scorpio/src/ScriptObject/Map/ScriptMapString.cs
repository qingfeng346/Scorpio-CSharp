using System.Text;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;

namespace Scorpio {
    //脚本map类型
    public class ScriptMapString : ScriptMap, IEnumerable<KeyValuePair<object, ScriptValue>> {
        //数组迭代器
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
            readonly IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            internal Enumerator(ScriptMapString map) {
                m_Enumerator = map.m_Values.GetEnumerator();
            }
            public bool MoveNext() {
                return m_Enumerator.MoveNext();
            }
            public KeyValuePair<object, ScriptValue> Current { 
                get { 
                    return new KeyValuePair<object, ScriptValue>(m_Enumerator.Current.Key, m_Enumerator.Current.Value); 
                }
            }
            object IEnumerator.Current { get { return m_Enumerator.Current; } }
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
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
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
            m_Values.Remove(key as string);
        }
        public override ScriptArray GetKeys() {
            var ret = m_Script.NewArray();
            foreach (var pair in m_Values) {
                using (var key = new ScriptValue(pair.Key)) {
                    ret.Add(key);
                }
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
