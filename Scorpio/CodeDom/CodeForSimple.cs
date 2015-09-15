using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    // for (i=begin,finished,step)
    public class CodeForSimple : CodeObject {
        private Script m_Script;                            //脚本引擎
        public string Identifier;
        public CodeObject Begin;
        public CodeObject Finished;
        public CodeObject Step;
        public ScriptExecutable BlockExecutable;            //for内容
        public CodeForSimple(Script script) {
            m_Script = script;
        }
        public ScriptContext GetBlockContext() {
            return new ScriptContext(m_Script, BlockExecutable, null, Executable_Block.For);
        }
    }
}
