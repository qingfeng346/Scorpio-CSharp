using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;

namespace Scorpio.CodeDom
{
    public class CodeFunction : CodeObject
    {
        public ScriptFunction Func;
        public CodeFunction(ScriptFunction func)
        {
            this.Func = func;
        }
    }
}
