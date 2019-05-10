using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.Commons {
    public class ScorpioValue<Key,Value> {
        public Key key;
        public Value value;
        public ScorpioValue<Key, Value> Set(ScorpioValue<Key, Value> v) {
            this.key = v.key;
            this.value = v.value;
            return this;
        }
    }
    public class ScorpioDictionary<Key, Value> {
        private static readonly ScorpioValue<Key, Value>[] EmptyArray = new ScorpioValue<Key, Value>[0];
        protected int mSize;
        protected ScorpioValue<Key, Value>[] mValues;
        public ScorpioDictionary() {
            mValues = EmptyArray;
            mSize = 0;
        }
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
        public int Count { get { return mSize; } }
        public virtual int IndexOf(Key key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
        public virtual bool ContainsKey(Key key) {
            return IndexOf(key) > -1;
        }
        public virtual bool Remove(Key key) {
            int index = IndexOf(key);
            if (index < 0) { return false; }
            mSize--;
            if (index < mSize) {
                Array.Copy(mValues, index + 1, mValues, index, mSize - index);
            }
            mValues[mSize].value = default(Value);
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
        public virtual Value this[Key key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key.Equals(key)) {
                        return mValues[i].value;
                    }
                }
                return default(Value);
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
                    mValues[mSize].value = default(Value);
                    return true;
                }
            }
            return false;
        }
        public override Value this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        return mValues[i].value;
                    }
                }
                return default(Value);
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
}
