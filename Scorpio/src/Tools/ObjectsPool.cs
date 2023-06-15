using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ObjectsPool<T> {
#if SCORPIO_DEBUG
        private int count = 0;
#endif
        public Func<T> Generator;
        private Queue<T> pool = new Queue<T>();
        public ObjectsPool(Func<T> generator) {
            Generator = generator;
        }
        public T Alloc() {
#if SCORPIO_DEBUG
            if (pool.Count > 0) {
                return pool.Dequeue();
            } else {
                ++count;
                return Generator();
            }
#else
            return pool.Count > 0 ? pool.Dequeue() : Generator();
#endif
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
