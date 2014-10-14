using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;

namespace Scorpio.Exception
{
    //解析语法异常
    public class ParserException : ScriptException
    {
        public ParserException(String strMessage) : base(strMessage) { }
        public ParserException(String strMessage, Token token)
            : base(" Line:" + token.SourceLine + "  Column:" + token.SourceChar + "  Type:" + token.Type + "  value[" + token.Lexeme.ToString() + "]    " + strMessage) 
        { }
    }
}
