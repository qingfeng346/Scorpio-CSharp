//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Runtime.InteropServices;

//namespace Scorpio {
//    internal static class ScorpioVariableUtil {
//        [StructLayout(LayoutKind.Auto, Pack = 1)]
//        internal struct Variable {
//            public uint keyIndex;
//            public ushort valueIndex;
//        }
//        private static Dictionary<string, uint> stringToIndex = new Dictionary<string, uint>();
//        private static Dictionary<uint, string> indexToString = new Dictionary<uint, string>();
//        private static uint stringIndex = 0;
//        public static uint StringToIndex(string key) {
//            if (stringToIndex.TryGetValue(key, out var index))
//                return index;
//            indexToString[stringIndex] = key;
//            return stringToIndex[key] = stringIndex++;
//        }
//        public static string IndexToString(uint index) {
//            if (indexToString.TryGetValue(index, out var key))
//                return key;
//            return null;
//        }
//        private static uint stringId = 0;
//        private static object stringLock = new object();
//        private static ConcurrentQueue<uint> cacheStringId = new ConcurrentQueue<uint>();
//        private static List<Variable>[] stringVariables = new List<Variable>[256];
//        public static long StingId {
//            get {
//                if (cacheStringId.TryDequeue(out var id)) {
//                    return id;
//                }
//                id = stringId++;
//                if (id >= stringVariables.Length) {
//                    lock (stringLock) {
//                        var array = new List<Variable>[stringVariables.Length * 32];
//                        Array.Copy(stringVariables, 0, array, 0, stringVariables.Length);
//                        stringVariables = array;
//                    }
//                }
//                return id;
//            }
//        }
//        private static ConcurrentDictionary<long, Dictionary<object, int>> ObjectVariables = new ConcurrentDictionary<long, Dictionary<object, int>>();
//        public static string[] GetStringKeys(long id) {
//            if (StringVariables.TryGetValue(id, out var variable)) {
//                return variable.Keys.ToArray();
//            }
//            return Array.Empty<string>();
//        }
//        public static bool ContainsVariable(long id, string key) {
//            if (StringVariables.TryGetValue(id, out var variable)) {
//                return variable.ContainsKey(key);
//            }
//            return false;
//        }
//        public static bool TryGetVariable(long id, string key, out int index) {
//            if (StringVariables.TryGetValue(id, out var variable)) {
//                if (variable.TryGetValue(key, out index)) {
//                    return true;
//                }
//            }
//            index = default;
//            return false;
//        }
//        public static int AddVariable(long id, string key) {
//            if (StringVariables.TryGetValue(id, out var variable)) {
//                return variable[key] = variable.Count;
//            }
//            StringVariables[id] = variable = new Dictionary<string, int>();
//            return variable[key] = variable.Count;
//        }
//        public static void ReleaseStringId(long id) {
//            StringVariables.TryRemove(id, out _);
//        }

//        public static object[] GetObjectKeys(long id) {
//            if (ObjectVariables.TryGetValue(id, out var variable)) {
//                return variable.Keys.ToArray();
//            }
//            return Array.Empty<object>();
//        }
//        public static bool ContainsVariable(long id, object key) {
//            if (ObjectVariables.TryGetValue(id, out var variable)) {
//                return variable.ContainsKey(key);
//            }
//            return false;
//        }
//        public static bool TryGetVariable(long id, object key, out int index) {
//            if (ObjectVariables.TryGetValue(id, out var variable)) {
//                if (variable.TryGetValue(key, out index)) {
//                    return true;
//                }
//            }
//            index = default;
//            return false;
//        }
//        public static int AddVariable(long id, object key) {
//            if (ObjectVariables.TryGetValue(id, out var variable)) {
//                return variable[key] = variable.Count;
//            }
//            ObjectVariables[id] = variable = new Dictionary<object, int>();
//            return variable[key] = variable.Count;
//        }
//        public static void ReleaseObjectId(long id) {
//            ObjectVariables.TryRemove(id, out _);
//        }
//    }
//}
