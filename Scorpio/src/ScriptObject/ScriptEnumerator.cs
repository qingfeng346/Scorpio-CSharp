using System;

namespace Scorpio {
    //迭代器
    public class ScriptEnumerator : ScriptObject {
        private IDisposable m_Disposable;
        private ScriptValue m_Next;
        private ScriptValue m_Key;
        private ScriptValue m_Value;
        private Script m_Script;
        public ScriptEnumerator(Script script) : base(ObjectType.Enumerator) {
            m_Script = script;
        }
        public void SetNext(ScorpioHandle next) {
            m_Disposable = next as IDisposable;
            m_Next = m_Script.CreateFunction(next);
        }
        public void SetKey(ScriptValue key) {
            //key都是CreateValue的,不用再加计数
            m_Key = key;
        }
        public void SetValue(ScriptValue value) {
            //value都是直接获取的,需要引用一次
            m_Value = value;
        }
        public override ScriptValue GetValue(string key) {
            switch (key) {
                case "next": return m_Next;
                case "key": return m_Key;
                case "value": return m_Value;
                default: return default;
            }
        }
        ~ScriptEnumerator() {
            if (m_Disposable != null) {
                m_Disposable.Dispose();
                m_Disposable = null;
            }
            m_Next = default;
            m_Key = default;
            m_Value = default;
            m_Script = null;
        }
    }
}
