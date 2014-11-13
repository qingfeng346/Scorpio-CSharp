using System;
using Scorpio;
namespace Scorpio.CodeDom
{
    //返回一个继承ScriptObject的变量
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(Script script, object obj) { Object = script.CreateObject(obj); }
        public ScriptObject Object { get; private set; }
    }
}
