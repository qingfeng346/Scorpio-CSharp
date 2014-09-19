using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    public class CodeForeach : CodeObject
    {
        public string Identifier;
        public CodeObject LoopObject;
        public ScriptExecutable Executable;
    }
}
