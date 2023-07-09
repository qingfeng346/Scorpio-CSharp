using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptGlobal : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        public struct Enumerator : IEnumerator<KeyValuePair<string, ScriptValue>> {
            private IEnumerator<KeyValuePair<string, int>> indexs;
            private ScriptValue[] objects;
            private KeyValuePair<string, ScriptValue> current;
            internal Enumerator(ScriptGlobal global) {
                indexs = global.m_Indexs.GetEnumerator();
                objects = global.m_Objects;
                current = default;
            }
            public bool MoveNext() {
                if (indexs.MoveNext()) {
                    current = new KeyValuePair<string, ScriptValue>(indexs.Current.Key, objects[indexs.Current.Value]);
                    return true;
                }
                return false;
            }
            public KeyValuePair<string, ScriptValue> Current => current;
            object IEnumerator.Current => current;
            public void Reset() {
                indexs.Reset();
            }
            public void Dispose() {
                indexs.Dispose();
                objects = null;
                current = default;
            }
        }
        private ScriptValue[] m_Objects = ScorpioUtil.VALUE_EMPTY;                                //数据
        private int m_Size = 0;                                                             //有效数据数量
        private Dictionary<string, int> m_Indexs = new Dictionary<string, int>();           //名字到索引的映射
        public ScriptGlobal() : base(ObjectType.Global) { }
        public void Shutdown() {
            m_Objects = ScorpioUtil.VALUE_EMPTY;
            m_Indexs = null;
            m_Size = 0;
        }

        public int GetIndex(string key) {
            if (m_Indexs.TryGetValue(key, out var value)) {
                return value;
            }
            SetValue(key, ScriptValue.Null);
            return m_Indexs[key];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override ScriptValue GetValue(string key) {
            return m_Indexs.TryGetValue(key, out var index) ? GetValueByIndex(index) : default;
        }
        public override ScriptValue GetValueByIndex(int key) { return m_Objects[key]; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetValue(string key, ScriptValue value) {
            if (m_Indexs.TryGetValue(key, out var index)) {
                m_Objects[index] = value;
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
            m_Objects[m_Size++] = value;
        }
        public override void SetValueByIndex(int key, ScriptValue value) {
            m_Objects[key] = value;
        }
        public bool HasValue(string key) {
            return m_Indexs.ContainsKey(key);
        }
        public IEnumerable<string> GetKeys() { return m_Indexs.Keys; }

        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return new Enumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
    }
}
