using System;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptGlobal : ScriptObject {
        private ScriptValue[] m_Objects = ScriptValue.EMPTY;                                //数据
        private int m_Size = 0;                                                             //有效数据数量
        private ScorpioDictionaryString<int> m_Indexs = new ScorpioDictionaryString<int>(); //名字到索引的映射
        public ScriptGlobal() : base(ObjectType.Global) { }

        void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScriptValue[value];
                if (m_Size > 0) {
                    Array.Copy(m_Objects, 0, array, 0, m_Size);
                }
                m_Objects = array;
            } else {
                m_Objects = ScriptValue.EMPTY;
            }
        }
        void EnsureCapacity(int min) {
            if (m_Objects.Length < min) {
                var num = (m_Objects.Length == 0) ? 4 : (m_Objects.Length * 2);
                if (num > 2146435071) { num = 2146435071; }
                else if (num < min) { num = min; }
                SetCapacity(num);
            }
        }

        public int GetIndex(string key) {
            int value;
            if (m_Indexs.TryGetValue(key, out value)) {
                return value;
            }
            SetValue(key, ScriptValue.Null);
            return m_Indexs[key];
        }
        public override ScriptValue GetValue(string key) { return m_Indexs.ContainsKey(key) ? GetValueByIndex(m_Indexs[key]) : ScriptValue.Null; }
        public override ScriptValue GetValueByIndex(int key) { return m_Objects[key]; }
        public override void SetValue(string key, ScriptValue value) {
            int index;
            if (m_Indexs.TryGetValue(key, out index)) {
                SetValueByIndex(index, value);
                return;
            }
            m_Indexs[key] = m_Size;
            EnsureCapacity(m_Size + 1);
            m_Objects[m_Size++] = value;
        }
        public override void SetValueByIndex(int key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public bool HasValue(string key) {
            return m_Indexs.ContainsKey(key);
        }
    }
}
