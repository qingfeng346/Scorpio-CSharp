﻿using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ObjectsPool<T> {
        public Func<T> Generator;
        private Queue<T> pool = new Queue<T>();
        public ObjectsPool(Func<T> generator) {
            Generator = generator;
        }
        public T Alloc() {
            return pool.Count > 0 ? pool.Dequeue() : Generator();
        }
        public void Free(T item) {
            pool.Enqueue(item);
        }
    }
}
