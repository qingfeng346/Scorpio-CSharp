using System;
using System.Collections;
using System.Collections.Generic;
using Scorpio.Exception;
namespace Scorpio.Tools {
    public class ScorpioStringDictionary<Value> : IEnumerable<KeyValuePair<string, Value>> {
        private readonly static Value[] VALUE_EMPTY = new Value[0];
        public struct Enumerator : IEnumerator<KeyValuePair<string, Value>> {
            private readonly int length;
            private ScorpioStringDictionary<Value> dictionary;
            private int index;
            private KeyValuePair<string, Value> current;
            internal Enumerator(ScorpioStringDictionary<Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.Count;
                this.current = default;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = new KeyValuePair<string, Value>(dictionary.mKeys[index], dictionary.mValues[index]);
                    index++;
                    return true;
                }
                return false;
            }
            public KeyValuePair<string, Value> Current => current;
            object System.Collections.IEnumerator.Current => this.current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() {
                dictionary = null;
                current = default;
            }
        }
        protected int mSize;
        protected string[] mKeys;
        protected Value[] mValues;
        public ScorpioStringDictionary() {
            mSize = 0;
            mKeys = ScorpioUtil.KEY_EMPTY;
            mValues = VALUE_EMPTY;
        }
        public int Count => mSize;
        public IEnumerator<KeyValuePair<string, Value>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
        public void SetCapacity(int capacity) {
            if (capacity > mSize) {
                SetCapacity_impl(capacity);
            } else if (capacity < mSize) {
                throw new ExecutionException($"Capacity 不能小于当前size,  size:{mSize} capacity:{capacity}");
            }
        }
        private void SetCapacity_impl(int capacity) {
            var keyArray = new string[capacity];
            var valueArray = new Value[capacity];
            if (mSize > 0) {
                Array.Copy(mKeys, 0, keyArray, 0, mSize);
                Array.Copy(mValues, 0, valueArray, 0, mSize);
            }
            mKeys = keyArray;
            mValues = valueArray;
        }
        public void Add(string key, Value value) {
            this[key] = value;
        }
        public int IndexOf(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mKeys[i].Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
        public int IndexOfValue(Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Equals(value)) {
                    return i;
                }
            }
            return -1;
        }
        public bool ContainsKey(string key) {
            return IndexOf(key) > -1;
        }
        public bool ContainsValue(Value value) {
            return IndexOfValue(value) > -1;
        }
        public bool Remove(string key) {
            int index = IndexOf(key);
            if (index < 0) { return false; }
            mSize--;
            if (index < mSize) {
                mKeys[index] = mKeys[mSize];
                mValues[index] = mValues[mSize];
            }
            mKeys[mSize] = null;
            mValues[mSize] = default;
            return true;
        }
        public virtual void Clear() {
            mSize = 0;
            mKeys = ScorpioUtil.KEY_EMPTY;
            mValues = VALUE_EMPTY;
        }
        public bool TryGetValue(string key, out Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mKeys[i].Equals(key)) {
                    value = mValues[i];
                    return true;
                }
            }
            value = default;
            return false;
        }
        public string[] Keys {
            get {
                var keys = new string[mSize];
                Array.Copy(mKeys, 0, keys, 0, mSize);
                return keys;
            }
        }
        public Value[] Values {
            get {
                var values = new Value[mSize];
                Array.Copy(mValues, 0, values, 0, mSize);
                return values;
            }
        }
        public Value this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mKeys[i].Equals(key)) {
                        return mValues[i];
                    }
                }
                return default;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mKeys[i].Equals(key)) {
                        mValues[i] = value;
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    SetCapacity_impl(mSize == 0 ? 4 : mSize * 2);
                }
                mKeys[mSize] = key;
                mValues[mSize++] = value;
            }
        }
    }
}
