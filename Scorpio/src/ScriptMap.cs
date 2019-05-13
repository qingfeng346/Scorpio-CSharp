using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Function;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio {
    //脚本map类型
    public class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        private Dictionary<object, ScriptValue> m_Objects = new Dictionary<object, ScriptValue>();  //所有的数据(函数和数据都在一个数组)
        public ScriptMap(Script script) : base(script, ObjectType.Map, script.TypeMap) { }
        public IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator() { return m_Objects.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Objects.GetEnumerator(); }

        public override ScriptValue GetValue(string key) {
            return m_Objects.ContainsKey(key) ? m_Objects[key] : (Class != null ? Class.GetValue(key) : ScriptValue.Null);
        }
        public override ScriptValue GetValue(object key) {
            return m_Objects.ContainsKey(key) ? m_Objects[key] : ScriptValue.Null;
        }
        public override void SetValue(string key, ScriptValue value) {
            if (value.valueType == ScriptValue.nullValueType) {
                m_Objects.Remove(key);
            } else {
                m_Objects[key] = value;
            }
        }
        public override void SetValue(object key, ScriptValue value) {
            if (value.valueType == ScriptValue.nullValueType) {
                m_Objects.Remove(key);
            } else {
                m_Objects[key] = value;
            }
        }


        public bool ContainsKey(object key) {
            if (key == null) return false;
            return m_Objects.ContainsKey(key);
        }
        public bool ContainsValue(ScriptValue value) {
            return m_Objects.ContainsValue(value);
        }
        public int Count() {
            return m_Objects.Count;
        }
        public void Clear() {
            m_Objects.Clear();
        }
        public void Remove(object key) {
            m_Objects.Remove(key);
        }
        public ScriptArray GetKeys() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Objects) {
                ret.Add(m_Script.CreateObject(pair.Key));
            }
            return ret;
        }
        public ScriptArray GetValues() {
            var ret = new ScriptArray(m_Script);
            foreach (var pair in m_Objects) {
                ret.Add(pair.Value);
            }
            return ret;
        }

        public override ScriptObject Clone() {
            var ret = new ScriptMap(m_Script);
            foreach (var pair in m_Objects) {
                var value = pair.Value;
                if (value.valueType == ScriptValue.scriptValueType) {
                    var scriptObject = value.scriptValue;
                    if (scriptObject is ScriptArray || scriptObject is ScriptMap) {
                        ret.m_Objects[pair.Key] = new ScriptValue(scriptObject.Clone());
                    } else {
                        ret.m_Objects[pair.Key] = value;
                    }
                } else {
                    ret.m_Objects[pair.Key] = value;
                }
            }
            return ret;
        }
        public override string ToString() { return ToJson(); }
        public override string ToJson() {
            var builder = new StringBuilder();
            builder.Append("{");
            bool first = true;
            foreach (var pair in m_Objects) {
                var value = pair.Value;
                if (value.valueType == ScriptValue.scriptValueType && (value.scriptValue is ScriptFunction || value.scriptValue == this)) { continue; }
                if (first) { first = false; } else { builder.Append(","); }
                builder.Append($"\"{pair.Key}\":{value.ToJson()}");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
