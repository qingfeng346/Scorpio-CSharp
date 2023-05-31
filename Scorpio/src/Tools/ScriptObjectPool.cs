using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ScriptObjectPool {
        public struct Entity {
            public ScriptObject value;
            public int referenceCount;
        }
        private const int Stage = 8192;
        private static int count = 0;
        public static Queue<int> pool = new Queue<int>();
        public static Entity[] entities = new Entity[Stage];
        public static List<int> freeIndex = new List<int>(Stage);
        public static int GetIndex(ScriptObject scriptObject) {
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
            entities[index].value = scriptObject;
            entities[index].referenceCount = 1;
            return index;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                freeIndex.Add(index);
            }
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
        }
        public static ScriptObject GetValue(int index) {
            return entities[index].value;
        }
        //释放index
        public static void CheckFree() {
            for (var i = 0; i < freeIndex.Count; ++i) {
                var index = freeIndex[i];
                if (entities[index].referenceCount <= 0) {
                    entities[index].value.Free();
                    pool.Enqueue(index);
                    entities[index] = default;
                }
            }
            freeIndex.Clear();
        }
        public static void CheckEntity() {
            foreach (var entity in entities) {
                if (entity.value != null) {
                    Console.WriteLine("当前未释放变量 : " + entity.value.ToString() + "  " + entity.referenceCount);
                }
            }
        }
    }
}
