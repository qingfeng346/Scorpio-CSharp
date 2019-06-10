using System;
using Scorpio;
using Scorpio.Compiler;
namespace Scorpio.Exception
{
    //执行代码异常
    public class ExecutionException : ScriptException {
        public ExecutionException(string strMessage) : base(strMessage) { }
    }
    public class ExecutionStackException : ScriptException {
        public ExecutionStackException(string strMessage) : base(strMessage) { }
    }
}
