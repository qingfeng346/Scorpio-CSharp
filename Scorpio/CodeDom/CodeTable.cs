using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.CodeDom
{
    public class TableVariable
    {
        public object key;
        public CodeObject value;
        public TableVariable(object key, CodeObject value)
        {
            this.key = key;
            this.value = value;
        }
    }
    //返回一个table类型 t = { a = "1", b = "2", function hello() { } }
    public class CodeTable : CodeObject
    {
        public List<TableVariable> Variables = new List<TableVariable>();
        public List<ScriptFunction> Functions = new List<ScriptFunction>();
    }
}
