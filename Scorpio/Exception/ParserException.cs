using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;

namespace Scorpio.Exception
{
    public class ParserException : ScriptException
    {
        public ParserException(String strMessage) : base(strMessage) { }
        public ParserException(String strMessage, Token token)
            : base(" Line:" + token.SourceLine + "  Column:" + token.SourceChar + "  value[" + token.Lexeme.ToString() + "]    " + strMessage) 
        { }
    }
}
