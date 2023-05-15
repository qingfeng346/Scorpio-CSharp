using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Scorpio {
    internal class VariableFactory<Key> {
        private uint id = 0;
        private object sync = new object();
        private ConcurrentQueue<uint> cacheId = new ConcurrentQueue<uint>();
        public uint Id {
            get {
                if (cacheId.TryDequeue(out var id)) {
                    return id;
                }
                id = this.id++;
                if (id >= variables.Length) {
                    lock (sync) {
                        var array = new Dictionary<Key, int>[variables.Length * 32];
                        Array.Copy(variables, 0, array, 0, variables.Length);
                        variables = array;
                    }
                }
                return id;
            }
        }
        private Dictionary<Key, int>[] variables = new Dictionary<Key, int>[256];
        public Key[] GetKeys(uint id) {
            if (variables[id] == null) return Array.Empty<Key>();
            return variables[id].Keys.ToArray();
        }
        public bool ContainsVariable(uint id, Key key) {
            if (variables[id] == null) return false;
            return variables[id].ContainsKey(key);
        }
        public bool TryGetVariable(uint id, Key key, out int index) {
            if (variables[id] == null) {
                index = default;
                return false;
            }
            if (variables[id].TryGetValue(key, out index)) {
                return true;
            }
            index = default;
            return false;
        }
        public int AddVariable(uint id, Key key) {
            var variable = variables[id];
            if (variable == null) {
                variables[id] = variable = new Dictionary<Key, int>();
            }
            return variable[key] = variable.Count;
        }
        public void ReleaseId(uint id) {
            lock (sync) {
                variables[id] = null;
            }
            cacheId.Enqueue(id);
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

        private static VariableFactory<object> objectFactory = new VariableFactory<object>();
        public static uint ObjectId => objectFactory.Id;
        public static object[] GetObjectKeys(uint id) => objectFactory.GetKeys(id);
        public static bool ContainsVariable(uint id, object key) => objectFactory.ContainsVariable(id, key);
        public static bool TryGetVariable(uint id, object key, out int index) => objectFactory.TryGetVariable(id, key, out index);
        public static int AddVariable(uint id, object key) => objectFactory.AddVariable(id, key);
        public static void ReleaseObjectId(uint id) => objectFactory.ReleaseId(id);
    }
}
