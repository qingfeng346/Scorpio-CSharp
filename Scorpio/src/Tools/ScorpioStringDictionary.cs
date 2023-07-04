﻿using System.Collections.Generic;
using System;
using System.Collections;
using Scorpio.Exception;

namespace Scorpio.Tools {
    public class ScorpioStringDictionary : IEnumerable<KeyValuePair<string, ScriptValue>> {
        public static readonly string[] STRING_EMPTY = new string[0];
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
        protected int mSize;
        protected string[] mKeys;
        protected ScriptValue[] mValues;
        public ScorpioStringDictionary() : this(0) { }
        public ScorpioStringDictionary(int capacity) {
            mSize = 0;
            SetCapacity_impl(capacity);
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
            if (value > 0) {
                var keyArray = new string[value];
                var valueArray = new ScriptValue[value];
                if (mSize > 0) {
                    Array.Copy(mKeys, 0, keyArray, 0, mSize);
                    Array.Copy(mValues, 0, valueArray, 0, mSize);
                }
                mKeys = keyArray;
                mValues = valueArray;
            } else {
                mKeys = STRING_EMPTY;
                mValues = ScriptValue.EMPTY;
            }
        }
        protected void EnsureCapacity(int min) {
            if (mValues.Length < min) {
                int num = mValues.Length + 8;
                if (num > 2146435071) { num = 2146435071; }
                if (num < min) { num = min; }
                SetCapacity_impl(num);
            }
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
            if (index < mSize) {
                mKeys[index] = mKeys[mSize];
                mValues[index] = mValues[mSize];
            }
            mKeys[mSize] = null;
            mValues[mSize].Free();
            return true;
        }
        public virtual void Clear() {
            ScorpioUtil.Free(mValues, mSize);
            Array.Clear(mKeys, 0, mSize);
            mSize = 0;
        }
        public virtual void TrimCapacity() {
            if (mSize == mValues.Length) return;
            SetCapacity(mSize);
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
                EnsureCapacity(mSize + 1);
            }
            mKeys[mSize] = key;
            mValues[mSize++].Set(value);
        }
        public string[] Keys {
            get {
                var keys = new string[mSize];
                Array.Copy(mKeys, keys, mSize);
                return keys;
            }
        }
        public ScriptValue[] Values {
            get {
                var values = new ScriptValue[mSize];
                Array.Copy(mValues, values, mSize);
                return values;
            }
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
                    EnsureCapacity(mSize + 1);
                }
                mKeys[mSize] = key;
                mValues[mSize++].CopyFrom(value);
            }
        }
    }
}
