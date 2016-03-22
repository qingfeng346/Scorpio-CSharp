using System;
using Scorpio;
namespace Scorpio.Exception
{
    //执行代码异常
    class ExecutionException : ScriptException
    {
        private string m_Source = "";
        public ExecutionException(Script script, String strMessage) : base(strMessage)
        {
            if (script != null) {
                StackInfo stackInfo = script.GetCurrentStackInfo();
                if (stackInfo != null) m_Source = stackInfo.Breviary + ":" + stackInfo.Line + ": ";
            }
        }
        public override string Message { get { return m_Source + base.Message; } }
    }
}
