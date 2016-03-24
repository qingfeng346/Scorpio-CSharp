using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    // for (i=begin,finished,step)
    public class CodeForSimple : CodeObject {
        public string Identifier;
        public CodeObject Begin;
        public CodeObject Finished;
        public CodeObject Step;
        public ScriptExecutable BlockExecutable;            //for内容
    }
}
