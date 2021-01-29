using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Tools;
namespace Scorpio {
    public struct MapValue {
        public object Key;
        public ScriptValue Value;
        public MapValue(object key, ScriptValue value) {
            this.Key = key;
            this.Value = value;
        }
    }
    //脚本map类型
    public class ScriptMap : ScriptInstance, IEnumerable<MapValue> {
        public struct Enumerator : IEnumerator<MapValue> {
            private IEnumerator<KeyValuePair<int, ScriptValue>> enumerator1;
            private IEnumerator<KeyValuePair<object, ScriptValue>> enumerator2;
            private MapValue current;
            private bool first;
            internal Enumerator(ScriptMap map) {
                enumerator1 = map.m_Values.GetEnumerator();
                enumerator2 = map.m_Objects.GetEnumerator();
                current = default(MapValue);
                first = true;
            }
            public bool MoveNext() {
                if (first) {
                    if (enumerator1.MoveNext()) {
                        current.Key = enumerator1.Current.Key.GetStringByCode();
                        current.Value = enumerator1.Current.Value;
                        return true;
                    }
                    first = false;
                }
                if (enumerator2.MoveNext()) {
                    current.Key = enumerator2.Current.Key;
                    current.Value = enumerator2.Current.Value;
                    return true;
                }
                return false;
            }
            object IEnumerator.Current => current;
            public MapValue Current => current;
            public void Reset() {
                current = default(MapValue);
                enumerator1.Reset();
                enumerator2.Reset();
            }
            public void Dispose() {
                enumerator1.Dispose();
                enumerator2.Dispose();
            }
        }

        private Script m_Script;
        internal Dictionary<object, ScriptValue> m_Objects = new Dictionary<object, ScriptValue>();  //所有的数据(函数和数据都在一个数组)
        
        public ScriptMap(Script script) : base(ObjectType.Map, script.TypeMapValue) {
            m_Script = script;
        }
        internal ScriptMap(Script script, ScriptValue[] parameters, int length) : this(script) { }
        public Script getScript() { return m_Script; }
        public new IEnumerator<MapValue> GetEnumerator() {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator() { 
            return new Enumerator(this);
        }
        public override ScriptValue GetValue(object key) {
            return m_Objects.TryGetValue(key, out var value) ? value : ScriptValue.Null;
        }
        public override void SetValue(object key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public bool ContainsKey(object key) {
            if (key == null) return false;
            return key is string ? m_Values.ContainsKey(((string)key).GetCodeByString()) : m_Objects.ContainsKey(key);
        }
        public bool ContainsValue(ScriptValue value) {
            return m_Values.ContainsValue(value) || m_Objects.ContainsValue(value);
        }
        public int Count() {
            return m_Values.Count + m_Objects.Count;
        }
        public void Clear() {
            m_Values.Clear();
            m_Objects.Clear();
        }
        public void Remove(object key) {
            if (key is string) {
                m_Values.Remove(((string)key).GetCodeByString());
            } else {
                m_Objects.Remove(key);
            }
        }
        public ScriptArray GetKeys() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Values) {
                ret.Add(ScriptValue.CreateValue(pair.Key.GetStringByCode()));
            }
            foreach (var pair in m_Objects) {
                ret.Add(ScriptValue.CreateValue(pair.Key));
            }
            return ret;
        }
        public ScriptArray GetValues() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Values) {
                ret.Add(pair.Value);
            }
            foreach (var pair in m_Objects) {
                ret.Add(pair.Value);
            }
            return ret;
        }

        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptMap(m_Script);
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
                foreach (var pair in m_Values) {
                    ret.m_Values[pair.Key] = pair.Value;
                }
                foreach (var pair in m_Objects) {
                    ret.m_Objects[pair.Key] = pair.Value;
                }
            }
            return ret;
        }
        public ScriptMap NewCopy() {
            var ret = new ScriptMap(m_Script);
            foreach (var pair in m_Values) {
                ret.m_Values[pair.Key] = pair.Value;
            }
            foreach (var pair in m_Objects) {
                ret.m_Objects[pair.Key] = pair.Value;
            }
            return ret;
        }
        public override string ToString() { return ToJson(false, true); }
        public override string ToJson(bool supportKeyNumber, bool ucode) {
            var builder = new StringBuilder();
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Values) {
                var value = pair.Value;
                if (value.valueType == ScriptValue.scriptValueType && (value.scriptValue is ScriptFunction || value.scriptValue == this)) { continue; }
                if (first) { first = false; } else { builder.Append(","); }
                builder.Append($"\"{pair.Key.GetStringByCode()}\":{value.ToJson(supportKeyNumber, ucode)}");
            }
            if (supportKeyNumber) {
                foreach (var pair in m_Objects) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType && (value.scriptValue is ScriptFunction || value.scriptValue == this)) { continue; }
                    if (first) { first = false; } else { builder.Append(","); }
                    builder.Append($"{pair.Key}:{value.ToJson(supportKeyNumber, ucode)}");
                }
            } else {
                foreach (var pair in m_Objects) {
                    var value = pair.Value;
                    if (value.valueType == ScriptValue.scriptValueType && (value.scriptValue is ScriptFunction || value.scriptValue == this)) { continue; }
                    if (first) { first = false; } else { builder.Append(","); }
                    builder.Append($"\"{pair.Key}\":{value.ToJson(supportKeyNumber, ucode)}");
                }
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
