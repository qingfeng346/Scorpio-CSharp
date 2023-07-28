#if SCORPIO_DEBUG
using Scorpio.Exception;
using System.Collections.Generic;
using System;
using Scorpio.Function;
namespace Scorpio.Tools {
    public static class ScorpioProfiler {
        private static ulong AutoId = 0;
        public static StackInfo RecordStack;
        private static bool IsRecord = false;
        private static object sync = new object();
        public static Dictionary<ulong, (WeakReference, string)> AllObjects = new Dictionary<ulong, (WeakReference, string)>();
        public static Dictionary<ulong, (WeakReference, string)> Record = new Dictionary<ulong, (WeakReference, string)>();
        public static Dictionary<ulong, Dictionary<ulong, string>> CustomReference = new Dictionary<ulong, Dictionary<ulong, string>>();
        public static void StartRecord() {
            if (IsRecord) return;
            IsRecord = true;
            Record.Clear();
        }
        public static void AddRecord(ulong id, ScriptObject scriptObject) {
            lock (sync) {
                var source = RecordStack.ToString();
                var weak = new WeakReference(scriptObject);
                AllObjects[id] = (weak, source);
                if (!IsRecord) { return; }
                Record[id] = (weak, source);
            }
        }
        public static void DelRecord(ulong id) {
            lock (sync) {
                AllObjects.Remove(id);
                if (!IsRecord) { return; }
                Record.Remove(id);
            }
        }
        public static void EndRecord() {
            if (!IsRecord) return;
            IsRecord = false;
        }
        public static ulong AddCustomReference(ulong id, string str) {
            lock (sync) {
                if (!CustomReference.TryGetValue(id, out var value)) {
                    CustomReference[id] = value = new Dictionary<ulong, string>();
                }
                var index = AutoId++;
                value[index] = str;
                return index;
            }
        }
        public static void DelCustomReference(ulong id, ulong index) {
            lock (sync) {
                if (CustomReference.TryGetValue(id, out var value)) {
                    value.Remove(index);
                }
            }
        }
        #region 查找对象引用
        static void CollectReference(WeakReference weak, ulong targetId, HashSet<WeakReference> set) {
            if (!weak.IsAlive) { return; }
            void CollectReferenceObject(ScriptObject scriptObject) {
                if (scriptObject == null || scriptObject.Id != targetId) return;
                set.Add(weak);
            }
            void CollectReferenceValue(ScriptValue scriptValue) {
                if (scriptValue.valueType == ScriptValue.scriptValueType) {
                    CollectReferenceObject(scriptValue.scriptValue);
                }
            }
            var value = (ScriptObject)weak.Target;
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    CollectReferenceValue(pair.Value);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    CollectReferenceValue(element);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                CollectReferenceValue(((ScriptScriptBindFunctionBase)value).BindObject);
            } else if (value is ScriptType) {
                var type = (ScriptType)value;
                CollectReferenceObject(type.Prototype);
                foreach (var pair in type.m_Values) {
                    CollectReferenceValue(pair.Value);
                }
                foreach (var pair in type.m_GetProperties) {
                    CollectReferenceObject(pair.Value);
                }
            } else if (value is ScriptGlobal) {
                foreach (var pair in (value as ScriptGlobal)) {
                    CollectReferenceValue(pair.Value);
                }
            }
            if (value is ScriptInstance) {
                var instance = (ScriptInstance)value;
                foreach (var pair in instance) {
                    CollectReferenceValue(pair.Value);
                }
                CollectReferenceObject(instance.Prototype);
            }
            if (value is ScriptScriptFunctionBase) {
                var internalValues = ((ScriptScriptFunctionBase)value).InternalValues;
                if (internalValues != null) {
                    for (var i = 0; i < internalValues.Length; ++i) {
                        var internalValue = internalValues[i];
                        if (internalValue == null) { continue; }
                        CollectReferenceValue(internalValue.value);
                    }
                }
            }
        }
        public static void FindReference(ulong id, out HashSet<WeakReference> set, out List<string> custom) {
            lock (sync) {
                set = new HashSet<WeakReference>();
                custom = new List<string>();
                foreach (var pair in AllObjects) {
                    CollectReference(pair.Value.Item1, id, set);
                }
                if (CustomReference.TryGetValue(id, out var v)) {
                    custom.AddRange(v.Values);
                }
            }
        }
        #endregion
        #region 检测泄漏
        private static void CollectGlobal(ScriptValue value, HashSet<ulong> globalIndex) {
            if (value.valueType != ScriptValue.scriptValueType) { return; }
            CollectGlobal(value.scriptValue, globalIndex);
        }
        private static void CollectGlobal(ScriptObject value, HashSet<ulong> globalIndex) {
            if (value == null) return;
            if (globalIndex.Contains(value.Id)) { return; }
            globalIndex.Add(value.Id);
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    CollectGlobal(pair.Value, globalIndex);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    CollectGlobal(element, globalIndex);
                }
            } else if (value is ScriptHashSet) {
                foreach (var element in (ScriptHashSet)value) {
                    CollectGlobal(element, globalIndex);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                CollectGlobal(((ScriptScriptBindFunctionBase)value).BindObject, globalIndex);
            } else if (value is ScriptType) {
                var type = (ScriptType)value;
                CollectGlobal(type.Prototype, globalIndex);
                foreach (var pair in type.m_Values) {
                    CollectGlobal(pair.Value, globalIndex);
                }
                foreach (var pair in type.m_GetProperties) {
                    CollectGlobal(pair.Value, globalIndex);
                }
            }
            if (value is ScriptInstance) {
                var instance = (ScriptInstance)value;
                foreach (var pair in instance) {
                    CollectGlobal(pair.Value, globalIndex);
                }
            }
            if (value is ScriptScriptFunctionBase) {
                var internalValues = ((ScriptScriptFunctionBase)value).InternalValues;
                if (internalValues != null) {
                    for (var i = 0; i < internalValues.Length; ++i) {
                        var internalValue = internalValues[i];
                        if (internalValue == null) { continue; }
                        CollectGlobal(internalValue.value, globalIndex);
                    }
                }
            }
        }
        public static void CollectLeak(Script script, out HashSet<(WeakReference, string)> leak, out int globalCount, out int total) {
            lock (sync) {
                var globalIndex = new HashSet<ulong>();
                foreach (var pair in script.Global) {
                    CollectGlobal(pair.Value, globalIndex);
                }
                foreach (var pair in ScorpioTypeManager.m_UserdataTypes) {
                    CollectGlobal(pair.Value, globalIndex);
                }
                foreach (var pair in ScorpioTypeManager.m_Types) {
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
                total = AllObjects.Count;
                globalCount = globalIndex.Count;
                leak = new HashSet<(WeakReference, string)>();
                foreach (var pair in AllObjects) {
                    if (!globalIndex.Contains(pair.Key)) {
                        leak.Add(pair.Value);
                    }
                }
            }
        }
        #endregion
    }
}
#endif
