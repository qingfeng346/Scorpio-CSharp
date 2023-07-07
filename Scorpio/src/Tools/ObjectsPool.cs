using System;
namespace Scorpio.Tools {
#if SCRIPT_OBJECT
    public interface IPool {
        void Alloc();
        void Free();
    }
    public class ScriptObjectsPool<T> where T : IPool {
#else
    public class ObjectsPool<T> {
#endif
#if SCORPIO_DEBUG
        private int count = 0;
#endif
        public const int Stage = 8192;

        public Func<T> Generator;
        public int poolLength = 0;
        public T[] pool = new T[0];
#if SCRIPT_OBJECT
        public ScriptObjectsPool(Func<T> generator) {
#else
        public ObjectsPool(Func<T> generator) {
#endif
            Generator = generator;
        }
        public T Alloc() {
#if SCORPIO_DEBUG
            T ret;
            if (poolLength > 0) {
                ret = pool[--poolLength];
            } else {
                ++count;
                ret = Generator();
            }
#else
            var ret = poolLength > 0 ? pool[--poolLength] : Generator();
#endif
#if SCRIPT_OBJECT
            ret.Alloc();
#endif
            return ret;
        }
        public void Free(T item) {
#if SCORPIO_DEBUG
            if (item is ScriptObject)
                (item as ScriptObject).Source = null;
#endif
            if (poolLength == pool.Length) {
                var newPool = new T[poolLength + Stage];
                Array.Copy(pool, newPool, poolLength);
                pool = newPool;
            }
            pool[poolLength++] = item;
        }
        public int Check() {
#if SCORPIO_DEBUG
            //如果new的数量跟回收的数量不相同则有泄露
            return count - pool.Length;
#else
            return 0;
#endif
        }
    }
}
