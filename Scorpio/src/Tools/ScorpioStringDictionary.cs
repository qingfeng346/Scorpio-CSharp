using System.Collections.Generic;
using System;
using System.Collections;
using Scorpio.Exception;

namespace Scorpio.Tools {
    public class ScorpioStringDictionary : IEnumerable<KeyValuePair<string, ScriptValue>> {
        public struct Enumerator : IEnumerator<KeyValuePair<string, ScriptValue>>, IEnumerator {
            private int length;
            private int index;
            private ScorpioStringDictionary dictionary;
            private KeyValuePair<string, ScriptValue> current;
            internal Enumerator(ScorpioStringDictionary dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.Count;
                this.current = default;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = new KeyValuePair<string, ScriptValue>(dictionary.mKeys[index], dictionary.mValues[index]);
                    index++;
                    return true;
                }
                current = default;
                return false;
            }
            public KeyValuePair<string, ScriptValue> Current => current;
            object IEnumerator.Current => current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() {
                dictionary = null;
                current = default;
            }
        }
        public int mSize;
        public string[] mKeys;
        public ScriptValue[] mValues;
        public ScorpioStringDictionary() {
            mSize = 0;
            mKeys = ScorpioUtil.KEY_EMPTY;
            mValues = ScorpioUtil.VALUE_EMPTY;
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
        public void SetCapacity(int capacity) {
            if (capacity > mSize) {
                SetCapacity_impl(capacity);
            } else if (capacity < mSize) {
                throw new ExecutionException($"Capacity 不能小于当前size,  size:{mSize} capacity:{capacity}");
            }
        }
        private void SetCapacity_impl(int value) {
            var keyArray = new string[value];
            var valueArray = new ScriptValue[value];
            if (mSize > 0) {
                Array.Copy(mKeys, 0, keyArray, 0, mSize);
                Array.Copy(mValues, 0, valueArray, 0, mSize);
            }
            mKeys = keyArray;
            mValues = valueArray;
        }
        public int Count => mSize;
        public virtual void Add(string key, ScriptValue value) {
            this[key] = value;
        }
        public virtual int IndexOf(string index) {
            for (int i = 0; i < mSize; ++i) {
                if (mKeys[i] == index) {
                    return i;
                }
            }
            return -1;
        }
        public virtual int IndexOfValue(ScriptValue value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].Equals(value)) {
                    return i;
                }
            }
            return -1;
        }
        public virtual bool ContainsKey(string key) {
            return IndexOf(key) > -1;
        }
        public virtual bool ContainsValue(ScriptValue value) {
            return IndexOfValue(value) > -1;
        }
        public virtual bool Remove(string key) {
            int index = IndexOf(key);
            if (index < 0) { return false; }
            mSize--;
            mValues[index].Free();
            if (index < mSize) {
                mKeys[index] = mKeys[mSize];
                mValues[index] = mValues[mSize];
            }
            mKeys[mSize] = null;
            mValues[mSize] = default;
            return true;
        }
        public virtual void Clear() {
            ScorpioUtil.Free(mValues, mSize);
            mValues = ScorpioUtil.VALUE_EMPTY;
            mKeys = ScorpioUtil.KEY_EMPTY;
            mSize = 0;
        }
        public virtual bool TryGetValue(string key, out ScriptValue value) {
            for (int i = 0; i < mSize; ++i) {
                if (mKeys[i] == key) {
                    value = mValues[i];
                    return true;
                }
            }
            value = default;
            return false;
        }
        public void SetValue(string key, ScriptValue value) {
            for (int i = 0; i < mSize; ++i) {
                if (mKeys[i] == key) {
                    mValues[i].Set(value);
                    return;
                }
            }
            if (mSize == mValues.Length) {
                var keyArray = new string[mSize + 8];
                var valueArray = new ScriptValue[mSize + 8];
                if (mSize > 0) {
                    Array.Copy(mKeys, 0, keyArray, 0, mSize);
                    Array.Copy(mValues, 0, valueArray, 0, mSize);
                }
                mKeys = keyArray;
                mValues = valueArray;
            }
            mKeys[mSize] = key;
            mValues[mSize++].Set(value);
        }
        public virtual ScriptValue this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mKeys[i] == key) {
                        return mValues[i];
                    }
                }
                return default;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mKeys[i] == key) {
                        mValues[i].CopyFrom(value);
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    var keyArray = new string[mSize + 8];
                    var valueArray = new ScriptValue[mSize + 8];
                    if (mSize > 0) {
                        Array.Copy(mKeys, 0, keyArray, 0, mSize);
                        Array.Copy(mValues, 0, valueArray, 0, mSize);
                    }
                    mKeys = keyArray;
                    mValues = valueArray;
                }
                mKeys[mSize] = key;
                mValues[mSize++].CopyFrom(value);
            }
        }
    }
}
