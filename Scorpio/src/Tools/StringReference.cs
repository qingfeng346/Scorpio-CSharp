using System;
using System.Collections.Generic;
namespace Scorpio.Tools {
    public class StringReference {
        private static readonly Entity DefaultEntity = new Entity(null, -1);
        public struct Entity {
            public string value;
            public int referenceCount;
            public Entity(string value, int referenceCount) {
                this.value = value;
                this.referenceCount = referenceCount;
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
            }
            entities[index] = new Entity(value, 1);
            return index;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                freeIndex.Add(index);
            }
        }
        public static string GetValue(int index) {
            return entities[index].value;
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
        }
        //释放index
        public static void ReleaseAll() {
            for (var i = 0; i < freeIndex.Count; ++i) {
                var index = freeIndex[i];
                if (entities[index].referenceCount == 0) {
                    object2index.Remove(entities[index].value);
                    pool.Enqueue(index);
                    entities[index] = DefaultEntity;
                }
            }
            freeIndex.Clear();
        }
        public static void Check() {
            foreach (var entity in entities) {
                if (entity.value != null) {
                    Console.WriteLine("当前未释放String变量 : " + entity);
                }
            }
        }
    }
}
