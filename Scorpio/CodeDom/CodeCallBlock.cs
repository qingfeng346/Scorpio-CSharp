using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Runtime;

namespace Scorpio.CodeDom {
    public class CodeCallBlock : CodeObject {
        public ScriptExecutable Executable;
        public CodeCallBlock(ScriptExecutable executable) {
            this.Executable = executable;
        }
    }
}
