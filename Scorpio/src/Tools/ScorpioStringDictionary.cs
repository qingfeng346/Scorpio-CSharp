using System.Collections.Generic;
using System;

namespace Scorpio.Tools {
    public class ScorpioStringDictionary<Value> : IEnumerable<ScorpioKeyValue<string, Value>> {
        private static readonly ScorpioKeyValue<uint, Value>[] EMPTY = new ScorpioKeyValue<uint, Value>[0];
        public struct Enumerator : IEnumerator<ScorpioKeyValue<string, Value>> {
            private readonly int length;
            private readonly ScorpioStringDictionary<Value> dictionary;
            private int index;
            private ScorpioKeyValue<string, Value> current;
            internal Enumerator(ScorpioStringDictionary<Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.Count;
                this.current = default;
            }
            public bool MoveNext() {
                if (index < length) {
                    var value = dictionary.mValues[index];
                    current = new ScorpioKeyValue<string, Value>(ScorpioStringUtil.IndexToString(value.Key), value.Value);
                    index++;
                    return true;
                }
                return false;
            }
            public ScorpioKeyValue<string, Value> Current => current;
            object System.Collections.IEnumerator.Current => current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() { }
        }
        protected int mSize;
        protected ScorpioKeyValue<uint, Value>[] mValues;
        public ScorpioStringDictionary() : this(0) { }
        public ScorpioStringDictionary(int capacity) {
            mSize = 0;
            SetCapacity(capacity);
        }
        public IEnumerator<ScorpioKeyValue<string, Value>> GetEnumerator() => new Enumerator(this);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new Enumerator(this);
        protected void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScorpioKeyValue<uint, Value>[value];
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
        public virtual void Add(string key, Value value) {
            this[key] = value;
        }
        public virtual int IndexOf(string key) {
            return IndexOf(ScorpioStringUtil.StringToIndex(key));
        }
        public virtual int IndexOf(uint index) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key == index) {
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
        public virtual bool ContainsKey(string key) {
            return IndexOf(key) > -1;
        }
        public virtual bool ContainsValue(Value value) {
            return IndexOfValue(value) > -1;
        }
        public virtual bool Remove(string key) {
            int index = IndexOf(key);
            if (index < 0) { return false; }
            mSize--;
            if (index < mSize) {
                mValues[index] = mValues[mSize];
            }
            mValues[mSize] = default;
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
        public virtual bool SetValue(uint key, Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key == key) {
                    mValues[i].Value = value;
                    return true;
                }
            }
            return false;
        }
        public virtual bool TryGetValue(string key, out Value value) {
            return TryGetValue(ScorpioStringUtil.StringToIndex(key), out value);
        }
        public virtual bool TryGetValue(uint key, out Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Key == key) {
                    value = mValues[i].Value;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public string[] Keys {
            get {
                var keys = new string[mSize];
                for (int i = 0; i < mSize; ++i) {
                    keys[i] = ScorpioStringUtil.IndexToString(mValues[i].Key);
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
        public virtual Value this[string key] {
            get {
                return this[ScorpioStringUtil.StringToIndex(key)];
            }
            set {
                this[ScorpioStringUtil.StringToIndex(key)] = value;
            }
        }
        public virtual Value this[uint key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].Key == key) {
                        return mValues[i].Value;
                    }
                }
                return default;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].Key == key) {
                        mValues[i].Value = value;
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    EnsureCapacity(mSize + 1);
                }
                mValues[mSize++] = new ScorpioKeyValue<uint, Value>() { Key = key, Value = value };
            }
        }
    }
}
