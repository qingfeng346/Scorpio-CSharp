using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio {
    public class ScriptGlobal : ScriptObject, IEnumerable<ScorpioValue<string, ScriptValue>> {
        public struct Enumerator : IEnumerator<ScorpioValue<string, ScriptValue>> {
            private ScriptValue[] objects;
            private IEnumerator<KeyValuePair<int, int>> enumerator;
            private ScorpioValue<string, ScriptValue> current;
            internal Enumerator(ScriptGlobal global) {
                this.enumerator = global.m_Indexs.GetEnumerator();
                this.objects = global.m_Objects;
                this.current = new ScorpioValue<string, ScriptValue>();
            }
            public bool MoveNext() {
                if (enumerator.MoveNext()) {
                    current.Key = enumerator.Current.Key.GetStringByCode();
                    current.Value = objects[enumerator.Current.Value];
                    return true;
                }
                return false;
            }
            public ScorpioValue<string, ScriptValue> Current { get { return current; } }
            object System.Collections.IEnumerator.Current { get { return current; } }
            public void Reset() {
                enumerator.Reset();
            }
            public void Dispose() {
                enumerator.Dispose();
            }
        }
        private ScriptValue[] m_Objects = ScriptValue.EMPTY;                        //数据
        private int m_Size = 0;                                                     //有效数据数量
        private Dictionary<int, int> m_Indexs = new Dictionary<int, int>();         //名字到索引的映射
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
        public int GetIndex(int key) {
            if (m_Indexs.TryGetValue(key, out var value)) {
                return value;
            }
            SetValue(key, ScriptValue.Null);
            return m_Indexs[key];
        }
        public override ScriptValue GetValueByIndex(int index) { return m_Objects[index]; }
        public override ScriptValue GetValue(int key) {
            return m_Indexs.TryGetValue(key, out var index) ? GetValueByIndex(index) : ScriptValue.Null;
        }
        public override void SetValue(int key, ScriptValue value) {
            if (m_Indexs.TryGetValue(key, out var index)) {
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
            return m_Indexs.ContainsKey(key.GetCodeByString());
        }
        public IEnumerator<ScorpioValue<string, ScriptValue>> GetEnumerator() {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return new Enumerator(this);
        }
    }
}
