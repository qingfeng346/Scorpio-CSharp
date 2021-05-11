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
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
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
        public override ScriptArray GetKeys() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Values) {
                ret.Add(new ScriptValue(pair.Key));
            }
            return ret;
        }
        public override ScriptArray GetValues() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Values) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public override ScriptMap NewCopy() {
            var ret = new ScriptMapString(m_Script);
            foreach (var pair in m_Values) {
                ret.m_Values[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapString(m_Script);
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
