using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public interface IPool {
        void Alloc();
        void Free();
    }
    public class ScriptObjectsPool<T> where T : IPool {
        public Func<T> Generator;
        private Stack<T> pool = new Stack<T>();
        public ScriptObjectsPool(Func<T> generator) {
            Generator = generator;
        }
        public T Alloc() {
            var ret = pool.Count > 0 ? pool.Pop() : Generator();
            ret.Alloc();
            return ret;
        }
        public void Free(T item) {
            pool.Push(item);
        }
    }
}
