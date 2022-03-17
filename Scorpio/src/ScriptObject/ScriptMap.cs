using System.Collections.Generic;
using Scorpio.Library;
namespace Scorpio {
    //脚本map类型
    public abstract class ScriptMap : ScriptInstance, IEnumerable<KeyValuePair<object, ScriptValue>> {
        protected Script m_Script;
        public ScriptMap(Script script) : base(ObjectType.Map, script.TypeMap) {
            m_Script = script;
        }
        public Script getScript() { return m_Script; }
        public abstract new IEnumerator<KeyValuePair<object, ScriptValue>> GetEnumerator();
        public abstract bool ContainsKey(object key);
        public abstract bool ContainsValue(ScriptValue value);
        public abstract int Count();
        public abstract void Clear();
        public abstract void Remove(object key);
        public abstract ScriptArray GetKeys();
        public abstract ScriptArray GetValues();
        public abstract ScriptMap NewCopy();
        public override string ToString() { return new ScorpioJsonSerializer().ToJson(this); }
    }
}
