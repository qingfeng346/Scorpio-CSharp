using Scorpio.Runtime;
using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    public class GlobalCacheCompiler {
        public int Index { get; private set; }
        private List<double> DoubleList { get; set; } = new List<double>();     //所有的常量 double
        private Dictionary<double, int> DoubleMaps = new Dictionary<double, int>();
        private List<long> LongList { get; set; } = new List<long>();           //所有的常量 long
        private Dictionary<long, int> LongMaps = new Dictionary<long, int>();
        private List<string> StringList { get; set; } = new List<string>();     //所有的常量 string
        private Dictionary<string, int> StringMaps = new Dictionary<string, int>();
        private GlobalCache _globalCaches = null;
        public GlobalCache GlobalCache => _globalCaches ?? (_globalCaches = new GlobalCache(DoubleList.ToArray(), LongList.ToArray(), StringList.ToArray()));
        public GlobalCacheCompiler(int index) {
            Index = index;
        }
        /// <summary> 获取一个double常量的索引 </summary>
        public int GetConstDouble(double value) {
            if (!DoubleMaps.TryGetValue(value, out var index)) {
                index = DoubleList.Count;
                DoubleList.Add(value);
                DoubleMaps[value] = index;
            }
            return index;
        }
        /// <summary> 获取一个long常量的索引 </summary>
        public int GetConstLong(long value) {
            if (!LongMaps.TryGetValue(value, out var index)) {
                index = LongList.Count;
                LongList.Add(value);
                LongMaps[value] = index;
            }
            return index;
        }
        /// <summary> 获取一个string常量的索引 </summary>
        public int GetConstString(string value) {
            if (!StringMaps.TryGetValue(value, out var index)) {
                index = StringList.Count;
                StringList.Add(value);
                StringMaps[value] = index;
            }
            return index;
        }
    }
}
