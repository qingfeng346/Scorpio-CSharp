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
                using (var parameter = ScorpioParameters.Get()) {
                    parameter[0] = o1;
                    parameter[1] = o2;
                    var ret = func.Call(ScriptValue.Null, parameter.values, 2);
                    switch (ret.valueType) {
                        case ScriptValue.doubleValueType: return Convert.ToInt32(ret.doubleValue);
                        case ScriptValue.int64ValueType: return Convert.ToInt32(ret.longValue);
                        case ScriptValue.trueValueType: return 1;
                        case ScriptValue.falseValueType: return -1;
                        default: throw new ExecutionException("数组排序返回值必须是Number或Bool类型");
                    }
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
        internal ScriptValue[] m_Objects;
        internal int m_Length;
        
        public ScriptArray(Script script) : base(script, ObjectType.Array) {
            m_Objects = ScriptValue.EMPTY;
            m_Length = 0;
            Set(script.TypeArrayValue);
        }
        internal ScriptValue[] getObjects() { return m_Objects; }
        public override void Free() {
            Release();
            Clear();
            m_Script.Free(this);
        }
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
                int num = (m_Objects.Length == 0) ? 8 : (m_Objects.Length * 2);
                if (num > 2146435071) { num = 2146435071; } else if (num < min) { num = min; }
                SetCapacity(num);
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
                if (i < 0) throw new ExecutionException($"Array.set[] 索引小于0:{i}");
                if (i >= m_Length) {
                    EnsureCapacity(i + 1);
                    m_Length = i + 1;
                }
                m_Objects[i].CopyFrom(value);
            }
        }
        public void Add(ScriptValue value) {
            if (m_Length == m_Objects.Length) {
                EnsureCapacity(m_Length + 1);
            }
            m_Objects[m_Length++].CopyFrom(value);
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
            m_Objects[m_Length++].CopyFrom(value);
        }
        public void Insert(int index, ScriptValue value) {
            if (index < 0 || index > m_Length) throw new ExecutionException($"Array.Insert 索引小于0或超过最大值 index:{index} length:{m_Length}");
            if (m_Length == m_Objects.Length) {
                EnsureCapacity(m_Length + 1);
            }
            Array.Copy(m_Objects, index, m_Objects, index + 1, m_Length - index);
            m_Objects[index].CopyFrom(value);
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
            //先释放要删除的元素
            m_Objects[index].Free();
            m_Length--;
            //复制后续元素
            Array.Copy(m_Objects, index + 1, m_Objects, index, m_Length - index);
            //不能调用SetNull 否则可能会调用一次释放
            m_Objects[m_Length] = ScriptValue.Null;
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
            if (length < 0)  throw new ExecutionException($"Array.Resize长度小于0:{length}");
            if (length > m_Length) {
                EnsureCapacity(length);
                m_Length = length;
            } else {
                for (var i = length; i < m_Length; ++i) {
                    m_Objects[i].Free();
                }
                m_Length = length;
            }
        }
        public void Clear() {
            if (m_Length > 0) {
                ScorpioUtil.Free(m_Objects, m_Length);
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
            using var ret = m_Objects[--m_Length];
            return ret;
        }
        public ScriptValue SafePopLast() {
            if (m_Length == 0)
                return ScriptValue.Null;
            using var ret = m_Objects[--m_Length];
            return ret;
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
            var ret = m_Script.NewArray();
            ret.m_Length = m_Length;
            ret.EnsureCapacity(m_Length);
            if (deep) {
                for (int i = 0; i < m_Length; ++i) {
                    var value = m_Objects[i];
                    if (value.valueType == ScriptValue.scriptValueType) {
                        var scriptObject = value.scriptValue;
                        if (scriptObject != this && (scriptObject is ScriptArray || scriptObject is ScriptMap)) {
                            ret.m_Objects[i] = new ScriptValue(scriptObject.Clone(true));
                        } else {
                            ret.m_Objects[i].CopyFrom(value);
                        }
                    } else {
                        ret.m_Objects[i].CopyFrom(value);
                    }
                }
            } else {
                for (int i = 0; i < m_Length; ++i) {
                    ret.m_Objects[i].CopyFrom(m_Objects[i]);
                }
            }
            return ret;
        }
        public ScriptArray NewCopy() {
            var ret = m_Script.NewArray();
            ret.m_Length = m_Length;
            ret.EnsureCapacity(m_Length);
            for (int i = 0; i < m_Length; ++i) {
                ret.m_Objects[i].CopyFrom(m_Objects[i]);
            }
            return ret;
        }
        public override string ToString() {
            return m_Script.ToJson(this);
        }
        internal override void ToJson(ScorpioJsonSerializer jsonSerializer) {
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
