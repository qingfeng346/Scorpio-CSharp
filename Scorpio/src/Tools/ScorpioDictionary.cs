﻿using System;
using System.Collections.Generic;
namespace Scorpio.Tools {
    public class ScorpioValue<Key,Value> {
        public Key key;
        public Value value;
        public ScorpioValue<Key, Value> Set(ScorpioValue<Key, Value> v) {
            this.key = v.key;
            this.value = v.value;
            return this;
        }
    }
    public class ScorpioDictionary<Key, Value> : IEnumerable<ScorpioValue<Key, Value>> {
        public struct Enumerator : IEnumerator<ScorpioValue<Key, Value>> {
            private readonly int length;
            private readonly ScorpioDictionary<Key, Value> dictionary;
            private int index;
            private ScorpioValue<Key, Value> current;
            internal Enumerator(ScorpioDictionary<Key, Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.Count;
                this.current = null;
            }
            public bool MoveNext() {
                if (index < length) {
                    current = dictionary.mValues[index];
                    index++;
                    return true;
                }
                return false;
            }
            public ScorpioValue<Key, Value> Current { get { return current; } }
            object System.Collections.IEnumerator.Current { get { return current; } }
            public void Reset() {
                index = 0;
                current = null;
            }
            public void Dispose() { }
        }
        private static readonly ScorpioValue<Key, Value>[] EmptyArray = new ScorpioValue<Key, Value>[0];
        protected int mSize;
        protected ScorpioValue<Key, Value>[] mValues;
        public ScorpioDictionary() {
            mValues = EmptyArray;
            mSize = 0;
        }
        public IEnumerator<ScorpioValue<Key, Value>> GetEnumerator() { return new Enumerator(this); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return new Enumerator(this); }
        void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScorpioValue<Key, Value>[value];
                if (mSize > 0) {
                    Array.Copy(mValues, 0, array, 0, mSize);
                }
                mValues = array;
            } else {
                mValues = EmptyArray;
            }
        }
        protected void EnsureCapacity(int min) {
            if (mValues.Length < min) {
                int num = (mValues.Length == 0) ? 4 : (mValues.Length * 2);
                if (num > 2146435071) { num = 2146435071; }
                if (num < min) { num = min; }
                SetCapacity(num);
            }
        }
        public int Count => mSize;
        public virtual int IndexOf(Key key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
        public virtual int IndexOfValue(Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].value.Equals(value)) {
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
            mValues[mSize].value = default;
            return true;
        }
        public virtual bool SetValue(Key key, Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    mValues[i].value = value;
                    return true;
                }
            }
            return false;
        }
        public virtual bool TryGetValue(Key key, out Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    value = mValues[i].value;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public virtual ScorpioValue<Key, Value> Get(int index) {
            return mValues[index];
        }
        public virtual Key GetKey(Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].value.Equals(value)) {
                    return mValues[i].key;
                }
            }
            return default(Key);
        }
        public Key[] Keys {
            get {
                var keys = new Key[mSize];
                for (int i = 0; i < mSize; ++i) {
                    keys[i] = mValues[i].key;
                }
                return keys;
            }
        }
        public Value[] Values {
            get {
                var values = new Value[mSize];
                for (int i = 0; i < mSize; ++i) {
                    values[i] = mValues[i].value;
                }
                return values;
            }
        }
        public virtual Value this[Key key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key.Equals(key)) {
                        return mValues[i].value;
                    }
                }
                return default;
            }
            set {
                int index = IndexOf(key);
                if (index == -1) {
                    if (mSize == mValues.Length) {
                        EnsureCapacity(mSize + 1);
                    }
                    mValues[mSize] = new ScorpioValue<Key, Value>() { key = key, value = value };
                    ++mSize;
                } else {
                    mValues[index].value = value;
                }
            }
        }
    }
    public class ScorpioDictionaryString<Value> : ScorpioDictionary<string, Value> {
        public override int IndexOf(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    return i;
                }
            }
            return -1;
        }
        public override bool ContainsKey(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    return true;
                }
            }
            return false;
        }
        public override bool Remove(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    mSize--;
                    if (i < mSize) {
                        Array.Copy(mValues, i + 1, mValues, i, mSize - i);
                    }
                    mValues[mSize].value = default;
                    return true;
                }
            }
            return false;
        }
        public override bool TryGetValue(string key, out Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    value = mValues[i].value;
                    return true;
                }
            }
            value = default;
            return false;
        }
        public override Value this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        return mValues[i].value;
                    }
                }
                return default;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        mValues[i].value = value;
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    EnsureCapacity(mSize + 1);
                }
                mValues[mSize] = new ScorpioValue<string, Value>() { key = key, value = value };
                ++mSize;
            }
        }
    }
    public class ScorpioDictionaryStringValue : ScorpioDictionary<string, ScriptValue> {
        public override bool TryGetValue(string key, out ScriptValue value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    value = mValues[i].value;
                    return true;
                }
            }
            value = ScriptValue.Null;
            return false;
        }
        public override ScriptValue this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        return mValues[i].value;
                    }
                }
                return ScriptValue.Null;
            }
            set {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        mValues[i].value = value;
                        return;
                    }
                }
                if (mSize == mValues.Length) {
                    EnsureCapacity(mSize + 1);
                }
                mValues[mSize] = new ScorpioValue<string, ScriptValue>() { key = key, value = value };
                ++mSize;
            }
        }
    }
}
