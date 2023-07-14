using Scorpio.Tools;
using System.Collections.Generic;
using System;

namespace Scorpio {
    public partial class Script {
        public bool IsRecord = false;
        public int objEntityLength = 0, objPoolLength = 0;
        public int strEntityLength = 0, strPoolLength = 0;
        public Dictionary<string, Dictionary<uint, ScriptObject>> Record = new Dictionary<string, Dictionary<uint, ScriptObject>>();
        public Dictionary<string, List<string>> RecordDestroy = new Dictionary<string, List<string>>();
        public Dictionary<Type, uint> NewTypes = new Dictionary<Type, uint>();
        public void StartRecord() {
            if (IsRecord) return;
            IsRecord = true;
            RecordDestroy.Clear();
            Record.Clear();
            NewTypes.Clear();
            objEntityLength = ScriptObjectReference.entityLength;
            objPoolLength = ScriptObjectReference.poolLength;
            strEntityLength = StringReference.entityLength;
            strPoolLength = StringReference.poolLength;
        }
        public void NewType(Type type) {
            if (!NewTypes.ContainsKey(type)) {
                NewTypes[type] = 1;
                return;
            }
            NewTypes[type]++;
        }
        public void AddRecord(string source, uint id, ScriptObject scriptObject) {
            if (!IsRecord) { return; }
            if (!Record.ContainsKey(source)) {
                Record[source] = new Dictionary<uint, ScriptObject>();
            }
            Record[source][id] = scriptObject;
        }
        public void DelRecord(string source, uint id, ScriptObject scriptObject) {
            if (!IsRecord) { return; }
            if (Record.TryGetValue(source, out var dic)) {
                if (dic.ContainsKey(id)) {
                    dic.Remove(id);
                    return;
                }
            }
            if (!RecordDestroy.TryGetValue(source, out var list)) {
                RecordDestroy[source] = list = new List<string>();
            }
            // var str = scriptObject.ToString();
            // if (str.Length > 512) str = str.Substring(0, 512);
            list.Add($"{scriptObject.GetType()}");
        }
        public void EndRecord() {
            if (!IsRecord) return;
            IsRecord = false;
        }
        public void CollectLeak(out HashSet<int> set) {
            ScriptObjectReference.CollectLeak(this, out set);
        }
    }
}
