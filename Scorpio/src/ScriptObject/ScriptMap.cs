using Scorpio.Library;
using System.Collections.Generic;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        public ScriptMap(Script script) : base(script, ObjectType.Map) { }
        public abstract new IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        public override void Alloc() {
            AddRecord();
            SetPrototype(script.TypeMap);
        }
        public abstract bool ContainsKey(object key);
        public abstract bool ContainsValue(ScriptValue value);
        public abstract int Count();
        public abstract void Clear();
        public abstract void Remove(object key);
        public abstract ScriptArray GetKeys();
        public abstract ScriptArray GetValues();
        public abstract ScriptMap NewCopy();
        internal override void ToString(ScorpioStringSerializer serializer) {
            var builder = serializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in this) {
                if (first) { first = false; } else { builder.Append(","); }
                serializer.Serializer(pair.Key.ToString());
                builder.Append(":");
                serializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
        internal override void ToJson(ScorpioJsonSerializer serializer) {
            var builder = serializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in this) {
                if (first) { first = false; } else { builder.Append(","); }
                serializer.Serializer(pair.Key.ToString());
                builder.Append(":");
                serializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
        public override string ToString() {
            return m_Script.ToString(this);
        }
    }
}
