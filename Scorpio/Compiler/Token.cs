using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Compiler
{
    //脚本的表征
    public class Token
    {
        public TokenType Type { get; private set; }
        public object Lexeme { get; private set; }
        public int SourceLine { get; private set; }
        public int SourceChar { get; private set; }
        public Token(TokenType tokenType, object lexeme, int sourceLine, int sourceChar)
        {
            this.Type = tokenType;
            this.Lexeme = lexeme;
            this.SourceLine = sourceLine + 1;
            this.SourceChar = sourceChar;
        }
        public override String ToString()
        {
            return Type.ToString() + "(" + Lexeme.ToString() + ")";
        }
    }
}
