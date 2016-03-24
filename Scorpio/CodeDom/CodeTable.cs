using System.Collections.Generic;
using Scorpio;
namespace Scorpio.CodeDom
{
    //返回一个table类型 t = { a = "1", b = "2", function hello() { } }
    public class CodeTable : CodeObject
    {
        public class TableVariable {
            public object key;
            public CodeObject value;
            public TableVariable(object key, CodeObject value) {
                this.key = key;
                this.value = value;
            }
        }
        public List<TableVariable> _Variables = new List<TableVariable>();
        public List<ScriptFunction> _Functions = new List<ScriptFunction>();
        public TableVariable[] Variables;
        public ScriptFunction[] Functions;
        public void Init() {
            Variables = _Variables.ToArray();
            Functions = _Functions.ToArray();
        }
    }
}
