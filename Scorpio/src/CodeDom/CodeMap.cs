using System.Collections.Generic;
namespace Scorpio.CodeDom {
    //返回一个map类型 t = { a = "1", b = "2", function hello() { } }
    public class CodeMap : CodeObject {
        public class MapVariable {
            public string key;
            public CodeObject value;
            public MapVariable(string key, CodeObject value) {
                this.key = key;
                this.value = value;
            }
        }
        public List<MapVariable> Variables = new List<MapVariable>();
        public CodeMap(int line) : base(line) { }
    }
}
