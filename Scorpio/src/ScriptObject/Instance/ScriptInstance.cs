using System.Collections;
using System.Collections.Generic;
using Scorpio.Library;
using Scorpio.Tools;

namespace Scorpio {
    public class ScriptInstance : ScriptInstanceBase, IEnumerable<KeyValuePair<string, ScriptValue>> {
        protected ScorpioStringDictionary<ScriptValue> m_Values;         //所有的数据(函数和数据都在一个数组)
        public ScriptInstance(ScriptType prototype) : base(prototype) {
            m_Values = new ScorpioStringDictionary<ScriptValue>();
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return m_Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
        public void SetCapacity(int capacity) {
            m_Values.SetCapacity(capacity);
        }
        public bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public void DelValue(string key) {
            m_Values.Remove(key);
        }
        public override void ClearVariables() {
            m_Values.Clear();
        }
        #region GetValue SetValue重载
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key, this);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_Values[key] = value;
        }
        #endregion
        internal override void SerializerJson(ScorpioJsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Values) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(pair.Key);
                builder.Append(":");
                jsonSerializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
    }
}
