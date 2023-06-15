using System;
using System.Collections.Generic;
namespace Scorpio.Tools {
    public class StringReference {
        public class Entity {
            public string value;
            public int referenceCount;
            public void Set(string value) {
                this.value = value;
                this.referenceCount = 1;
            }
            public void Clear() {
                this.value = null;
                this.referenceCount = -1;
            }
            public override string ToString() {
                return $"{value}  引用计数:{referenceCount}";
            }
        }
        private const int Stage = 8192;
        private static int length = 0;
        private static Dictionary<string, int> object2index = new Dictionary<string, int>();
        public static Queue<int> pool = new Queue<int>();
        public static Entity[] entities = new Entity[Stage];
        public static List<int> freeIndex = new List<int>();
        public static int Alloc(string value) {
            if (object2index.TryGetValue(value, out var index)) {
                ++entities[index].referenceCount;
                return index;
            }
            if (pool.Count > 0) {
                index = pool.Dequeue();
            } else {
                index = length++;
                if (length >= entities.Length) {
                    var newEntities = new Entity[entities.Length + Stage];
                    Array.Copy(entities, newEntities, entities.Length);
                    entities = newEntities;
                }
                entities[index] = new Entity();
            }
            object2index.Add(value, index);
            entities[index].Set(value);
            return index;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                freeIndex.Add(index);
            }
#if SCORPIO_DEBUG
            if (entities[index].referenceCount < 0) {
                ScorpioLogger.error($"String 释放有问题,当前计数:{entities[index].referenceCount}  Index:{index} - {entities[index]}");
            }
#endif
        }
        public static string GetValue(int index) {
            return entities[index].value;
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
        }
        //释放index
        public static void ReleaseAll() {
            if (freeIndex.Count > 0) {
                int index;
                Entity entity;
                for (var i = 0; i < freeIndex.Count; ++i) {
                    index = freeIndex[i];
                    entity = entities[index];
                    if (entity.referenceCount == 0) {
                        object2index.Remove(entity.value);
                        pool.Enqueue(index);
                        entity.Clear();
                    }
                }
                freeIndex.Clear();
            }
        }
        internal static void CheckPool() {
            for (var i = 0; i < length; ++i) {
                if (entities[i].value != null) {
                    ScorpioLogger.error($"当前未释放String变量 索引:{i}  {entities[i]}");
                }
            }
        }
        public static void Shutdown() {
            object2index.Clear();
            pool.Clear();
            Array.Clear(entities, 0, length);
            freeIndex.Clear();
            length = 0;
        }
    }
}
