using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.CodeDom
{
    public class TableVariable
    {
        public string Key;
        public CodeObject Value;
        public TableVariable(string key, CodeObject value)
        {
            Key = key;
            Value = value;
        }
    }
    //返回一个table类型 t = { a = "1", b = "2", function hello() { } }
    public class CodeTable : CodeObject
    {
        public List<TableVariable> Variables = new List<TableVariable>();
        public List<ScriptFunction> Functions = new List<ScriptFunction>();
    }
}
