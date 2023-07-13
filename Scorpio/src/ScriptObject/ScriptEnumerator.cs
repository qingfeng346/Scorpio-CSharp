using System;

namespace Scorpio {
    //迭代器
    public class ScriptEnumerator : ScriptObject {
        private IDisposable m_Disposable;
        private ScriptValue m_Next;
        private ScriptValue m_Key;
        private ScriptValue m_Value;
        public ScriptEnumerator(Script script) : base(script, ObjectType.Enumerator) { }
        public void Set(ScorpioHandle next) {
            m_Disposable = next as IDisposable;
            m_Next = new ScriptValue(m_Script.CreateFunction(next));
        }
        public void SetKey(ScriptValue key) {
            //key都是CreateValue的,不用再加计数
            m_Key.Set(key);
        }
        public void SetValue(ScriptValue value) {
            //value都是直接获取的,需要引用一次
            m_Value.CopyFrom(value);
        }
        public void SetValueUserdata(ScriptValue value) {
            //userdata都是CreateValue的,不用再加计数
            m_Value.Set(value);
        }
        public override void Free() {
            DelRecord();
            if (m_Disposable != null) {
                m_Disposable.Dispose();
                m_Disposable = null;
            }
            m_Next.Free();
            m_Key.Free();
            m_Value.Free();
            m_Script.Free(this);
        }
        public override void gc() { }
        public override ScriptValue GetValue(string key) {
            switch (key) {
                case "next": return m_Next;
                case "key": return m_Key;
                case "value": return m_Value;
                default: return default;
            }
        }
    }
}
