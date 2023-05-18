using System.Collections;
using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio {
    //脚本map类型
    public class ScriptMapString : ScriptMap {
        //数组迭代器
        public struct Enumerator : IEnumerator<KeyValuePair<object, ScriptValue>> {
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
        public ScriptMapString(Script script, int capacity) : base(script) {
            m_Values = new ScorpioStringDictionary<ScriptValue>(capacity);
        }
        public override IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() => new Enumerator(this);
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
            throw new ExecutionException("MapString 不支持 Remove, 请使用普通 Map");
        }
        public override void Trim() {
            m_Values.TrimCapacity();
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
            var ret = new ScriptMapString(m_Script, m_Values.Count);
            foreach (var pair in m_Values) {
                ret.m_Values[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMapString(m_Script, m_Values.Count);
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
