#if SCORPIO_DEBUG
using Scorpio.Exception;
using System.Collections.Generic;
using System;
using Scorpio.Function;
namespace Scorpio.Tools {
    public static class ScorpioProfiler {
        public static StackInfo RecordStack;
        private static bool IsRecord = false;
        private static object sync = new object();
        public static Dictionary<ulong, (WeakReference, string)> AllObjects = new Dictionary<ulong, (WeakReference, string)>();
        public static Dictionary<ulong, (WeakReference, string)> Record = new Dictionary<ulong, (WeakReference, string)>();
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
        private static void CollectGlobal(ScriptValue value, HashSet<ulong> globalIndex, HashSet<ulong> globalObjects) {
            if (value.valueType != ScriptValue.scriptValueType) { return; }
            CollectGlobal(value.scriptValue, globalIndex, globalObjects);
        }
        private static void CollectGlobal(ScriptObject value, HashSet<ulong> globalIndex, HashSet<ulong> globalObjects) {
            if (value == null) return;
            if (globalObjects.Contains(value.Id)) { return; }
            globalObjects.Add(value.Id);
            if (value is ScriptMap) {
                foreach (var pair in (ScriptMap)value) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
            } else if (value is ScriptArray) {
                foreach (var element in (ScriptArray)value) {
                    CollectGlobal(element, globalIndex, globalObjects);
                }
            } else if (value is ScriptHashSet) {
                foreach (var element in (ScriptHashSet)value) {
                    CollectGlobal(element, globalIndex, globalObjects);
                }
            } else if (value is ScriptScriptBindFunctionBase) {
                CollectGlobal(((ScriptScriptBindFunctionBase)value).BindObject, globalIndex, globalObjects);
            } else if (value is ScriptType) {
                var type = (ScriptType)value;
                CollectGlobal(type.Prototype, globalIndex, globalObjects);
                foreach (var pair in type.m_Values) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
                foreach (var pair in type.m_GetProperties) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
            }
            if (value is ScriptInstance) {
                var instance = (ScriptInstance)value;
                globalIndex.Add(instance.Id);
                foreach (var pair in instance) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
            }
            if (value is ScriptScriptFunctionBase) {
                var internalValues = ((ScriptScriptFunctionBase)value).InternalValues;
                if (internalValues != null) {
                    for (var i = 0; i < internalValues.Length; ++i) {
                        var internalValue = internalValues[i];
                        if (internalValue == null) { continue; }
                        CollectGlobal(internalValue.value, globalIndex, globalObjects);
                    }
                }
            }
        }
        public static void CollectLeak(Script script, out HashSet<(WeakReference, string)> set, out int count) {
            lock (sync) {
                var globalIndex = new HashSet<ulong>();
                var globalObjects = new HashSet<ulong>();
                foreach (var pair in script.Global) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
                foreach (var pair in ScorpioTypeManager.m_UserdataTypes) {
                    CollectGlobal(pair.Value, globalIndex, globalObjects);
                }
                foreach (var pair in ScorpioTypeManager.m_Types) {
                    foreach (var v in pair.Value.m_Values) {
                        CollectGlobal(v.Value, globalIndex, globalObjects);
                    }
                    foreach (var v in pair.Value.m_InstanceMethods) {
                        CollectGlobal(v.Value, globalIndex, globalObjects);
                    }
                    foreach (var v in pair.Value.m_StaticMethods) {
                        CollectGlobal(v.Value, globalIndex, globalObjects);
                    }
                }
                count = globalIndex.Count;
                set = new HashSet<(WeakReference, string)>();
                foreach (var pair in AllObjects) {
                    if (!globalIndex.Contains(pair.Key)) {
                        set.Add(pair.Value);
                    }
                }
            }
        }
    }
}
#endif
