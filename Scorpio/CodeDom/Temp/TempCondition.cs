using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Runtime;
namespace Scorpio.CodeDom.Temp
{
    internal class TempCondition
    {
        public CodeObject Allow;                        //判断条件
        public ScriptExecutable Executable;             //指令列表
        public ScriptContext Context;                   //指令执行
        public TempCondition(Script script, CodeObject allow, ScriptExecutable executable, Executable_Block block)
        {
            this.Allow = allow;
            this.Executable = executable;
            this.Context = new ScriptContext(script, executable, null, block);
        }
    }
}
