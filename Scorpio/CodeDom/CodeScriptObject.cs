using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.CodeDom
{
    //返回一个继承ScriptObject的变量
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(object obj) { Object = obj; }
        public object Object { get; private set; }
    }
}
