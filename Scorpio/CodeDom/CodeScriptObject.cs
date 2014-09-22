using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.CodeDom
{
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(object obj) {
            Object = ScriptObject.CreateObject(obj);
        }
        public ScriptObject Object { get; private set; }
    }
}
