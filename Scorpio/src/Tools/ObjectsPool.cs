using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ObjectsPool<T> {
        public Func<T> Generator;
        private Stack<T> pool = new Stack<T>();
        public ObjectsPool(Func<T> generator) {
            Generator = generator;
        }
        public T Alloc() {
            return pool.Count > 0 ? pool.Pop() : Generator();
        }
        public void Free(T item) {
            pool.Push(item);
        }
    }
}
