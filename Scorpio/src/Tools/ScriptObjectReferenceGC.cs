using Scorpio.Function;
using System.Collections.Generic;
using System.Linq;

namespace Scorpio.Tools {
    public partial class ScriptObjectReference {
        public class GCHandle {
            public int index;
            public int count;
            public HashSet<int> beCatch = new HashSet<int>();
            public GCHandle(int index) {
                this.index = index;
                this.count = 0;
            }
        }
        static Dictionary<int, GCHandle> gcHandle = new Dictionary<int, GCHandle>();
        static HashSet<int> canGC = new HashSet<int>();
        static HashSet<int> stack = new HashSet<int>();
        static void Collect(ScriptValue value, int originIndex) {
            if (value.valueType == ScriptValue.scriptValueType) {
                Collect(value.index, originIndex);
            }
        }
        static void Collect(int index, int originIndex) {
            if (!gcHandle.TryGetValue(index, out var handle)) {
                handle = gcHandle[index] = new GCHandle((int)index);
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
            for (var i = 0; i < entityLength; ++i) {
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
                CollectReference((value as ScriptType).Prototype.Index, originIndex, targetIndex, set, isString);
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
        public static HashSet<int> GetReference(int index, bool isString) {
            var set = new HashSet<int>();
            for (var i = 0; i < entityLength; ++i) {
                if (entities[i].value != null) {
                    CollectReference(entities[i].value, i, index, set, isString);
                }
            }
            return set;
        }
    }
}
