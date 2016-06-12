using System;
using Scorpio;
namespace Scorpio.CodeDom
{
    //返回一个继承ScriptObject的变量
    public class CodeScriptObject : CodeObject
    {
        public ScriptObject Object;
        public CodeScriptObject(Script script, object obj) {
            Object = script.CreateObject(obj);
            Object.Name = "" + obj;
        }
    }
}
