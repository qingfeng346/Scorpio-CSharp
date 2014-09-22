using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Scorpio.Collections
{
    /// <summary> 只读map类 </summary>
    public class ReadOnlyDictionary<TKey, TValue>
        : IDictionary<TKey, TValue>
        , ICollection<KeyValuePair<TKey, TValue>>
        , IEnumerable<KeyValuePair<TKey, TValue>>
        , IDictionary
        , ICollection
        , IEnumerable
        , ISerializable
        , IDeserializationCallback
    {
        /// <summary> Returns a read only dictionary. </summary>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly(IDictionary<TKey, TValue> dictionaryToWrap)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dictionaryToWrap);
        }
        private IDictionary<TKey, TValue> m_dictionaryTyped;
        private IDictionary m_dictionary;
        /// <summary> 构造函数 </summary>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionaryToWrap)
        {
            m_dictionaryTyped = dictionaryToWrap;
            m_dictionary = (IDictionary)m_dictionaryTyped;
        }
        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
        /// <summary> Returns an  object for the object. </summary>
        IDictionaryEnumerator IDictionary.GetEnumerator() { return m_dictionary.GetEnumerator(); }
        /// <summary> Returns an enumerator that iterates through the collection. </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return m_dictionaryTyped.GetEnumerator(); }
        /// <summary> 返回Key值集合 </summary>
        ICollection IDictionary.Keys { get { return m_dictionary.Keys; } }
        /// <summary> 返回Key值只读集合 </summary>
        public ICollection<TKey> Keys { get { return ReadOnlyICollection<TKey>.AsReadOnly(m_dictionaryTyped.Keys); } }
        /// <summary> 返回Value值集合 </summary>
        ICollection IDictionary.Values { get { return m_dictionary.Values; } }
        /// <summary> 返回Value值只读集合 </summary>
        public ICollection<TValue> Values { get { return ReadOnlyICollection<TValue>.AsReadOnly(m_dictionaryTyped.Values); } }

        /// <summary> 屏蔽Add </summary>
        public void Add(TKey key, TValue value) { }
        /// <summary> 屏蔽Add </summary>
        public void Add(KeyValuePair<TKey, TValue> item) { }
        /// <summary> 屏蔽Add </summary>
        public void Add(object key, object value) { }
        /// <summary> 屏蔽Remove </summary>
        public bool Remove(TKey key) { return false; }
        /// <summary> 屏蔽Remove </summary>
        public void Remove(object key) { }
        /// <summary> 屏蔽Remove </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item) { return false; }
        /// <summary> 屏蔽Clear </summary>
        public void Clear() { }
        /// <summary> 是否包含某Key </summary>
        public bool ContainsKey(TKey key) { return m_dictionaryTyped.ContainsKey(key); }
        /// <summary> 是否包含某Key </summary>
        public bool Contains(object key) { return m_dictionary.Contains(key); }
        /// <summary> 是否包含某值 </summary>
        public bool Contains(KeyValuePair<TKey, TValue> item) { return m_dictionaryTyped.Contains(item); }
        /// <summary> 尝试获得某值 </summary>
        public bool TryGetValue(TKey key, out TValue value) { return m_dictionaryTyped.TryGetValue(key, out value); }
        /// <summary> 拷贝到数组 </summary>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { m_dictionaryTyped.CopyTo(array, arrayIndex); }
        /// <summary> Gets the <see cref="System.Object"/> with the specified key. Set does not affect a ReadOnlyDictionary </summary>
        public object this[object key] { get { return m_dictionary[key]; } set { } }
        /// <summary> Gets the <see typeparamref="TValue"/> with the specified key. Set does not change a read only Dictionary </summary>
        public TValue this[TKey key] { get { return m_dictionaryTyped[key]; } set { } }
        /// <summary> Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index. </summary>
        public void CopyTo(Array array, int index) { }
        /// <summary> Runs when the entire object graph has been deserialized. </summary>
        public void OnDeserialization(object sender)
        {
            IDeserializationCallback callback = m_dictionaryTyped as IDeserializationCallback;
            callback.OnDeserialization(sender);
        }
        /// <summary> Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object. </summary>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ISerializable serializable = m_dictionaryTyped as ISerializable;
            serializable.GetObjectData(info, context);
        }
        /// <summary> 返回数量 </summary>
        public int Count { get { return m_dictionaryTyped.Count; } }
        /// <summary> 是否只读 </summary>
        public bool IsReadOnly { get { return true; } }
        /// <summary> Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size. </summary>
        public bool IsFixedSize { get { return m_dictionary.IsFixedSize; } }
        /// <summary> Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe). </summary>
        public bool IsSynchronized { get { return m_dictionary.IsSynchronized; } }
        /// <summary> Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>. </summary>
        public object SyncRoot { get { return m_dictionary.SyncRoot; } }
    }
}
