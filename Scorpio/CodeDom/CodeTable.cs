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
            this.Key = key;
            this.Value = value;
        }
    }
    public class CodeTable : CodeObject
    {
        public List<TableVariable> Variables = new List<TableVariable>();
        public List<ScriptFunction> Functions = new List<ScriptFunction>();
    }
}
