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
        private static readonly ScorpioValue<Key, Value>[] _emptyArray = new ScorpioValue<Key, Value>[0];
        private int mSize;
        private ScorpioValue<Key, Value>[] mValues;
        public ScorpioDictionary() {
            mValues = _emptyArray;
            mSize = 0;
        }
        public void Set(ScorpioDictionary<Key, Value> value) {
            mSize = value.mSize;
            mValues = new ScorpioValue<Key, Value>[mSize];
            for (var i = 0; i < mSize; ++i) {
                mValues[i] = new ScorpioValue<Key, Value>().Set(value.mValues[i]);
            }
        }
        void SetCapacity(int value) {
            if (value > 0) {
                var array = new ScorpioValue<Key, Value>[value];
                if (mSize > 0) {
                    Array.Copy(mValues, 0, array, 0, mSize);
                }
                mValues = array;
            } else {
                mValues = _emptyArray;
            }
        }
        void EnsureCapacity(int min) {
            if (mValues.Length < min) {
                int num = (mValues.Length == 0) ? 4 : (mValues.Length * 2);
                if (num > 2146435071) {
                    num = 2146435071;
                }
                if (num < min) {
                    num = min;
                }
                SetCapacity(num);
            }
        }
        public int IndexOf(Key key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    return i;
                }
            }
            return -1;
        }
        public bool ContainsKey(Key key) {
            return IndexOf(key) > -1;
        }
        public int Count { get { return mSize; } }
        public bool SetValue(Key key, Value value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key.Equals(key)) {
                    mValues[i].value = value;
                    return true;
                }
            }
            return false;
        }

        public Value this[Key key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key.Equals(key)) {
                        return mValues[i].value;
                    }
                }
                return default(Value);
            } set {
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
}
