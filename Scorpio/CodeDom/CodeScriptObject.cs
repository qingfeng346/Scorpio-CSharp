using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.CodeDom
{
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(ScriptObject obj) {
            Object = obj;
        }
        public ScriptObject Object { get; private set; }
    }
}
