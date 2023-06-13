//#define PRINT_REFERENCE
using Scorpio.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        static void AddChildren(ScriptValue value, int originIndex, string from, HashSet<string> children, HashSet<int> childrenIndex) {
            if (value.valueType == ScriptValue.stringValueType) {
                children.Add($"{from}s{value.stringValueIndex}");
            } else if (value.valueType == ScriptValue.scriptValueType) {
                if (originIndex != value.scriptValueIndex && !childrenIndex.Contains(value.scriptValueIndex)) {
                    childrenIndex.Add(value.scriptValueIndex);
                    children.Add($"{from}{value.scriptValueIndex}");
                    AddChildren(value.scriptValue, originIndex, $"{from}{value.scriptValueIndex}->", children, childrenIndex);
                }
            }
        }
        static void AddChildren(ScriptObject value, int originIndex, string from, HashSet<string> children, HashSet<int> childrenIndex) {
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    AddChildren(pair.Value, originIndex, $"{from}[k:{pair.Key}]", children, childrenIndex);
                }
            } else if (value is ScriptArray) {
                var index = 0;
                foreach (var element in (ScriptArray)value) {
                    AddChildren(element, originIndex, $"{from}[i:{index}]", children, childrenIndex);
                    ++index;
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                AddChildren(((ScriptScriptBindFunctionBase)value).BindObject, originIndex, $"{from}[b]", children, childrenIndex);
            } else if (value is ScriptType) {
                AddChildren((value as ScriptType).PrototypeValue, originIndex, $"{from}[p]", children, childrenIndex);
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    AddChildren(pair.Value, originIndex, $"{from}[ik:{pair.Key}]", children, childrenIndex);
                }
                AddChildren((value as ScriptInstance).PrototypeValue, originIndex, $"{from}[t]", children, childrenIndex);
            }
        }
        internal static void CheckPool() {
            for (var i = 0; i < entities.Length; ++i) {
                if (entities[i].value != null) {
                    var childrenIndex = new HashSet<int>();
                    var children = new HashSet<string>();
                    AddChildren(entities[i].value, i, "", children, childrenIndex);
                    ScorpioLogger.error($"当前未释放Scirpt变量 索引:{i}  {entities[i]}, 持有对象:{string.Join(",", children.ToArray())}");
                }
            }
        }
        static void Collect(ScriptValue value, int originIndex, Dictionary<int, HashSet<int>> re, Dictionary<int, HashSet<int>> beRe) {
            if (value.valueType == ScriptValue.scriptValueType) {
                var index = value.scriptValueIndex;
                if (originIndex != index && (!re.TryGetValue(originIndex, out var set) || !set.Contains(index))) {
                    if (set == null) {
                        set = re[originIndex] = new HashSet<int>();
                    }
                    set.Add(index);
                    if (!beRe.TryGetValue(index, out var beSet)) {
                        beSet = beRe[index] = new HashSet<int>();
                    }
                    beSet.Add(originIndex);
                    Collect(value.scriptValue, originIndex, re, beRe);
                }
            }
        }
        static void Collect(ScriptObject value, int originIndex, Dictionary<int, HashSet<int>> re, Dictionary<int, HashSet<int>> beRe) {
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    Collect(pair.Value, originIndex, re, beRe);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    Collect(element, originIndex, re, beRe);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                Collect(((ScriptScriptBindFunctionBase)value).BindObject, originIndex, re, beRe);
            } else if (value is ScriptType) {
                Collect((value as ScriptType).PrototypeValue, originIndex, re, beRe);
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    Collect(pair.Value, originIndex, re, beRe);
                }
                Collect((value as ScriptInstance).PrototypeValue, originIndex, re, beRe);
            }
        }
        static void Release(int index) {
            //entities[index].referenceCount = 0;
            //object2index.Remove(entities[index].value.Id);
            //entities[index].value.Free();
            //pool.Enqueue(index);
            //entities[index] = DefaultEntity;
        }
        public static void GCCollect() {
            var re = new Dictionary<int, HashSet<int>>();       //持有
            var beRe = new Dictionary<int, HashSet<int>>();     //被持有
            for (var i = 0; i < entities.Length; ++i) {
                if (entities[i].value != null && entities[i].referenceCount > 0) {
                    Collect(entities[i].value, i, re, beRe);
                }
            }
            var dic = new Dictionary<int, int>();
            foreach (var pair in beRe) {
                Console.WriteLine($"{pair.Key} 被持有 : {string.Join(",", pair.Value.ToArray())}  {entities[pair.Key]}");
                //被持有数量跟计数不同,重复持有判断有问题
                if (pair.Value.Count == entities[pair.Key].referenceCount) {
                    foreach (var index in pair.Value) {
                        if (beRe.TryGetValue(index, out var set) && set.Contains(pair.Key)) {
                            if (dic.ContainsKey(pair.Key)) {
                                dic[pair.Key] += 1;
                            } else {
                                dic[pair.Key] = 1;
                            }
                        }
                    }
                }
            }
            foreach (var pair in beRe) {
                //
                if (dic.TryGetValue(pair.Key, out var number) && pair.Value.Count == number) {
                    Console.WriteLine("被释放 " + pair.Key);
                }
            }
            //(entities[293].value as ScriptMap).Clear();
            //(entities[294].value as ScriptMap).Clear();
            //Release(293);
            //Release(294);
            //Release(295);
            //Release(296);
            //if () {
            //    //添加到待释放列表
            //}
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
