using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public interface IPool {
        void Alloc();
        void Free();
    }
    public class ScriptObjectsPool<T> where T : IPool {
#if SCORPIO_DEBUG
        private int count = 0;
#endif
        public Func<T> Generator;
        private Queue<T> pool = new Queue<T>();
        public ScriptObjectsPool(Func<T> generator) {
            Generator = generator;
        }
        public T Alloc() {
#if SCORPIO_DEBUG
            T ret;
            if (pool.Count > 0) {
                ret = pool.Dequeue();
            } else {
                ++count;
                ret = Generator();
            }
#else
            var ret = pool.Count > 0 ? pool.Dequeue() : Generator();
#endif
            ret.Alloc();
            return ret;
        }
        public void Free(T item) {
            pool.Enqueue(item);
        }
        public int Check() {
#if SCORPIO_DEBUG
            //如果new的数量跟回收的数量不相同则有泄露
            return count - pool.Count;
#else
            return 0;
#endif
        }
    }
}
