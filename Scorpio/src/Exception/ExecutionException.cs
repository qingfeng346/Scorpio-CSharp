using System;
using Scorpio;
using Scorpio.Compiler;
namespace Scorpio.Exception
{
    //执行代码异常
    class ExecutionException : ScriptException {
        public ExecutionException(string strMessage) : base(strMessage) { }
    }
}
