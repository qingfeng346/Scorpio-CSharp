using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Collections
{
    /// <summary> 只读list类 </summary>
    public class ReadOnlyICollection<T> : ICollection<T>
    {
        /// <summary> Returned a read only wrapper around the collectionToWrap. </summary>
        public static ReadOnlyICollection<T> AsReadOnly(ICollection<T> collectionToWrap)
        {
            return new ReadOnlyICollection<T>(collectionToWrap);
        }
        private ICollection<T> m_collection;
        /// <summary> 构造函数 </summary>
        public ReadOnlyICollection(ICollection<T> collectionToWrap) { m_collection = collectionToWrap; }
        /// <summary> 屏蔽Add </summary>
        public void Add(T item) { }
        /// <summary> 屏蔽Remove </summary>
        public bool Remove(T item) { return false; }
        /// <summary> 屏蔽Clear </summary>
        public void Clear() { }
        /// <summary> 是否包含某值 </summary>
        public bool Contains(T item) { return m_collection.Contains(item); }
        /// <summary> 拷贝到数组 </summary>
        public void CopyTo(T[] array, int arrayIndex) { m_collection.CopyTo(array, arrayIndex); }
        /// <summary> 是否只读 </summary>
        public bool IsReadOnly { get { return true; } }
        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        IEnumerator IEnumerable.GetEnumerator() { return m_collection.GetEnumerator(); }
        /// <summary> Returns an enumerator that iterates through the collection. </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return m_collection.GetEnumerator();
        }
        /// <summary> 返回数量 </summary>
        public int Count { get { return m_collection.Count; } }
    }
}
