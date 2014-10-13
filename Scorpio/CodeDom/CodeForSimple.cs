using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Runtime;
using Scorpio.Collections;
namespace Scorpio.CodeDom
{
    // for (i=begin,finished,step)
    public class CodeForSimple : CodeObject
    {
        public string Identifier;
        public CodeObject Begin;
        public CodeObject Finished;
        public CodeObject Step;
        public ScriptExecutable BlockExecutable;            //for内容
        public ScriptContext BlockContext;                  //内容执行
        private Script m_Script;                            //脚本引擎
        public VariableDictionary variables;                //变量
        public CodeForSimple(Script script)
        {
            m_Script = script;
            variables = new VariableDictionary();
        }
        public void SetContextExecutable(ScriptExecutable blockExecutable)
        {
            BlockExecutable = blockExecutable;
            BlockContext = new ScriptContext(m_Script, blockExecutable, null, Executable_Block.For);
        }
    }
}
