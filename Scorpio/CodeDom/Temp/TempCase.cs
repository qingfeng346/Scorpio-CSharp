using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Runtime;
namespace Scorpio.CodeDom.Temp
{
    /// <summary> switch语句中一个cast条件</summary>
    internal class TempCase {
        private Script m_Script;
        private ScriptExecutable Executable;        //指令列表
        private Executable_Block Block;
        public List<CodeObject> Allow;              //判断条件
        public TempCase(Script script, List<CodeObject> allow, ScriptExecutable executable, Executable_Block block) {
            m_Script = script;
            this.Allow = allow;
            this.Executable = executable;
            this.Block = block;
        }
        public ScriptContext GetContext() {
            return new ScriptContext(m_Script, this.Executable, null, this.Block);
        }
    }
}
