using System.Collections;
using System.Collections.Generic;
using Scorpio.Library;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        protected Script m_Script;
        public ScriptMap(Script script) : base(ObjectType.Map) {
            //不继承,避免分配父级m_Values
            m_Script = script;
            m_Prototype = script.TypeMap;
        }
        public Script getScript() { return m_Script; }
        public abstract new IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public abstract bool ContainsKey(object key);
        public abstract bool ContainsValue(ScriptValue value);
        public abstract int Count();
        public abstract void Clear();
        public abstract void Remove(object key);
        public abstract ScriptArray GetKeys();
        public abstract ScriptArray GetValues();
        public abstract ScriptMap NewCopy();
        public override string ToString() {
            using (var serializer = new ScorpioJsonSerializer()) {
                return serializer.ToJson(this);
            }
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
