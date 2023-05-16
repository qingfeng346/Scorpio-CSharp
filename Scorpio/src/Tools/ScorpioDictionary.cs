using System;
using System.Collections;
using System.Collections.Generic;

namespace Scorpio {
    public struct ScorpioKeyValue<TKey, TValue> {
        public TKey Key;
        public TValue Value;
        public ScorpioKeyValue(TKey key, TValue value) {
            Key = key;
            Value = value;
        }
    }
    public abstract class ScorpioDictionary<Key, Value> {
        private static Value[] EMPTY = new Value[0];
        protected uint id;
        protected int size = 0;
        protected Value[] values;
        public ScorpioDictionary() {
            this.size = 0;
            this.values = EMPTY;
        }
        public ScorpioDictionary(int capacity) : this() {
            SetCapacity(capacity);
        }
        protected void SetCapacity(int value) {
            if (value > 0) {
                var array = new Value[value];
                if (size > 0) {
                    Array.Copy(values, 0, array, 0, size);
                }
                values = array;
            } else {
                values = EMPTY;
            }
        }
        protected void EnsureCapacity(int min) {
            if (values.Length < min) {
                int num = values.Length + 4;
                if (num < min) { num = min; }
                SetCapacity(num);
            }
        }
        public void TrimCapacity() {
            if (size == values.Length) return;
            SetCapacity(size);
        }
        public int Count => size;
        public void Clear() {
            size = 0;
            Array.Clear(values, 0, values.Length);
        }
        public bool ContainsValue(Value value) {
            return Array.IndexOf(values, value, 0, size) >= 0;
        }
    }
    public class ScorpioStringDictionary<Value> : ScorpioDictionary<string, Value>, IEnumerable<ScorpioKeyValue<string, Value>> {
        public struct Enumerator : IEnumerator<ScorpioKeyValue<string, Value>> {
            private readonly int length;
            private readonly ScorpioStringDictionary<Value> dictionary;
            private int index;
            private ScorpioKeyValue<string, Value> current;
            private string[] keys;
            internal Enumerator(ScorpioStringDictionary<Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.size;
                this.current = default;
                this.keys = ScorpioVariable.GetStringKeys(dictionary.id);
            }
            public bool MoveNext() {
                if (index < length) {
                    current = new ScorpioKeyValue<string, Value>() { Key = keys[index], Value = dictionary[keys[index]] };
                    index++;
                    return true;
                }
                return false;
            }
            public ScorpioKeyValue<string, Value> Current => current;
            object IEnumerator.Current => this.current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() { }
        }
        public ScorpioStringDictionary() : base() {
            this.id = ScorpioVariable.StringId;
        }
        public ScorpioStringDictionary(int capacity) : base(capacity) {
            this.id = ScorpioVariable.StringId;
        }
        ~ScorpioStringDictionary() {
            ScorpioVariable.ReleaseStringId(id);
        }
        public virtual IEnumerator<ScorpioKeyValue<string, Value>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public virtual bool ContainsKey(string key) {
            return ScorpioVariable.ContainsVariable(id, key);
        }
        public virtual bool TryGetValue(string key, out Value value) {
            if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                value = values[index];
                return true;
            }
            value = default;
            return false;
        }
        public Value this[string key] {
            get {
                if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                    return values[index];
                }
                return default;
            }
            set {
                if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                    values[index] = value;
                    return;
                }
                index = ScorpioVariable.AddVariable(id, key);
                if (index >= values.Length) {
                    EnsureCapacity(index + 1);
                }
                values[index] = value;
                ++size;
            }
        }
    }
    public class ScorpioObjectDictionary<Value> : ScorpioDictionary<object, Value>, IEnumerable<ScorpioKeyValue<object, Value>> {
        public struct Enumerator : IEnumerator<ScorpioKeyValue<object, Value>> {
            private readonly int length;
            private readonly ScorpioObjectDictionary<Value> dictionary;
            private int index;
            private ScorpioKeyValue<object, Value> current;
            private object[] keys;
            internal Enumerator(ScorpioObjectDictionary<Value> dictionary) {
                this.dictionary = dictionary;
                this.index = 0;
                this.length = dictionary.size;
                this.current = default;
                this.keys = ScorpioVariable.GetObjectKeys(dictionary.id);
            }
            public bool MoveNext() {
                if (index < length) {
                    current = new ScorpioKeyValue<object, Value>() { Key = keys[index], Value = dictionary[keys[index]] };
                    index++;
                    return true;
                }
                return false;
            }
            public ScorpioKeyValue<object, Value> Current => current;
            object IEnumerator.Current => this.current;
            public void Reset() {
                index = 0;
                current = default;
            }
            public void Dispose() { }
        }
        public ScorpioObjectDictionary() : base() {
            this.id = ScorpioVariable.ObjectId;
        }
        public ScorpioObjectDictionary(int capacity) : base(capacity) {
            this.id = ScorpioVariable.ObjectId;
        }
        ~ScorpioObjectDictionary() {
            ScorpioVariable.ReleaseObjectId(id);
        }
        public IEnumerator<ScorpioKeyValue<object, Value>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
        public bool ContainsKey(object key) {
            return ScorpioVariable.ContainsVariable(id, key);
        }
        public bool TryGetValue(object key, out Value value) {
            if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                value = values[index];
                return true;
            }
            value = default;
            return false;
        }
        public Value this[object key] {
            get {
                if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                    return values[index];
                }
                return default;
            }
            set {
                if (ScorpioVariable.TryGetVariable(id, key, out var index)) {
                    values[index] = value;
                    return;
                }
                index = ScorpioVariable.AddVariable(id, key);
                if (index >= values.Length) {
                    EnsureCapacity(index + 1);
                }
                values[index] = value;
                ++size;
            }
        }
    }
}
