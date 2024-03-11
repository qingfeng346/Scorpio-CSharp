using Scorpio.Runtime;
using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    public class GlobalCacheCompiler {
        private object sync = new object();
        public int Index { get; private set; }
        private List<double> DoubleList = new List<double>();     //所有的常量 double
        private Dictionary<double, int> DoubleMaps = new Dictionary<double, int>();
        private List<long> LongList = new List<long>();           //所有的常量 long
        private Dictionary<long, int> LongMaps = new Dictionary<long, int>();
        private List<string> StringList = new List<string>();     //所有的常量 string
        private Dictionary<string, int> StringMaps = new Dictionary<string, int>();
        private GlobalCache _globalCaches = new GlobalCache();
        private int Changed = 1 | 2 | 4;
        public GlobalCache GlobalCache {
            get {
                lock (sync) {
                    if ((Changed & 1) != 0) {
                        Changed ^= 1;
                        _globalCaches.ConstDouble = DoubleList.ToArray();
                    }
                    if ((Changed & 2) != 0) {
                        Changed ^= 2;
                        _globalCaches.ConstLong = LongList.ToArray();
                    }
                    if ((Changed & 4) != 0) {
                        Changed ^= 4;
                        _globalCaches.ConstString = StringList.ToArray();
                    }
                    return _globalCaches;
                }
            }
        }
        public GlobalCacheCompiler(int index) {
            Index = index;
        }
        /// <summary> 获取一个double常量的索引 </summary>
        public int GetConstDouble(double value) {
            lock (sync) {
                if (!DoubleMaps.TryGetValue(value, out var index)) {
                    Changed |= 1;
                    index = DoubleList.Count;
                    DoubleList.Add(value);
                    DoubleMaps[value] = index;
                }
                return index;
            }
        }
        /// <summary> 获取一个long常量的索引 </summary>
        public int GetConstLong(long value) {
            lock (sync) {
                if (!LongMaps.TryGetValue(value, out var index)) {
                    Changed |= 2;
                    index = LongList.Count;
                    LongList.Add(value);
                    LongMaps[value] = index;
                }
                return index;
            }
        }
        /// <summary> 获取一个string常量的索引 </summary>
        public int GetConstString(string value) {
            lock (sync) {
                if (!StringMaps.TryGetValue(value, out var index)) {
                    Changed |= 4;
                    index = StringList.Count;
                    StringList.Add(value);
                    StringMaps[value] = index;
                }
                return index;
            }
        }
    }
}
