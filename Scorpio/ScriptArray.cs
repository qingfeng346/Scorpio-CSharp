using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本数组类型
    public class ScriptArray : ScriptObject
    {
        public struct Comparer : IComparer<ScriptObject> {
            Script script;
            ScriptFunction func;
            internal Comparer(Script script, ScriptFunction func) {
                this.script = script;
                this.func = func;
            }
            public int Compare(ScriptObject x, ScriptObject y) {
                ScriptNumber ret = func.Call(new ScriptObject[] { x, y }) as ScriptNumber;
                if (ret == null) throw new ExecutionException(script, "Sort 返回值 必须是Number类型");
                return ret.ToInt32();
            }
        }
        public struct Enumerator : System.Collections.IEnumerator {
            private ScriptArray list;
            private int index;
            private ScriptObject current;
            internal Enumerator(ScriptArray list) {
                this.list = list;
                this.index = 0;
                this.current = null;
            }
            public bool MoveNext() {
                if (index < list.m_size) {
                    current = list.m_listObject[index] ?? list.m_null;
                    index++;
                    return true;
                }
                return false;
            }
            public ScriptObject Current { get { return current; } }
            object System.Collections.IEnumerator.Current { get { return current; } }
            public void Reset() {
                index = 0;
                current = null;
            }
        }


        public override ObjectType Type { get { return ObjectType.Array; } }
        private static readonly ScriptObject[] _emptyArray = new ScriptObject[0];
        private ScriptObject[] m_listObject;
        private int m_size;
        private ScriptObject m_null;
        public ScriptArray(Script script) : base(script) {
            m_listObject = _emptyArray;
            m_size = 0;
            m_null = script.Null;
        }
        public override ScriptObject GetValue(object index)
        {
            if (index is double || index is int || index is long) {
                int i = Util.ToInt32(index);
                if (i < 0)
                    throw new ExecutionException(m_Script, "Array GetValue索引小于0 index值为:" + index);
                if (i >= m_size)
                    return m_null;
                return m_listObject[i] ?? m_null;
            } else if (index is string && index.Equals("length")){
                return m_Script.CreateNumber(m_size);
            }
            throw new ExecutionException(m_Script, "Array SetValue只支持Number类型 index值为:" + index);
        }
        public override void SetValue(object index, ScriptObject obj)
        {
            if (index is double || index is int || index is long) {
                int i = Util.ToInt32(index);
                if (i < 0)
                    throw new ExecutionException(m_Script, "Array SetValue索引小于0 index值为:" + index);
                if (i >= m_size) {
                    EnsureCapacity(i + 1);
                    m_size = i + 1;
                }
                m_listObject[i] = obj;
            } else {
                throw new ExecutionException(m_Script, "Array SetValue只支持Number类型 index值为:" + index);
            }
        }
        void SetCapacity(int value) {
            if (value > 0) {
                ScriptObject[] array = new ScriptObject[value];
                if (m_size > 0) {
                    Array.Copy(m_listObject, 0, array, 0, m_size);
                }
                m_listObject = array;
            } else {
                m_listObject = _emptyArray;
            }
        }
        void EnsureCapacity(int min) {
            if (m_listObject.Length < min) {
                int num = (m_listObject.Length == 0) ? 4 : (m_listObject.Length * 2);
                if (num > 2146435071) {
                    num = 2146435071;
                }
                if (num < min) {
                    num = min;
                }
                SetCapacity(num);
            }
        }
        public void Add(ScriptObject obj)
        {
            if (m_size == m_listObject.Length) {
                EnsureCapacity(m_size + 1);
            }
            m_listObject[m_size] = obj;
            m_size++;
        }
        public void Insert(int index, ScriptObject obj)
        {
            if (m_size == m_listObject.Length) {
                EnsureCapacity(m_size + 1);
            }
            if (index < m_size) {
                Array.Copy(m_listObject, index, m_listObject, index + 1, m_size - index);
            }
            m_listObject[index] = obj;
            m_size++;
        }
        public bool Remove(ScriptObject obj)
        {
            int num = IndexOf(obj);
            if (num >= 0) {
                RemoveAt(num);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index)
        {
            m_size--;
            if (index < m_size) {
                Array.Copy(m_listObject, index + 1, m_listObject, index, m_size - index);
            }
            m_listObject[m_size] = null;
        }
        public bool Contains(ScriptObject obj)
        {
            for (int i = 0;i < m_size; ++i) {
                if (obj.Equals(m_listObject[i])) {
                    return true;
                }
            }
            return false;
        }
        public int IndexOf(ScriptObject obj)
        {
            for (int i = 0; i < m_size; ++i) {
                if (obj.Equals(m_listObject[i])) {
                    return i;
                }
            }
            return -1;
        }
        public int LastIndexOf(ScriptObject obj)
        {
            for (int i = m_size - 1; i >= 0; --i) {
                if (obj.Equals(m_listObject[i])) {
                    return i;
                }
            }
            return -1;
        }
        public void Resize(int length) {
            if (length < 0)
                throw new ExecutionException(m_Script, "Resize长度小于0 length:" + length);
            if (length > m_size) {
                EnsureCapacity(length);
                m_size = length;
            } else {
                Array.Clear(m_listObject, length, m_size - length);
                m_size = length;
            }
        }
        public void Clear() {
            if (m_size > 0) {
                Array.Clear(m_listObject, 0, m_size);
                m_size = 0;
            }
        }
        public int Count() {
            return m_size;
        }
		public void Sort(ScriptFunction func) {
            Array.Sort<ScriptObject>(m_listObject, 0, m_size, new Comparer(m_Script, func));
        }
        public ScriptObject First()
        {
            if (m_size > 0)
                return m_listObject[0];
            return m_null;
        }
        public ScriptObject Last()
        {
            if (m_size > 0)
                return m_listObject[m_size - 1];
            return m_null;
        }
        public ScriptObject PopFirst()
        {
            if (m_size == 0)
                throw new ExecutionException(m_Script, "Array Pop 数组长度为0");
            ScriptObject obj = m_listObject[0];
            RemoveAt(0);
            return obj;
        }
        public ScriptObject SafePopFirst()
        {
            if (m_size == 0)
                return m_null;
            ScriptObject obj = m_listObject[0];
            RemoveAt(0);
            return obj;
        }
        public ScriptObject PopLast()
        {
            if (m_size == 0)
                throw new ExecutionException(m_Script, "Array Pop 数组长度为0");
            int index = m_size - 1;
            ScriptObject obj = m_listObject[index];
            RemoveAt(index);
            return obj;
        }
        public ScriptObject SafePopLast()
        {
            if (m_size == 0)
                return m_null;
            int index = m_size - 1;
            ScriptObject obj = m_listObject[index];
            RemoveAt(index);
            return obj;
        }

        public Enumerator GetIterator()
        {
            return new Enumerator(this);
        }
        public ScriptObject[] ToArray()
        {
            ScriptObject[] array = new ScriptObject[m_size];
            Array.Copy(m_listObject, 0, array, 0, m_size);
            return array;
        }
        public override ScriptObject Clone()
        {
            ScriptArray ret = m_Script.CreateArray();
            ret.m_listObject = new ScriptObject[m_size];
            ret.m_size = m_size;
            for (int i = 0; i < m_size; ++i) {
                if (m_listObject[i] == this) {
                    ret.m_listObject[i] = ret;
                } else if (m_listObject[i] == null) {
                    ret.m_listObject[i] = m_null;
                } else {
                    ret.m_listObject[i] = m_listObject[i].Clone();
                }
            }
            return ret;
        }
        public override string ToString() { return "Array"; }
        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            for (int i = 0; i < m_size; ++i) {
                if (i != 0) builder.Append(",");
                if (m_listObject[i] == null) {
                    builder.Append(m_null.ToJson());
                } else {
                    builder.Append(m_listObject[i].ToJson());
                }
            }
            builder.Append("]");
            return builder.ToString();
        }
    }
}
