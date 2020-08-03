using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio {
    //脚本数组类型
    public class ScriptArray : ScriptInstance, IEnumerable<ScriptValue> {
        //排序
        public struct Comparer : IComparer<ScriptValue> {
            ScriptFunction func;
            public Comparer(ScriptFunction func) {
                this.func = func;
            }
            public int Compare(ScriptValue o1, ScriptValue o2) {
                ScriptValue.Parameters[0] = o1;
                ScriptValue.Parameters[1] = o2;
                var ret = func.Call(ScriptValue.Null, ScriptValue.Parameters, 2);
                switch (ret.valueType) {
                    case ScriptValue.doubleValueType: return Convert.ToInt32(ret.doubleValue);
                    case ScriptValue.longValueType: return Convert.ToInt32(ret.longValue);
                    case ScriptValue.trueValueType: return 1;
                    case ScriptValue.falseValueType: return -1;
                    default: throw new ExecutionException("数组排序返回值必须是Number或Bool类型");
                }
            }
        }
        //数组迭代器
        public struct Enumerator : IEnumerator<ScriptValue> {
            private int length;
            private ScriptValue[] values;
            private int index;
            private ScriptValue current;
            internal Enumerator(ScriptArray list) {
                this.length = list.m_Length;
                this.values = list.m_Objects;
                this.index = 0;
                this.current = ScriptValue.Null;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = values[index];
                    index++;
                    return true;
                }
                return false;
            }
            public ScriptValue Current { get { return current; } }
            object System.Collections.IEnumerator.Current { get { return current; } }
            public void Reset() {
                index = 0;
                current = ScriptValue.Null;
            }
            public void Dispose() { }
        }
        private Script m_Script;
        internal ScriptValue[] m_Objects;
        internal int m_Length;
        
        public ScriptArray(Script script) : base(ObjectType.Array, script.TypeArrayValue) {
            m_Script = script;
            m_Objects = ScriptValue.EMPTY;
            m_Length = 0;
        }
        internal ScriptArray(Script script, ScriptValue[] parameters, int length) : this(script) {
            for (var i = 0; i < length;++i) {
                Add(parameters[i]);
            }
        }
        public Script getScript() { return m_Script; }
        internal ScriptValue[] getObjects() { return m_Objects; }
        void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScriptValue[value];
                if (m_Length > 0) {
                    Array.Copy(m_Objects, 0, array, 0, m_Length);
                }
                m_Objects = array;
            } else {
                m_Objects = ScriptValue.EMPTY;
            }
        }
        void EnsureCapacity(int min) {
            if (m_Objects.Length < min) {
                int num = (m_Objects.Length == 0) ? 4 : (m_Objects.Length * 2);
                if (num > 2146435071) { num = 2146435071; } else if (num < min) { num = min; }
                SetCapacity(num);
            }
        }
        public new IEnumerator<ScriptValue> GetEnumerator() { return new Enumerator(this); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new Enumerator(this); }
        public IEnumerator<ScriptValue> GetIterator() { return new Enumerator(this); }

        public override ScriptValue GetValue(object index) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                var i = Convert.ToInt32(index);
                if (i >= m_Length) return ScriptValue.Null;
                if (i < 0) throw new ExecutionException($"数组获取变量索引小于0 index : {i}");
                return m_Objects[i];
            }
            return base.GetValue(index);
        }
        public override void SetValue(object index, ScriptValue value) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                var i = Convert.ToInt32(index);
                if (i >= m_Length) {
                    EnsureCapacity(i + 1);
                    m_Length = i + 1;
                } else if (i < 0) {
                    throw new ExecutionException($"数组设置变量索引小于0 index : {i}");
                }
                m_Objects[i] = value;
            } else {
                base.SetValue(index, value);
            }
        }

        public virtual ScriptValue this[int i] {
            get {
                if (i >= m_Length) return ScriptValue.Null;
                if (i < 0) throw new ExecutionException($"数组获取变量索引小于0 index : {i}");
                return m_Objects[i];
            }
            set {
                if (i >= m_Length) {
                    EnsureCapacity(i + 1);
                    m_Length = i + 1;
                } else if (i < 0) {
                    throw new ExecutionException($"数组设置变量索引小于0 index : {i}");
                }
                m_Objects[i] = value;
            }
        }
        public void Add(ScriptValue value) {
            if (m_Length == m_Objects.Length) {
                EnsureCapacity(m_Length + 1);
            }
            m_Objects[m_Length++] = value;
        }
        public void AddUnique(ScriptValue value) {
            for (int i = 0; i < m_Length; ++i) {
                if (value.Equals(m_Objects[i])) {
                    return;
                }
            }
            if (m_Length == m_Objects.Length) {
                EnsureCapacity(m_Length + 1);
            }
            m_Objects[m_Length++] = value;
        }
        public void Insert(int index, ScriptValue value) {
            if (index < 0 || index >= m_Length)
                throw new ExecutionException($"Insert 索引小于0或超过最大值 index:{index}   length:{m_Length}");
            if (m_Length == m_Objects.Length) {
                EnsureCapacity(m_Length + 1);
            }
            Array.Copy(m_Objects, index, m_Objects, index + 1, m_Length - index);
            m_Objects[index] = value;
            m_Length++;
        }
        public bool Remove(ScriptValue value) {
            int num = IndexOf(value);
            if (num >= 0) {
                RemoveAt(num);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index) {
            if (index < 0 || index >= m_Length)
                throw new ExecutionException($"RemoveAt 索引小于0或超过最大值 index:{index}  length:{m_Length}");
            m_Length--;
            Array.Copy(m_Objects, index + 1, m_Objects, index, m_Length - index);
            m_Objects[m_Length].valueType = ScriptValue.nullValueType;
        }
        public bool Contains(ScriptValue obj) {
            for (int i = 0; i < m_Length; ++i) {
                if (obj.Equals(m_Objects[i])) {
                    return true;
                }
            }
            return false;
        }
        public int IndexOf(ScriptValue obj) {
            for (int i = 0; i < m_Length; ++i) {
                if (obj.Equals(m_Objects[i])) {
                    return i;
                }
            }
            return -1;
        }
        public int LastIndexOf(ScriptValue obj) {
            for (int i = m_Length - 1; i >= 0; --i) {
                if (obj.Equals(m_Objects[i])) {
                    return i;
                }
            }
            return -1;
        }
        public void Resize(int length) {
            if (length < 0)
                throw new ExecutionException($"Resize长度小于0 length:{length}");
            if (length > m_Length) {
                EnsureCapacity(length);
                m_Length = length;
            } else {
                Array.Clear(m_Objects, length, m_Length - length);
                m_Length = length;
            }
        }
        public void Clear() {
            if (m_Length > 0) {
                Array.Clear(m_Objects, 0, m_Length);
                m_Length = 0;
            }
        }
        public int Length() {
            return m_Length;
        }
        public void Sort(ScriptFunction func) {
            Array.Sort<ScriptValue>(m_Objects, 0, m_Length, new Comparer(func));
        }
        public ScriptValue First() {
            return m_Length > 0 ? m_Objects[0] : ScriptValue.Null;
        }
        public ScriptValue Last() {
            return m_Length > 0 ? m_Objects[m_Length - 1] : ScriptValue.Null;
        }
        public ScriptValue PopFirst() {
            if (m_Length == 0)
                throw new ExecutionException("Array PopFirst 数组长度为 0");
            var value = m_Objects[0];
            RemoveAt(0);
            return value;
        }
        public ScriptValue SafePopFirst() {
            if (m_Length == 0)
                return ScriptValue.Null;
            var value = m_Objects[0];
            RemoveAt(0);
            return value;
        }
        public ScriptValue PopLast() {
            if (m_Length == 0)
                throw new ExecutionException("Array PopLast 数组长度为0");
            return m_Objects[--m_Length];
        }
        public ScriptValue SafePopLast() {
            if (m_Length == 0)
                return ScriptValue.Null;
            return m_Objects[--m_Length];
        }
        //仅限于number和string
        public T[] ToArray<T>() {
            var array = new T[m_Length];
            for (var i = 0; i < m_Length; ++i) {
                array[i] = (T)Util.ChangeType(m_Objects[i], typeof(T));
            }
            return array;
        }
        public Array ToArray(Type type) {
            var array = Array.CreateInstance(type, m_Length);
            for (var i = 0; i < m_Length; ++i) {
                array.SetValue(Util.ChangeType(m_Objects[i], type), i);
            }
            return array;
        }
        public override ScriptObject Clone(bool deep) {
            var ret = new ScriptArray(m_Script);
            ret.m_Objects = new ScriptValue[m_Length];
            ret.m_Length = m_Length;
            if (deep) {
                for (int i = 0; i < m_Length; ++i) {
                    var value = m_Objects[i];
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Objects[i] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Objects[i] = value;
                        }
                    } else {
                        ret.m_Objects[i] = value;
                    }
                }
            } else {
                for (int i = 0; i < m_Length; ++i) {
                    ret.m_Objects[i] = m_Objects[i];
                }
            }
            return ret;
        }
        public ScriptArray NewCopy() {
            var ret = new ScriptArray(m_Script);
            ret.m_Objects = new ScriptValue[m_Length];
            ret.m_Length = m_Length;
            for (int i = 0; i < m_Length; ++i) {
                ret.m_Objects[i] = m_Objects[i];
            }
            return ret;
        }
        public override string ToString() { return ToJson(false); }
        public override string ToJson(bool supportKeyNumber) {
            var builder = new StringBuilder();
            builder.Append("[");
            for (int i = 0; i < m_Length; ++i) {
                if (i != 0) builder.Append(",");
                builder.Append(m_Objects[i].ToJson(supportKeyNumber));
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
