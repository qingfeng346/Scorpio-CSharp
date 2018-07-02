using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.Commons {
    public class ScorpioDictionary<T> {
        public class Value {
            public string key;
            public T value;
        }
        private static readonly Value[] _emptyArray = new Value[0];
        private int mSize;
        private Value[] mValues;
        public ScorpioDictionary() {
            mValues = _emptyArray;
            mSize = 0;
        }
        public void Set(ScorpioDictionary<T> value) {
            mSize = value.mSize;
            mValues = new Value[mSize];
            for (var i = 0; i < mSize; ++i) {
                mValues[i] = value.mValues[i];
            }
        }
        void SetCapacity(int value) {
            if (value > 0) {
                Value[] array = new Value[value];
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
        public int IndexOf(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    return i;
                }
            }
            return -1;
        }
        public bool ContainsKey(string key) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    return true;
                }
            }
            return false;
        }
        public int Count {
            get { return mSize; }
        }
        public bool SetValue(string key, T value) {
            for (int i = 0; i < mSize; ++i) {
                if (mValues[i].key == key) {
                    mValues[i].value = value;
                    return true;
                }
            }
            return false;
        }

        public T this[string key] {
            get {
                for (int i = 0; i < mSize; ++i) {
                    if (mValues[i].key == key) {
                        return mValues[i].value;
                    }
                }
                return default(T);
            } set {
                int index = IndexOf(key);
                if (index == -1) {
                    if (mSize == mValues.Length) {
                        EnsureCapacity(mSize + 1);
                    }
                    mValues[mSize] = new Value() { key = key, value = value };
                    ++mSize;
                } else {
                    mValues[index].value = value;
                }
            }
        }
    }
}
