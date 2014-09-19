using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    public class LexerException : ScriptException
    {
        public LexerException(String strMessage, int iSourceLine)
            : base(" Line:" + (iSourceLine+1) + "    " + strMessage)
        {
        }
    }
}
