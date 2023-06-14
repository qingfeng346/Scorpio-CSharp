//#define PRINT_REFERENCE
using Scorpio.Function;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.Tools {
    public class ScriptObjectReference {
        public class Entity {
            public ScriptObject value;
            public int referenceCount;
            public void Set(ScriptObject value) {
                this.value = value;
                this.referenceCount = 1;
            }
            public void Clear() {
                this.value = null;
                this.referenceCount = -1;
            }
            public override string ToString() {
                try {
                    return $"{value.ToFullString()} 计数:{referenceCount}";
                } catch (System.Exception) {
                    return $"{value?.GetType()}:  计数:{referenceCount}";
                }
            }
        }
        public class GCHandle {
            public int index;
            public int count;
            public HashSet<int> beCatch = new HashSet<int>();
            public GCHandle(int index) {
                this.index = index;
                this.count = 0;
            }
        }
        private const int Stage = 8192;
        private static int length = 0;
        private static Dictionary<uint, int> object2index = new Dictionary<uint, int>();
        public static Stack<int> pool = new Stack<int>();
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
                index = pool.Pop();
            } else {
                index = length++;
                if (length == entities.Length) {
                    var newEntities = new Entity[entities.Length + Stage];
                    Array.Copy(entities, newEntities, entities.Length);
                    entities = newEntities;
                }
                entities[index] = new Entity();
            }
            object2index.Add(value.Id, index);
            entities[index].Set(value);
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
#if SCORPIO_DEBUG
            if (entities[index].referenceCount < 0) {
                ScorpioLogger.error($"ScriptObject 释放有问题,当前计数:{entities[index].referenceCount}  Index:{index} - {entities[index]}");
            }
#endif
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
        public static void ReleaseAll() {
            if (freeIndex.Count > 0) {
                int index;
                Entity entity;
                for (var i = 0; i < freeIndex.Count; ++i) {
                    index = freeIndex[i];
                    entity = entities[index];
                    if (entity.referenceCount == 0) {
#if PRINT_REFERENCE
                        if (Is(index, entities[index])) {
                            logger.debug($"============Release  Index:{index} - {entities[index]}");
                        }
#endif
                        object2index.Remove(entity.value.Id);
                        entity.value.Free();
                        pool.Push(index);
                        entity.Clear();
                    }
                }
                freeIndex.Clear();
            }
        }
        //检查未释放
        internal static void CheckPool() {
            for (var i = 0; i < length; ++i) {
                if (entities[i].value != null) {
                    ScorpioLogger.error($"当前未释放Scirpt变量 索引:{i}  {entities[i]}");
                }
            }
        }

        //static ObjectsPool<GCHandle> gcHandlePool = new ObjectsPool<GCHandle>(() => new GCHandle());
        static Dictionary<int, GCHandle> gcHandle = new Dictionary<int, GCHandle>();
        static HashSet<int> canGC = new HashSet<int>();
        static HashSet<int> stack = new HashSet<int>();
        static void Collect(ScriptValue value, int originIndex) {
            if (value.valueType == ScriptValue.scriptValueType) {
                var index = value.scriptValueIndex;
                if (!gcHandle.TryGetValue(index, out var handle)) {
                    handle = gcHandle[index] = new GCHandle(index);// gcHandlePool.Alloc().Set(index);
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
                foreach (var pair in (value as ScriptType)) {
                    Collect(pair.Value, originIndex);
                }
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
        static void CollectGC() {
            gcHandle.Clear();
            for (var i = 0; i < length; ++i) {
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
        }
        public static void GCCollect() {
            CollectGC();
            if (canGC.Count > 0) {
                foreach (var index in canGC) {
                    entities[index].value.gc();
                }
            }
        }
        public static void CheckGCCollect() {
            CollectGC();
            if (canGC.Count > 0) {
                foreach (var index in canGC) {
                    ScorpioLogger.error($"[{index}] 可以被释放,被持有列表:{string.Join(",", gcHandle[index].beCatch.ToArray())}  内容:{entities[index]}");
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
