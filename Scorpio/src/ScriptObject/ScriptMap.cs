using System.Collections.Generic;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        public ScriptMap(Script script) : base(script, ObjectType.Map) { }
        public abstract new IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        public override void Alloc() {
            base.Alloc();
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
        public override string ToString() {
            return m_Script.ToJson(this);
        }
    }
}
