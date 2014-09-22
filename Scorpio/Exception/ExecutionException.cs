using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;

namespace Scorpio.Exception
{
    class ExecutionException : ScriptException
    {
        public ExecutionException(String strMessage, CodeObject obj) :
            base("Source[" + obj.Breviary + "] Line[" + obj.Line + "] Object[" + obj.ToString() + "] is error : " + strMessage)
        {
        }
    }
}
