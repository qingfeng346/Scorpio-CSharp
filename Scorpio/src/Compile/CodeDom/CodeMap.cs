using System.Collections.Generic;
namespace Scorpio.Compile.CodeDom {
    //返回一个map类型 t = { a = "1", b = "2", function hello() { } }
    public class CodeMap : CodeObject {
        public class MapVariable {
            public object key;
            public CodeObject value;
            public MapVariable(object key, CodeObject value) {
                this.key = key;
                this.value = value;
            }
        }
        public bool onlyString;
        public List<MapVariable> Variables = new List<MapVariable>();
        public CodeMap(int line, bool onlyString) : base(line) {
            this.onlyString = onlyString;
        }
    }
}
