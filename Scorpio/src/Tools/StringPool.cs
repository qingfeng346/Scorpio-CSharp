using System;
using System.Collections.Generic;
namespace Scorpio.Tools {
    public class StringPool {
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
        private static int count = 0;
        public static Queue<int> pool = new Queue<int>();
        public static Entity[] entities = new Entity[Stage];
        public static List<int> freeIndex = new List<int>(Stage);
        public static int GetIndex(string stringValue) {
            int index;
            if (pool.Count > 0) {
                index = pool.Dequeue();
            } else {
                index = count++;
                if (count >= entities.Length) {
                    var newEntities = new Entity[entities.Length + Stage];
                    Array.Copy(entities, newEntities, entities.Length);
                    entities = newEntities;
                }
            }
            entities[index] = new Entity(stringValue, 1);
            //if (entities[index].value == "1")
            //    logger.debug("GetIndex- " + entities[index]);
            return index;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                freeIndex.Add(index);
            }
            //if (entities[index].value == "1")
            //    logger.debug("Free- " + entities[index]);
        }
        public static string GetValue(int index) {
            return entities[index].value;
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
            //if (entities[index].value == "1")
            //    logger.debug("Reference- " + entities[index]);
        }
        //释放index
        public static void CheckFree() {
            for (var i = 0; i < freeIndex.Count; ++i) {
                var index = freeIndex[i];
                if (entities[index].referenceCount == 0) {
                    pool.Enqueue(index);
                    entities[index] = DefaultEntity;
                }
            }
            freeIndex.Clear();
        }
        public static void CheckEntity() {
            foreach (var entity in entities) {
                if (entity.value != null) {
                    Console.WriteLine("当前未释放String变量 : " + entity);
                }
            }
        }
    }
}
