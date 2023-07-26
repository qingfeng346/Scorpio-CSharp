using System.Collections.Generic;
using System;
using Scorpio.Function;

namespace Scorpio {
    public partial class Script {
#if SCORPIO_DEBUG
        private bool IsRecord = false;
        private object sync = new object();
        public Dictionary<ulong, (WeakReference, string)> AllObjects = new Dictionary<ulong, (WeakReference, string)>();
        public Dictionary<ulong, (WeakReference, string)> Record = new Dictionary<ulong, (WeakReference, string)>();
        public void StartRecord() {
            if (IsRecord) return;
            IsRecord = true;
            Record.Clear();
        }
        public void AddRecord(ulong id, string source, ScriptInstance scriptInstance) {
            lock (sync) {
                var weak = new WeakReference(scriptInstance);
                AllObjects[id] = (weak, source);
                if (!IsRecord) { return; }
                Record[id] = (weak, source);
            }
        }
        public void DelRecord(ulong id) {
            lock (sync) {
                AllObjects.Remove(id);
                if (!IsRecord) { return; }
                Record.Remove(id);
            }
        }
        public void EndRecord() {
            if (!IsRecord) return;
            IsRecord = false;
        }
#endif
        static void CollectGlobal(ScriptValue value, HashSet<ulong> globalIndex, HashSet<ulong> globalObjects) {
            if (value.valueType != ScriptValue.scriptValueType) { return; }
            CollectGlobal(value.scriptValue, globalIndex, globalObjects);
        }
        static void CollectGlobal(ScriptObject value, HashSet<ulong> globalIndex, HashSet<ulong> globalObjects) {
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
        public void CollectLeak(out HashSet<(WeakReference, string)> set, out int count) {
            lock (sync) {
                var globalIndex = new HashSet<ulong>();
                var globalObjects = new HashSet<ulong>();
                foreach (var pair in m_Global) {
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
