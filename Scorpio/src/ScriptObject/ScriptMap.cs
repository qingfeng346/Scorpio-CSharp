using Scorpio.Library;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        public ScriptMap(Script script) : base(script, ObjectType.Map) { }
        public abstract new IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract bool ContainsKey(object key);
        public abstract bool ContainsValue(ScriptValue value);
        public abstract int Count();
        public abstract void Clear();
        public abstract void Remove(object key);
        public ScriptArray GetKeys() {
            var ret = m_Script.NewArray();
            foreach (var pair in this) {
                using (var key = ScriptValue.CreateValue(m_Script, pair.Key)) {
                    ret.Add(key);
                }
            }
            return ret;
        }
        public ScriptArray GetValues() {
            var ret = m_Script.NewArray();
            foreach (var pair in this) {
                ret.Add(pair.Value);
            }
            return ret;
        }
        public abstract ScriptMap NewCopy();
        public override string ToString() {
            return m_Script.ToJson(this);
        }
        internal override void ToJson(ScorpioJsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in this) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(pair.Key.ToString());
                builder.Append(":");
                jsonSerializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
    }
}
