using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    class ExecutionException : ScriptException
    {
        public ExecutionException(String strMessage)
            : base(strMessage)
        {

        }
    }
}
