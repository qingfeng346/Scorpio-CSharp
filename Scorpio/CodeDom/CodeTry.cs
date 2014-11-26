using Scorpio.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.CodeDom
{
    //try catch 语句
    public class CodeTry : CodeObject
    {
        public ScriptContext TryContext;            //try指令执行
        public ScriptContext CatchContext;          //catch指令执行
        public string Identifier;                   //异常对象
    }
}
