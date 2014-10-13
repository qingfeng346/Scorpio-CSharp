using System;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.Exception
{
    //执行代码异常
    class ExecutionException : ScriptException
    {
        public ExecutionException(String strMessage) : base(strMessage)
        {
        }
    }
}
