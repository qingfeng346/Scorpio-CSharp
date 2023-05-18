using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ScorpioPollingDictionary<Key, Value> : IEnumerable<ScorpioKeyValue<Key, Value>> {
        private static readonly ScorpioKeyValue<Key, Value>[] EMPTY = new ScorpioKeyValue<Key, Value>[0];
        public struct Enumerator : IEnumerator<ScorpioKeyValue<Key, Value>> {
            private readonly int length;
            private readonly ScorpioPollingDictionary<Key, Value> dictionary;
            private int index;
            private ScorpioKeyValue<Key, Value> current;
            internal Enumerator(ScorpioPollingDictionary<Key, Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.Count;
                this.current = default;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = dictionary.mValues[index];
                    index++;
                    return true;
                }
                return false;
            }
            public ScorpioKeyValue<Key, Value> Current => current;
            object System.Collections.IEnumerator.Current => current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() { }
        }
        protected int mSize;
        protected ScorpioKeyValue<Key, Value>[] mValues;
        public ScorpioPollingDictionary() : this(0) { }
        public ScorpioPollingDictionary(int capacity) {
            mSize = 0;
            SetCapacity(capacity);
        }
        public IEnumerator<ScorpioKeyValue<Key, Value>> GetEnumerator() => new Enumerator(this);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new Enumerator(this);
        protected void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScorpioKeyValue<Key, Value>[value];
                if (mSize > 0) {
                    Array.Copy(mValues, 0, array, 0, mSize);
                }
                mValues = array;
            } else {
                mValues = EMPTY;
            }
        }
        protected void EnsureCapacity(int min) {
            if (mValues.Length < min) {
                int num = mValues.Length + 8;
                if (num > 2146435071) { num = 2146435071; }
                if (num < min) { num = min; }
                SetCapacity(num);
            }
        }
        public int Count => mSize;
        public virtual void Add(Key key, Value value) {
            this[key] = value;
        }
        public virtual int IndexOf(Key key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key.Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
        public virtual int IndexOfValue(Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Value.Equals(value)) {
                    return i;
                }
            }
            return -1;
        }
        public virtual bool ContainsKey(Key key) {
            return IndexOf(key) > -1;
        }
        public virtual bool ContainsValue(Value value) {
            return IndexOfValue(value) > -1;
        }
        public virtual bool Remove(Key key) {
            int index = IndexOf(key);
            if (index < 0) { return false; }
            mSize--;
            if (index < mSize) {
                Array.Copy(mValues, index + 1, mValues, index, mSize - index);
            }
            mValues[mSize].Value = default;
            return true;
        }
        public virtual void Clear() {
            mSize = 0;
            mValues = EMPTY;
        }
        public virtual void TrimCapacity() {
            if (mSize == mValues.Length) return;
            SetCapacity(mSize);
        }
        public virtual bool SetValue(Key key, Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key.Equals(key)) {
                    mValues[i].Value = value;
                    return true;
                }
            }
            return false;
        }
        public virtual bool TryGetValue(Key key, out Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key.Equals(key)) {
                    value = mValues[i].Value;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public virtual ScorpioKeyValue<Key, Value> Get(int index) {
            return mValues[index];
        }
        public virtual Key GetKey(Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Value.Equals(value)) {
                    return mValues[i].Key;
                }
            }
            return default;
        }
        public Key[] Keys {
            get {
                var keys = new Key[mSize];
                for (int i = 0; i < mSize; ++i) {
                    keys[i] = mValues[i].Key;
                }
                return keys;
            }
        }
        public Value[] Values {
            get {
                var values = new Value[mSize];
                for (int i = 0; i < mSize; ++i) {
                    values[i] = mValues[i].Value;
                }
                return values;
            }
        }
        public virtual Value this[Key key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].Key.Equals(key)) {
                        return mValues[i].Value;
                    }
                }
                return default;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].Key.Equals(key)) {
                        mValues[i].Value = value;
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    EnsureCapacity(mSize + 1);
                }
                mValues[mSize++] = new ScorpioKeyValue<Key, Value>() { Key = key, Value = value };
            }
        }
    }
}
