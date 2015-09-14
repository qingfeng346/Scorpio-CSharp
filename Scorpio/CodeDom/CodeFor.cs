using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    //for循环 for (var i=0;i<10;++i) {}
    public class CodeFor : CodeObject
    {
        public ScriptExecutable BeginExecutable;            //开始执行
        public CodeObject Condition;                        //跳出条件
        public ScriptExecutable LoopExecutable;             //循环执行
        public ScriptExecutable BlockExecutable;            //for内容
        private Script m_Script;                            //脚本引擎
        public CodeFor(Script script) {
            m_Script = script;
        }
        public ScriptContext GetContext() {
            return new ScriptContext(m_Script, null, null, Executable_Block.For);
        }
        public ScriptContext GetBlockContext() {
            return new ScriptContext(m_Script, BlockExecutable, null, Executable_Block.For);
        }
    }
}
