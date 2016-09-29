using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Compiler
{
    //脚本的表征
    public class Token
    {
        public TokenType Type { get; private set; }         //标记类型
        public object Lexeme { get; private set; }          //标记值
        public int SourceLine { get; private set; }         //所在行
        public int SourceChar { get; private set; }         //所在列
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
