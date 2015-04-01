using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    //词法分析程序
    public class LexerException : ScriptException
    {
        public LexerException(String strMessage) : base(strMessage) { }
    }
}
