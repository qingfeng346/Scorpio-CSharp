#define SCRIPT_OBJECT
using System;
using System.Collections.Generic;
#if SCRIPT_OBJECT
using EntityValue = Scorpio.ScriptObject;
#else
using EntityValue = System.String;
#endif
namespace Scorpio.Tools {
#if SCRIPT_OBJECT
    public partial class ScriptObjectReference {
#else
    public partial class StringReference {
#endif
        public struct Entity {
            public EntityValue value;
            public int referenceCount;
#if SCORPIO_DEBUG
            public int index;
#endif
            public override string ToString() {
                try {
                    return $"{value} 计数:{referenceCount}";
                } catch (System.Exception) {
                    return $"Exception:{value?.GetType()}:  计数:{referenceCount}";
                }
            }
        }
        public const int Stage = 8192;

#if !SCRIPT_OBJECT
        public static Dictionary<string, int> object2index = new Dictionary<string, int>();
#endif
        public static int entityLength = 0;
        public static Entity[] entities = new Entity[Stage];

        public static int poolLength = 0;
        public static int[] pool = new int[Stage];

        public static int freeLength = 0;
        public static int[] frees = new int[Stage];
        public static int Alloc(EntityValue value) {
#if SCRIPT_OBJECT
            if (value.Index > 0) {
                ++entities[value.Index].referenceCount;
                return value.Index;
            }
            int index;
#else
            if (object2index.TryGetValue(value, out var index)) {
                ++entities[index].referenceCount;
                return index;
            }
#endif
            if (poolLength > 0) {
                index = pool[--poolLength];
            } else {
                if (entityLength == entities.Length) {
                    var newEntities = new Entity[entityLength + Stage];
                    Array.Copy(entities, newEntities, entityLength);
                    entities = newEntities;
                }
                index = entityLength++;
#if SCORPIO_DEBUG
                entities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            object2index.Add(value, index);
#endif
            entities[index].value = value;
            entities[index].referenceCount = 1;
            return index;
        }
        //获取index,如果是新创建的立刻加入释放列表
        public static int GetIndex(EntityValue value) {
#if SCRIPT_OBJECT
            if (value.Index > 0) {
                ++entities[value.Index].referenceCount;
                return value.Index;
            }
            int index;
#else
            if (object2index.TryGetValue(value, out var index)) {
                return index;
            }
#endif
            if (poolLength > 0) {
                index = pool[--poolLength];
            } else {
                if (entityLength == entities.Length) {
                    var newEntities = new Entity[entityLength + Stage];
                    Array.Copy(entities, newEntities, entityLength);
                    entities = newEntities;
                }
                index = entityLength++;
#if SCORPIO_DEBUG
                entities[index].index = index;
#endif
            }
#if SCRIPT_OBJECT
            value.Index = index;
#else
            object2index.Add(value, index);
#endif
            entities[index].value = value;
            entities[index].referenceCount = 0;
            if (freeLength == frees.Length) {
                var newFrees = new int[freeLength + Stage];
                Array.Copy(frees, newFrees, freeLength);
                frees = newFrees;
            }
            frees[freeLength++] = index;
            return index;
        }
        public static int GetReferenceCount(int index) {
            return entities[index].referenceCount;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                if (freeLength == frees.Length) {
                    var newFrees = new int[freeLength + Stage];
                    Array.Copy(frees, newFrees, freeLength);
                    frees = newFrees;
                }
                frees[freeLength++] = index;
            }
#if SCORPIO_DEBUG
            if (entities[index].referenceCount < 0) {
                ScorpioLogger.error($"{typeof(EntityValue)} 释放有问题,当前计数:{entities[index].referenceCount}  Index:{index} - {entities[index]}");
            }
#endif
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
        }
        public static EntityValue GetValue(int index) {
            return entities[index].value;
        }
        //释放
        public static unsafe void ReleaseAll() {
            if (freeLength > 0) {
                int index;
                Entity entity;
                for (var i = 0; i < freeLength; ++i) {
                    index = frees[i];
                    entity = entities[index];
                    if (entity.referenceCount == 0) {
#if SCRIPT_OBJECT
                        entity.value.Index = -1;
                        entity.value.Free();
#else
                        object2index.Remove(entity.value);
#endif
                        if (poolLength == pool.Length) {
                            var newPool = new int[poolLength + Stage];
                            Array.Copy(pool, newPool, poolLength);
                            pool = newPool;
                        }
                        pool[poolLength++] = index;
                        entities[index].value = null;
                        entities[index].referenceCount = -1;
                    }
                }
                freeLength = 0;
            }
        }
        //检查未释放
        public static void CheckPool() {
            for (var i = 0; i < entityLength; ++i) {
                if (entities[i].value != null) {
                    ScorpioLogger.error($"当前未释放{typeof(EntityValue)}变量 索引:{i}  {entities[i]}");
                }
            }
        }
        public static void Shutdown() {
#if !SCRIPT_OBJECT
            object2index.Clear();
#endif
            Array.Clear(entities, 0, entityLength);
            entityLength = 0;
            poolLength = 0;
            freeLength = 0;
        }
    }
}
