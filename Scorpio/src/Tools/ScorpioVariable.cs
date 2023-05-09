using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Scorpio {
    internal static class ScorpioVariable {
        private static long newId = 0;
        public static long Id => newId++;
        private static ConcurrentDictionary<long, Dictionary<string, int>> StringVariables = new ConcurrentDictionary<long, Dictionary<string, int>>();
        private static ConcurrentDictionary<long, Dictionary<object, int>> ObjectVariables = new ConcurrentDictionary<long, Dictionary<object, int>>();
        public static string[] GetStringKeys(long id) {
            if (StringVariables.TryGetValue(id, out var variable)) {
                return variable.Keys.ToArray();
            }
            return Array.Empty<string>();
        }
        public static bool ContainsVariable(long id, string key) {
            if (StringVariables.TryGetValue(id, out var variable)) {
                return variable.ContainsKey(key);
            }
            return false;
        }
        public static bool TryGetVariable(long id, string key, out int index) {
            if (StringVariables.TryGetValue(id, out var variable)) {
                if (variable.TryGetValue(key, out index)) {
                    return true;
                }
            }
            index = default;
            return false;
        }
        public static int AddVariable(long id, string key) {
            if (StringVariables.TryGetValue(id, out var variable)) {
                return variable[key] = variable.Count;
            }
            StringVariables[id] = variable = new Dictionary<string, int>();
            return variable[key] = variable.Count;
        }
        public static void ReleaseStringId(long id) {
            StringVariables.TryRemove(id, out _);
        }

        public static object[] GetObjectKeys(long id) {
            if (ObjectVariables.TryGetValue(id, out var variable)) {
                return variable.Keys.ToArray();
            }
            return Array.Empty<object>();
        }
        public static bool ContainsVariable(long id, object key) {
            if (ObjectVariables.TryGetValue(id, out var variable)) {
                return variable.ContainsKey(key);
            }
            return false;
        }
        public static bool TryGetVariable(long id, object key, out int index) {
            if (ObjectVariables.TryGetValue(id, out var variable)) {
                if (variable.TryGetValue(key, out index)) {
                    return true;
                }
            }
            index = default;
            return false;
        }
        public static int AddVariable(long id, object key) {
            if (ObjectVariables.TryGetValue(id, out var variable)) {
                return variable[key] = variable.Count;
            }
            ObjectVariables[id] = variable = new Dictionary<object, int>();
            return variable[key] = variable.Count;
        }
        public static void ReleaseObjectId(long id) {
            ObjectVariables.TryRemove(id, out _);
        }
    }
}
