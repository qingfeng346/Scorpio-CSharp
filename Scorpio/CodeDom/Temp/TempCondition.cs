using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Runtime;
namespace Scorpio.CodeDom.Temp
{
    /// <summary> if语句中一个 if语句 </summary>
    internal class TempCondition {
        public ScriptExecutable Executable;        //指令列表
        public Executable_Block Block;             //指令域类型
        public CodeObject Allow;                    //判断条件
        public TempCondition(CodeObject allow, ScriptExecutable executable, Executable_Block block) {
            this.Allow = allow;
            this.Executable = executable;
            this.Block = block;
        }
    }
}
