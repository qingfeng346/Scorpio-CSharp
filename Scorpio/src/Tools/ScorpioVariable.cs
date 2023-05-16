using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Scorpio {
    internal class VariableFactory<Key> {
        private uint id = 0;
        private ConcurrentQueue<uint> cacheId = new ConcurrentQueue<uint>();
        public uint Id {
            get {
                if (cacheId.TryDequeue(out var id)) {
                    return id;
                }
                id = this.id++;
                return id;
            }
        }
        private ConcurrentDictionary<uint, List<Key>> variables = new ConcurrentDictionary<uint, List<Key>>();
        public Key[] GetKeys(uint id) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.ToArray();
            }
            return Array.Empty<Key>();
        }
        public bool ContainsVariable(uint id, Key key) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.Contains(key);
            }
            return false;
        }
        public bool TryGetVariable(uint id, Key key, out int index) {
            if (variables.TryGetValue(id, out var variable)) {
                index = variable.IndexOf(key);
                return index >= 0;
            }
            index = default;
            return false;
        }
        public int AddVariable(uint id, Key key) {
            if (!variables.TryGetValue(id, out var variable)) {
                variables[id] = variable = new List<Key>();
            }
            variable.Add(key);
            return variable.Count - 1;
        }
        public void ReleaseId(uint id) {
            variables.TryRemove(id, out _);
            cacheId.Enqueue(id);
        }
        public void TrimExcess() {
            foreach (var pair in variables) {
                pair.Value.TrimExcess();
            }
        }
    }
    internal class VariableHashFactory<Key> {
        private uint id = 0;
        private ConcurrentQueue<uint> cacheId = new ConcurrentQueue<uint>();
        public uint Id {
            get {
                if (cacheId.TryDequeue(out var id)) {
                    return id;
                }
                id = this.id++;
                return id;
            }
        }
        private ConcurrentDictionary<uint, Dictionary<Key, int>> variables = new ConcurrentDictionary<uint, Dictionary<Key, int>>();
        public Key[] GetKeys(uint id) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.Keys.ToArray();
            }
            return Array.Empty<Key>();
        }
        public bool ContainsVariable(uint id, Key key) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.ContainsKey(key);
            }
            return false;
        }
        public bool TryGetVariable(uint id, Key key, out int index) {
            if (variables.TryGetValue(id, out var variable)) {
                if (variable.TryGetValue(key, out index)) {
                    return true;
                }
            }
            index = default;
            return false;
        }
        public int AddVariable(uint id, Key key) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable[key] = variable.Count;
            }
            variables[id] = variable = new Dictionary<Key, int>();
            return variable[key] = 0;
        }
        public void ReleaseId(uint id) {
            variables.TryRemove(id, out _);
            cacheId.Enqueue(id);
        }
        public void TrimExcess() {
            
        }
    }
    internal static class ScorpioVariable {
        private static VariableFactory<string> stringFactory = new VariableFactory<string>();
        public static uint StringId => stringFactory.Id;
        public static string[] GetStringKeys(uint id) => stringFactory.GetKeys(id);
        public static bool ContainsVariable(uint id, string key) => stringFactory.ContainsVariable(id, key);
        public static bool TryGetVariable(uint id, string key, out int index) => stringFactory.TryGetVariable(id, key, out index);
        public static int AddVariable(uint id, string key) => stringFactory.AddVariable(id, key);
        public static void ReleaseStringId(uint id) => stringFactory.ReleaseId(id);

        private static VariableHashFactory<object> objectFactory = new VariableHashFactory<object>();
        public static uint ObjectId => objectFactory.Id;
        public static object[] GetObjectKeys(uint id) => objectFactory.GetKeys(id);
        public static bool ContainsVariable(uint id, object key) => objectFactory.ContainsVariable(id, key);
        public static bool TryGetVariable(uint id, object key, out int index) => objectFactory.TryGetVariable(id, key, out index);
        public static int AddVariable(uint id, object key) => objectFactory.AddVariable(id, key);
        public static void ReleaseObjectId(uint id) => objectFactory.ReleaseId(id);
    }
}
