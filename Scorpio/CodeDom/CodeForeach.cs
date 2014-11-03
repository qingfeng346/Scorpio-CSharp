using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    //foreach 循环  foreach ( element in pairs(table)) { }
    public class CodeForeach : CodeObject
    {
        public string Identifier;
        public CodeObject LoopObject;
        public ScriptContext Context;
    }
}
