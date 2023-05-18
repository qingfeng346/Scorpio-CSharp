using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Scorpio.Tools {  
    internal class VariableHashFactory<Key> {
        private static uint id = 0;
        private static ConcurrentQueue<uint> cacheId = new ConcurrentQueue<uint>();
        public static uint Id {
            get {
                if (cacheId.TryDequeue(out var ret)) {
                    return ret;
                }
                return id++;
            }
        }
        private static ConcurrentDictionary<uint, Dictionary<Key, int>> variables = new ConcurrentDictionary<uint, Dictionary<Key, int>>();
        public static Key[] GetKeys(uint id) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.Keys.ToArray();
            }
            return Array.Empty<Key>();
        }
        public static bool ContainsVariable(uint id, Key key) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable.ContainsKey(key);
            }
            return false;
        }
        public static bool TryGetVariable(uint id, Key key, out int index) {
            if (variables.TryGetValue(id, out var variable)) {
                if (variable.TryGetValue(key, out index)) {
                    return true;
                }
            }
            index = default;
            return false;
        }
        public static int AddVariable(uint id, Key key) {
            if (variables.TryGetValue(id, out var variable)) {
                return variable[key] = variable.Count;
            }
            variables[id] = variable = new Dictionary<Key, int>();
            return variable[key] = 0;
        }
        public static void ReleaseId(uint id) {
            variables.TryRemove(id, out _);
            cacheId.Enqueue(id);
        }
    }
}
