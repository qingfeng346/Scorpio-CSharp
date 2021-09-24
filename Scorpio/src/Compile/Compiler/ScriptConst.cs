using System.Collections.Generic;

namespace Scorpio.Compile.Compiler {
    public class ScriptConst {
        public Dictionary<string, object> values = new Dictionary<string, object>();
        public object Get(string key, out bool contains) {
            if (values.TryGetValue(key, out var value)) {
                contains = true;
                return value;
            }
            contains = false;
            return null;
        }
        public void Add(string key, object value) {
            values[key] = value;
        }
    }
}
