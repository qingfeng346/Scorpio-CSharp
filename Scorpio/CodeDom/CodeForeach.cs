using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    //foreach 循环  foreach ( element in pairs(table)) { }
    public class CodeForeach : CodeObject
    {
        public string Identifier;
        public CodeObject LoopObject;
        public ScriptExecutable BlockExecutable;            //for内容
        private Script m_Script;                            //脚本引擎
        public CodeForeach(Script script)
        {
            m_Script = script;
        }
        public ScriptContext GetBlockContext() {
            return new ScriptContext(m_Script, BlockExecutable, null, Executable_Block.Foreach);
        }
    }
}
