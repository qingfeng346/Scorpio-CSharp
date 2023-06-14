//#define PRINT_REFERENCE
using Scorpio.Function;
using System;
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
                    return $"ID:{value?.Id}  {value}  引用计数:{referenceCount}";
                } catch (System.Exception) {
                    return $"ID:{value?.Id}  {value.GetType()}:  引用计数:{referenceCount}";
                }
                
            }
        }
        public class GCHandle {
            public int index;
            public int count;
            public HashSet<int> beCatch = new HashSet<int>();
            public GCHandle Set(int index) {
                this.index = index;
                this.beCatch.Clear();
                this.count = 0;
                return this;
            }
        }
        private const int Stage = 8192;
        private static int length = 0;
        private static Dictionary<uint, int> object2index = new Dictionary<uint, int>();
        public static Queue<int> pool = new Queue<int>();
        public static Entity[] entities = new Entity[Stage];
        public static List<int> freeIndex = new List<int>();

#if PRINT_REFERENCE
        static bool Is(int index, Entity entity) {
            //return index == 296;
            if (entity.value is ScriptMapObject) {
                return true;
                //return ((ScriptMapObject)entity.value).ContainsKey("www");
            }
            return false;
        }
#endif
        public static int Alloc(ScriptObject value) {
            if (object2index.TryGetValue(value.Id, out var index)) {
                ++entities[index].referenceCount;
#if PRINT_REFERENCE
                if (Is(index, entities[index])) {
                    logger.debug($"===================== Alloc重复  Index:{index} - {entities[index]}");
                }
#endif
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
            object2index.Add(value.Id, index);
            entities[index] = new Entity(value, 1);
#if PRINT_REFERENCE
            if (Is(index, entities[index])) {
                logger.debug($"===================== Alloc新  Index:{index} - {entities[index]}");
            }
#endif
            return index;
        }
        public static void Free(int index) {
            if ((--entities[index].referenceCount) == 0) {
                //添加到待释放列表
                freeIndex.Add(index);
            }
            if (entities[index].referenceCount < 0) {
                ScorpioLogger.error($"ScriptObject 释放有问题,当前计数:{entities[index].referenceCount}  Index:{index} - {entities[index]}");
            }
#if PRINT_REFERENCE
            if (Is(index, entities[index])) {
                logger.debug($"===================== Free  Index:{index} - {entities[index]}");
            }
#endif
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
#if PRINT_REFERENCE
            if (Is(index, entities[index])) {
                logger.debug($"===================== Reference  Index:{index} - {entities[index]}");
            }
#endif
        }
        public static ScriptObject GetValue(int index) {
            return entities[index].value;
        }
        //释放
        public static bool ReleaseAll() {
            var isReleased = false;
            if (freeIndex.Count > 0) {
                for (var i = 0; i < freeIndex.Count; ++i) {
                    var index = freeIndex[i];
                    if (entities[index].referenceCount == 0) {
#if PRINT_REFERENCE
                        if (Is(index, entities[index])) {
                            logger.debug($"============Release  Index:{index} - {entities[index]}");
                        }
#endif
                        object2index.Remove(entities[index].value.Id);
                        entities[index].value.Free();
                        pool.Enqueue(index);
                        entities[index] = DefaultEntity;
                        isReleased = true;
                    }
                }
                freeIndex.Clear();
            }
            return isReleased;
        }
        //检查未释放
        internal static void CheckPool() {
            for (var i = 0; i < entities.Length; ++i) {
                if (entities[i].value != null) {
                    ScorpioLogger.error($"当前未释放Scirpt变量 索引:{i}  {entities[i]}");
                }
            }
        }

        static ObjectsPool<GCHandle> gcHandlePool = new ObjectsPool<GCHandle>(() => new GCHandle());
        static Dictionary<int, GCHandle> gcHandle = new Dictionary<int, GCHandle>();
        static HashSet<int> canGC = new HashSet<int>();
        static HashSet<int> stack = new HashSet<int>();
        static void Collect(ScriptValue value, int originIndex) {
            if (value.valueType == ScriptValue.scriptValueType) {
                var index = value.scriptValueIndex;
                if (!gcHandle.TryGetValue(index, out var handle)) {
                    handle = gcHandle[index] = gcHandlePool.Alloc().Set(index);
                }
                handle.count++;
                handle.beCatch.Add(originIndex);
            }
        }
        static void Collect(ScriptObject value, int originIndex) {
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    Collect(pair.Value, originIndex);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    Collect(element, originIndex);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                Collect(((ScriptScriptBindFunctionBase)value).BindObject, originIndex);
            } else if (value is ScriptType) {
                Collect((value as ScriptType).PrototypeValue, originIndex);
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    Collect(pair.Value, originIndex);
                }
                Collect((value as ScriptInstance).PrototypeValue, originIndex);
            }
        }
        static bool CanGCRoot(GCHandle handle, HashSet<int> check) {
            if (canGC.Contains(handle.index)) return true;
            if (check.Contains(handle.index)) return true;
            check.Add(handle.index);
            //次数不同是被外部引用, c#对象或delegate或全局变量
            if (handle.count != entities[handle.index].referenceCount) return false;
            foreach (var index in handle.beCatch) {
                if (!gcHandle.TryGetValue(index, out var parent))
                    return false;
                if (!CanGCRoot(parent, check))
                    return false;
            }
            return true;
        }
        public static void GCCollect() {
            foreach (var pair in gcHandle) {
                gcHandlePool.Free(pair.Value);
            }
            gcHandle.Clear();
            for (var i = 0; i < entities.Length; ++i) {
                if (entities[i].value != null && entities[i].referenceCount > 0) {
                    Collect(entities[i].value, i);
                }
            }
            canGC.Clear();
            foreach (var pair in gcHandle) {
                stack.Clear();
                if (CanGCRoot(pair.Value, stack)) {
                    canGC.Add(pair.Key);
                }
            }
            if (canGC.Count > 0) {
                //ScorpioLogger.error("回收对象 : " + canGC.Count);
                foreach (var index in canGC) {
                    entities[index].value.gc();
                }
            }
        }
        public static void Shutdown() {
            object2index.Clear();
            pool.Clear();
            Array.Clear(entities, 0, entities.Length);
            freeIndex.Clear();
            length = 0;
        }
    }
}
