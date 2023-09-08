using Scorpio.Library;
using System.Collections;
using System.Collections.Generic;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstanceBase, IEnumerable<KeyValuePair<object, ScriptValue>> {
        public ScriptMap(Script script) : base(script.TypeMap) { }
        public abstract IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        
        public abstract bool ContainsKey(object key);
        public abstract bool ContainsValue(ScriptValue value);
        public abstract int Count();
        public abstract void Clear();

        public abstract bool HasValue(string key);
        public abstract void DelValue(string key);
        public abstract void Remove(object key);
        public abstract ScriptArray GetKeys();
        public abstract ScriptArray GetValues();
        public abstract ScriptMap NewCopy();
        internal override void SerializerJson(ScorpioJsonSerializer jsonSerializer) {
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
        public override string ToString() {
            return script.ToJson(this);
        }
    }
}
