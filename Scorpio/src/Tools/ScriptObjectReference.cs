﻿using System;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public class ScriptObjectReference {
        private static readonly Entity DefaultEntity = new Entity(null, -1);
        public struct Entity {
            public ScriptObject value;
            public int referenceCount;
            public Entity(ScriptObject value, int referenceCount) {
                this.value = value;
                this.referenceCount = referenceCount;
            }
            public override string ToString() {
                try {
                    return $"{value}  引用计数:{referenceCount}";
                } catch (System.Exception) {
                    return $"{value.GetType()}  引用计数:{referenceCount}";
                }
                
            }
        }
        private const int Stage = 8192;
        private static int length = 0;
        private static Dictionary<uint, int> object2index = new Dictionary<uint, int>();
        public static Queue<int> pool = new Queue<int>();
        public static Entity[] entities = new Entity[Stage];
        public static List<int> freeIndex = new List<int>();
        public static void Clear() {
            object2index.Clear();
            pool.Clear();
            Array.Clear(entities, 0, entities.Length);
            freeIndex.Clear();
            length = 0;
        }
        public static int Alloc(ScriptObject value) {
            //if (value is ScriptMapObject) {
            //    logger.debug("===================== Alloc ");
            //}
            if (object2index.TryGetValue(value.Id, out var index)) {
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
            //if (entities[index].value is ScriptMapObject) {
            //    logger.debug("Free  ===================== ");
            //}
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
            //if (entities[index].value is ScriptMapObject) {
            //    logger.debug("Reference  ===================== ");
            //}
        }
        public static ScriptObject GetValue(int index) {
            return entities[index].value;
        }
        //释放
        public static bool ReleaseAll() {
            var isReleased = false;
            for (var i = 0; i < freeIndex.Count; ++i) {
                var index = freeIndex[i];
                if (entities[index].referenceCount == 0) {
                    object2index.Remove(entities[index].value.Id);
                    entities[index].value.Free();
                    pool.Enqueue(index);
                    entities[index] = DefaultEntity;
                    isReleased = true;
                }
            }
            freeIndex.Clear();
            return isReleased;
        }
        public static void Check(Action<int, Entity> action) {
            for (var i = 0; i < entities.Length; ++i) {
                if (entities[i].value != null) {
                    action(i, entities[i]);
                    //Console.WriteLine($"当前未释放Scirpt变量 索引:{i}  {entities[i]}");
                }
            }
        }
    }
}
