using System;
using Scorpio;
namespace Scorpio.CodeDom
{
    //返回一个继承ScriptObject的变量
    public class CodeScriptObject : CodeObject
    {
        public CodeScriptObject(Script script, object obj) {
            Object = script.CreateObject(obj);
            Object.Name = "" + obj;
        }
        public CodeScriptObject(Script script, object obj, string breviary, int line) : base(breviary, line) {
            Object = script.CreateObject(obj);
            Object.Name = "" + obj;
        }
        public ScriptObject Object { get; private set; }
    }
}
