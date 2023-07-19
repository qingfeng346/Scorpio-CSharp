using Scorpio.Function;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scorpio.Tools {
    public class GCHandle {
        public int index;               //当前索引
        public int count;               //被计算到的引用的次数
        public HashSet<int> beCatch;    //被引用的索引列表
        public GCHandle(int index) {
            this.index = index;
            this.count = 0;
            beCatch = new HashSet<int>();
        }
    }
    public partial class ScriptObjectReference {
        #region 搜集GC对象
        //每个索引的数据
        static Dictionary<int, GCHandle> gcHandles = new Dictionary<int, GCHandle>();
        static Dictionary<int, List<string>> customReference = new Dictionary<int, List<string>>();
        //可以被释放的索引列表
        static HashSet<int> canGC = new HashSet<int>();
        static void Collect(ScriptValue value, int parentIndex) {
            if (value.valueType == ScriptValue.scriptValueType) {
                Collect(value.index, parentIndex);
            }
        }
        static void Collect(int index, int originIndex) {
            if (!gcHandles.TryGetValue(index, out var handle)) {
                handle = gcHandles[index] = new GCHandle(index);
            }
            handle.count++;
            handle.beCatch.Add(originIndex);
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
            } else if (value is ScriptHashSet) {
                foreach (var element in (ScriptHashSet)value) {
                    Collect(element, originIndex);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                Collect(((ScriptScriptBindFunctionBase)value).BindObject, originIndex);
            } else if (value is ScriptType) {
                var protoType = ((ScriptType)value).Prototype;
                if (protoType != null) Collect(protoType.Index, originIndex);
                foreach (var pair in (value as ScriptType)) {
                    Collect(pair.Value, originIndex);
                }
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    Collect(pair.Value, originIndex);
                }
                Collect((value as ScriptInstance).Prototype.Index, originIndex);
            }
            if (value is ScriptScriptFunctionBase) {
                var internalValues = ((ScriptScriptFunctionBase)value).InternalValues;
                if (internalValues != null) {
                    for (var i = 0; i < internalValues.Length; ++i) {
                        var internalValue = internalValues[i];
                        if (internalValue == null) { continue; }
                        Collect(internalValue.value, originIndex);
                    }
                }
            }
        }
        static bool CanGCRoot(GCHandle handle, HashSet<int> check) {
            if (canGC.Contains(handle.index)) return true;
            if (check.Contains(handle.index)) return true;
            check.Add(handle.index);
            //次数不同是被外部引用, c#对象或delegate或全局变量
            if (handle.count != entities[handle.index].referenceCount) return false;
            foreach (var index in handle.beCatch) {
                if (!gcHandles.TryGetValue(index, out var parent))
                    return false;
                if (!CanGCRoot(parent, check))
                    return false;
            }
            return true;
        }
        //计算所有的GCHandle
        static void CollectGCHandle() {
            gcHandles.Clear();
            for (var i = 0; i < entityLength; ++i) {
                if (entities[i].value != null && entities[i].referenceCount > 0) {
                    Collect(entities[i].value, i);
                }
            }
            canGC.Clear();
            var stack = new HashSet<int>();
            foreach (var pair in gcHandles) {
                stack.Clear();
                if (CanGCRoot(pair.Value, stack)) {
                    canGC.Add(pair.Key);
                }
            }
        }
        //收集可以被GC的索引
        public static void CollectGCInfos(out HashSet<int> gc, out Dictionary<int, GCHandle> handles) {
            CollectGCHandle();
            gc = canGC;
            handles = gcHandles;
        }
        //收集可以被GC的对象并调用gc
        public static void GCCollect() {
            CollectGCInfos(out var gc, out _);
            if (gc.Count > 0) {
                foreach (var index in gc) {
                    entities[index].value.gc();
                }
            }
        }
        #endregion
        #region 查找每个对象的引用关系
        static void CollectReference(ScriptValue value, int originIndex, int targetIndex, HashSet<int> set, bool isString) {
            if (isString) {
                if (value.valueType == ScriptValue.stringValueType && value.index == targetIndex) {
                    set.Add(originIndex);
                }
            } else if (value.valueType == ScriptValue.scriptValueType && value.index == targetIndex) {
                set.Add(originIndex);
            }
        }
        static void CollectReference(int index, int originIndex, int targetIndex, HashSet<int> set, bool isString) {
            if (!isString && index == targetIndex) {
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
                var protoType = ((ScriptType)value).Prototype;
                if (protoType != null) CollectReference(protoType.Index, originIndex, targetIndex, set, isString);
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
                CollectReference((value as ScriptInstance).Prototype.Index, originIndex, targetIndex, set, isString);
            }
        }
        //计算被引用的对象
        public static void GetReference(int index, bool isString, out HashSet<int> set, out List<string> custom, out int referenceCount) {
            set = new HashSet<int>();
            referenceCount = isString ? StringReference.GetReferenceCount(index) : GetReferenceCount(index);
            for (var i = 0; i < entityLength; ++i) {
                if (entities[i].value != null) {
                    CollectReference(entities[i].value, i, index, set, isString);
                }
            }
            customReference.TryGetValue(index, out custom);
        }
        [Conditional("SCORPIO_DEBUG")]
        public static void AddCustomReference(int index, string str) {
            if (!customReference.TryGetValue(index, out var value)) {
                customReference[index] = value = new List<string>();
            }
            value.Add(str);
        }
        [Conditional("SCORPIO_DEBUG")]
        public static void DelCustomReference(int index, string str) {
            if (customReference.TryGetValue(index, out var value)) {
                value.Remove(str);
            }
        }
        #endregion
        static void CollectGlobal(ScriptValue value, HashSet<int> global) {
            if (value.valueType != ScriptValue.scriptValueType) { return; }
            CollectGlobal(value.scriptValue, global);
        }
        static void CollectGlobal(ScriptObject value, HashSet<int> global) {
            if (value == null) return;
            var index = value.Index;
            if (global.Contains(index)) { return; }
            global.Add(index);
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    CollectGlobal(pair.Value, global);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    CollectGlobal(element, global);
                }
            } else if (value is ScriptHashSet) {
                foreach (var element in (ScriptHashSet)value) {
                    CollectGlobal(element, global);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                CollectGlobal(((ScriptScriptBindFunctionBase)value).BindObject, global);
            } else if (value is ScriptType) {
                CollectGlobal(((ScriptType)value).Prototype, global);
                foreach (var pair in (ScriptType)value) {
                    CollectGlobal(pair.Value, global);
                }
                foreach (var pair in ((ScriptType)value).m_GetProperties) {
                    CollectGlobal(pair.Value, global);
                }
            }
            if (value is ScriptInstance) {
                foreach (var pair in (value as ScriptInstance)) {
                    CollectGlobal(pair.Value, global);
                }
                CollectGlobal(((ScriptInstance)value).Prototype, global);
            }
            if (value is ScriptScriptFunctionBase) {
                var internalValues = ((ScriptScriptFunctionBase)value).InternalValues;
                if (internalValues != null) {
                    for (var i = 0; i < internalValues.Length; ++i) {
                        var internalValue = internalValues[i];
                        if (internalValue == null) { continue; }
                        CollectGlobal(internalValue.value, global);
                    }
                }
            }
        }
        //搜集泄漏对象,暂时判断没有在global里就算泄漏
        public static void CollectLeak(Script script, out HashSet<int> set) {
            set = new HashSet<int>();
            var globalIndex = new HashSet<int>();
            foreach (var pair in script.Global) {
                CollectGlobal(pair.Value, globalIndex);
            }
            foreach (var pair in script.m_UserdataTypes) {
                CollectGlobal(pair.Value, globalIndex);
            }
            foreach (var pair in script.m_Types) {
                foreach (var v in pair.Value.m_Values) {
                    CollectGlobal(v.Value, globalIndex);
                }
                foreach (var v in pair.Value.m_InstanceMethods) {
                    CollectGlobal(v.Value, globalIndex);
                }
                foreach (var v in pair.Value.m_StaticMethods) {
                    CollectGlobal(v.Value, globalIndex);
                }
            }
            for (var i = 0; i < entityLength;++i) {
                if (entities[i].value != null && !globalIndex.Contains(i)) {
                    set.Add(i);
                }
            }
        }
    }
}
