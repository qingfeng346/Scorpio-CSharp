using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptGlobal : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        public struct Enumerator : IEnumerator<KeyValuePair<string, ScriptValue>> {
            private IEnumerator<string> keys;
            private IEnumerator<int> values;
            private ScriptValue[] objects;
            private KeyValuePair<string, ScriptValue> current;
            internal Enumerator(ScriptGlobal global) {
                this.keys = global.m_Indexs.Keys.GetEnumerator();
                this.values = global.m_Indexs.Values.GetEnumerator();
                this.objects = global.m_Objects;
                this.current = default;
            }
            public bool MoveNext() {
                if (keys.MoveNext()) {
                    values.MoveNext();
                    current = new KeyValuePair<string, ScriptValue>(keys.Current, objects[values.Current]);
                    return true;
                }
                return false;
            }
            public KeyValuePair<string, ScriptValue> Current => current;
            object IEnumerator.Current => current;
            public void Reset() {
                keys.Reset();
                values.Reset();
            }
            public void Dispose() {
                keys.Dispose();
                values.Dispose();
            }
        }
        private ScriptValue[] m_Objects = ScorpioUtil.VALUE_EMPTY;                                //数据
        private int m_Size = 0;                                                             //有效数据数量
        private Dictionary<string, int> m_Indexs = new Dictionary<string, int>();           //名字到索引的映射
        public ScriptGlobal(Script script) : base(script, ObjectType.Global) { }
        public void Shutdown() {
            ScorpioUtil.Free(m_Objects, m_Size);
            m_Indexs.Clear();
            m_Size = 0;
        }
        public override void Free() { }
        public override void gc() { }
        public int GetIndex(string key) {
            if (m_Indexs.TryGetValue(key, out var value)) {
                return value;
            }
            SetValue(key, ScriptValue.Null);
            return m_Indexs[key];
        }
        #region 重载 GetValue SetValue
        public override ScriptValue GetValue(string key) {
            return m_Indexs.TryGetValue(key, out var index) ? GetValueByIndex(index) : ScriptValue.Null;
        }
        public override ScriptValue GetValueByIndex(int key) { return m_Objects[key]; }
        public override void SetValue(string key, ScriptValue value) {
            if (m_Indexs.TryGetValue(key, out var index)) {
                SetValueByIndex(index, value);
                return;
            }
            if (m_Size == m_Objects.Length) {
                var array = new ScriptValue[m_Size + 128];
                if (m_Size > 0) {
                    Array.Copy(m_Objects, 0, array, 0, m_Size);
                }
                m_Objects = array;
            }
            m_Indexs[string.Intern(key)] = m_Size;
            m_Objects[m_Size++].CopyFrom(value);
        }
        public override void SetValueByIndex(int key, ScriptValue value) {
            m_Objects[key].CopyFrom(value);
        }
        #endregion
        public void SetValueNoReference(string key, ScriptValue value) {
            if (m_Indexs.TryGetValue(key, out var index)) {
                m_Objects[index].Set(value);
                return;
            }
            if (m_Size == m_Objects.Length) {
                var array = new ScriptValue[m_Size + 128];
                if (m_Size > 0) {
                    Array.Copy(m_Objects, 0, array, 0, m_Size);
                }
                m_Objects = array;
            }
            m_Indexs[string.Intern(key)] = m_Size;
            m_Objects[m_Size++].Set(value);
        }

        public bool HasValue(string key) {
            return m_Indexs.ContainsKey(key);
        }
        public IEnumerable<string> GetKeys() { return m_Indexs.Keys; }

        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
    }
}
