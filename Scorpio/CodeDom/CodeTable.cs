using System.Collections.Generic;
using Scorpio.Function;
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
        public List<ScriptScriptFunction> _Functions = new List<ScriptScriptFunction>();
        public TableVariable[] Variables;
        public ScriptScriptFunction[] Functions;
        public void Init() {
            Variables = _Variables.ToArray();
            Functions = _Functions.ToArray();
        }
    }
}
