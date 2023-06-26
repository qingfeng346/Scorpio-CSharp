//#define SCORPIO_REFERENCE
using Scorpio.Function;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.Tools {
    public class ScriptObjectReference {
        public class Entity {
            public ScriptObject value;
            public int referenceCount;
#if SCORPIO_DEBUG
            public int index;
            public Entity(int index) {
                this.index = index;
            }
#endif
            public void Set(ScriptObject value, int referenceCount) {
                this.value = value;
                this.referenceCount = referenceCount;
            }
            public void Clear() {
                this.value = null;
                this.referenceCount = -1;
            }
            public override string ToString() {
                try {
                    return $"{value.ToFullString()} 计数:{referenceCount}";
                } catch (System.Exception) {
                    return $"Exception:{value?.GetType()}:  计数:{referenceCount}";
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
        public static Func<int, Entity, bool> IsEntity;
        public static Action<int, int, Entity> EntityReferenceChanged;
#if SCORPIO_REFERENCE
        static bool Is(int index, Entity entity) {
            return IsEntity?.Invoke(index, entity) ?? false;
        }
#endif
        public static int Alloc(ScriptObject value) {
            if (object2index.TryGetValue(value.Id, out var index)) {
                ++entities[index].referenceCount;
#if SCORPIO_REFERENCE
                if (Is(index, entities[index])) EntityReferenceChanged(1, index, entities[index]);
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
#if SCORPIO_DEBUG
                entities[index] = new Entity(index);
#else
                entities[index] = new Entity();
#endif
            }
            object2index.Add(value.Id, index);
            entities[index].Set(value, 1);
#if SCORPIO_REFERENCE
            if (Is(index, entities[index])) EntityReferenceChanged(1, index, entities[index]);
#endif
            return index;
        }
        //获取index,如果是新创建的立刻加入释放列表
        public static int GetIndex(ScriptObject value) {
            if (object2index.TryGetValue(value.Id, out var index)) {
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
#if SCORPIO_DEBUG
                entities[index] = new Entity(index);
#else
                entities[index] = new Entity();
#endif
            }
            object2index.Add(value.Id, index);
            entities[index].Set(value, 0);
            freeIndex.Add(index);
            return index;
        }
        public static int GetReferenceCount(int index) {
            return entities[index].referenceCount;
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
#if SCORPIO_REFERENCE
            if (Is(index, entities[index])) EntityReferenceChanged(0, index, entities[index]);
#endif
        }
        public static void Reference(int index) {
            ++entities[index].referenceCount;
#if SCORPIO_REFERENCE
            if (Is(index, entities[index])) EntityReferenceChanged(2, index, entities[index]);
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
#if SCORPIO_REFERENCE
                        if (Is(index, entities[index])) EntityReferenceChanged(3, index, entities[index]);
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

        static Dictionary<int, GCHandle> gcHandle = new Dictionary<int, GCHandle>();
        static HashSet<int> canGC = new HashSet<int>();
        static HashSet<int> stack = new HashSet<int>();
        static void Collect(ScriptValue value, int originIndex) {
            if (value.valueType == ScriptValue.scriptValueType) {
                var index = value.index;
                if (!gcHandle.TryGetValue((int)index, out var handle)) {
                    handle = gcHandle[(int)index] = new GCHandle((int)index);
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
        public static void CheckGCCollect(out HashSet<int> gc, out Dictionary<int, GCHandle> h) {
            CollectGC();
            if (canGC.Count > 0) {
                foreach (var index in canGC) {
                    ScorpioLogger.error($"[{index}] 可以被释放,被持有列表:{string.Join(",", gcHandle[index].beCatch.ToArray())}  内容:{entities[index]}");
                }
            }
            gc = canGC;
            h = gcHandle;
        }
        static void CollectReference(ScriptValue value, int originIndex, int targetIndex, HashSet<int> set, bool isString) {
            if (isString) {
                if (value.valueType == ScriptValue.stringValueType && value.index == targetIndex) {
                    set.Add(originIndex);
                }
            } else if (value.valueType == ScriptValue.scriptValueType && value.index == targetIndex) {
                set.Add(originIndex);
            }
        }
        static void CollectReference(ScriptObject value, int originIndex, int targetIndex, HashSet<int> set, bool isString) {
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    CollectReference(pair.Value, originIndex, targetIndex, set, isString);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    CollectReference(element, originIndex, targetIndex, set, isString);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                CollectReference(((ScriptScriptBindFunctionBase)value).BindObject, originIndex, targetIndex, set, isString);
            } else if (value is ScriptType) {
                CollectReference((value as ScriptType).PrototypeValue, originIndex, targetIndex, set, isString);
                foreach (var pair in (value as ScriptType)) {
                    CollectReference(pair.Value, originIndex, targetIndex, set, isString);
                }
            } else if (value is ScriptGlobal) {
                foreach (var pair in (value as ScriptGlobal)) {
                    CollectReference(pair.Value, originIndex, targetIndex, set, isString);
                }
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    CollectReference(pair.Value, originIndex, targetIndex, set, isString);
                }
                CollectReference((value as ScriptInstance).PrototypeValue, originIndex, targetIndex, set, isString);
            }
        }
        public static HashSet<int> GetReference(int index, bool isString) {
            var set = new HashSet<int>();
            for (var i = 0; i < length; ++i) {
                if (entities[i].value != null) {
                    CollectReference(entities[i].value, i, index, set, isString);
                }
            }
            return set;
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
