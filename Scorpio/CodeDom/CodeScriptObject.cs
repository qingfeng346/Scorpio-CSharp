using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.CodeDom
{
    //返回一个继承ScriptObject的变量
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(ScriptObject obj) { Object = obj; }
        public ScriptObject Object { get; private set; }
    }
}
