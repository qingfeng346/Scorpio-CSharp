using System;
using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Tools;
using Scorpio.Library;
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
                var parameters = ScorpioUtil.Parameters;
                parameters[0] = o1;
                parameters[1] = o2;
                var ret = func.Call(default, parameters, 2);
                switch (ret.valueType) {
                    case ScriptValue.doubleValueType: return (int)ret.doubleValue;
                    case ScriptValue.longValueType: return (int)ret.longValue;
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
                this.current = default;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = values[index];
                    index++;
                    return true;
                }
                return false;
            }
            public ScriptValue Current => current;
            object System.Collections.IEnumerator.Current => current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() {
                values = null;
                current = default;
            }
        }
        internal ScriptValue[] m_Objects;
        internal int m_Length;
        
        public ScriptArray(Script script) : base(script, ObjectType.Array, script.TypeArray) {
            m_Objects = ScorpioUtil.VALUE_EMPTY;
            m_Length = 0;
        }
        internal ScriptArray(Script script, ScriptValue[] parameters, int length) : this(script) {
            for (var i = 0; i < length;++i) {
                Add(parameters[i]);
            }
        }
        public void SetArrayCapacity(int capacity) {
            if (capacity > m_Length) {
                SetCapacity_impl(capacity);
            } else {
                m_Objects = ScorpioUtil.VALUE_EMPTY;
            }
        }
        void SetCapacity_impl(int capacity) {
            var array = new ScriptValue[capacity];
            if (m_Length > 0) {
                Array.Copy(m_Objects, 0, array, 0, m_Length);
            }
            m_Objects = array;
        }
        void ExpandCapacity() {
            SetCapacity_impl(m_Length + 8);
        }
        public void TrimCapacity() {
            if (m_Length != m_Objects.Length) {
                SetCapacity_impl(m_Length);
            }
        }
        public new IEnumerator<ScriptValue> GetEnumerator() { return new Enumerator(this); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new Enumerator(this); }
        public IEnumerator<ScriptValue> GetIterator() { return new Enumerator(this); }

        public override ScriptValue GetValue(double index) {
            return this[(int)index];
        }
        public override ScriptValue GetValue(long index) {
            return this[(int)index];
        }
        public override ScriptValue GetValue(object index) {
            return this[Convert.ToInt32(index)];
        }
        public override void SetValue(double index, ScriptValue value) {
            this[(int)index] = value;
        }
        public override void SetValue(long index, ScriptValue value) {
            this[(int)index] = value;
        }
        public override void SetValue(object index, ScriptValue value) {
            this[Convert.ToInt32(index)] = value;
        }

        public virtual ScriptValue this[int i] {
            get {
                if (i < 0) throw new ExecutionException($"Array.get[] 索引小于0:{i}");
                return i < m_Length ? m_Objects[i] : ScriptValue.Null;
            }
            set {
                if (i < 0 || i > m_Length) throw new ExecutionException($"Array.set[] 索引小于0或超过当前长度:{i}  Length:{m_Length}");
                if (i == m_Length) {
                    ExpandCapacity();
                    m_Length = i + 1;
                }
                m_Objects[i] = value;
            }
        }
        public void Add(ScriptValue value) {
            if (m_Length == m_Objects.Length) {
                ExpandCapacity();
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
                ExpandCapacity();
            }
            m_Objects[m_Length++] = value;
        }
        public void Insert(int index, ScriptValue value) {
            if (index < 0 || index > m_Length) throw new ExecutionException($"Array.Insert 索引小于0或超过最大值 index:{index} length:{m_Length}");
            if (m_Length == m_Objects.Length) {
                ExpandCapacity();
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
            if (index < 0 || index >= m_Length) throw new ExecutionException($"Array.RemoveAt 索引小于0或超过最大值 index:{index} length:{m_Length}");
            m_Length--;
            Array.Copy(m_Objects, index + 1, m_Objects, index, m_Length - index);
            m_Objects[m_Length] = default;
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
        public void Clear() {
            if (m_Length > 0) {
                m_Objects = ScorpioUtil.VALUE_EMPTY;
                m_Length = 0;
            }
        }
        public int Length() {
            return m_Length;
        }
        public void Sort(ScriptFunction func) {
            Array.Sort(m_Objects, 0, m_Length, new Comparer(func));
        }
        public ScriptValue First() {
            return m_Length > 0 ? m_Objects[0] : ScriptValue.Null;
        }
        public ScriptValue Last() {
            return m_Length > 0 ? m_Objects[m_Length - 1] : ScriptValue.Null;
        }
        public ScriptValue PopFirst() {
            if (m_Length <= 0)  throw new ExecutionException($"Array.PopFirst 数组长度为0");
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
            if (m_Length <= 0)  throw new ExecutionException($"Array.PopLast 数组长度为0");
            return m_Objects[--m_Length];
        }
        public ScriptValue SafePopLast() {
            return m_Length == 0 ? ScriptValue.Null : m_Objects[--m_Length];
        }
        //仅限于number和string
        public T[] ToArray<T>() {
            var array = new T[m_Length];
            for (var i = 0; i < m_Length; ++i) {
                array[i] = (T)m_Objects[i].ChangeType(typeof(T));
            }
            return array;
        }
        public Array ToArray(Type type) {
            var array = Array.CreateInstance(type, m_Length);
            for (var i = 0; i < m_Length; ++i) {
                array.SetValue(m_Objects[i].ChangeType(type), i);
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
        public ScriptArray NewCopy(int length = 0) {
            var ret = new ScriptArray(m_Script) {
                m_Objects = new ScriptValue[m_Length + length],
                m_Length = m_Length
            };
            for (int i = 0; i < m_Length; ++i) {
                ret.m_Objects[i] = m_Objects[i];
            }
            return ret;
        }
        public override string ToString() {
            return m_Script.ToJson(this);
        }
        internal override void SerializerJson(ScorpioJsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("[");
            for (int i = 0; i < m_Length; ++i) {
                if (i != 0) builder.Append(",");
                jsonSerializer.Serializer(m_Objects[i]);
            }
            builder.Append("]");
        }
    }
}
