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
        public ScriptContext Context;                       //for执行
        public ScriptContext BlockContext;                  //内容执行
        private Script m_Script;                            //脚本引擎
        public CodeFor(Script script)
        {
            m_Script = script;
            Context = new ScriptContext(script, null, null, Executable_Block.For);
        }
        public void SetContextExecutable(ScriptExecutable blockExecutable)
        {
            BlockExecutable = blockExecutable;
            BlockContext = new ScriptContext(m_Script, blockExecutable, null, Executable_Block.For);
        }
    }
}
