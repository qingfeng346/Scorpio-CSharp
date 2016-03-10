using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Compiler
{
    public partial class ScriptParser
    {
        /// <summary> 是否还有更多需要解析的语法 </summary>
        protected bool HasMoreTokens()
        {
            return m_iNextToken < m_listTokens.Count;
        }
        /// <summary> 获得第一个Token </summary>
        protected Token ReadToken()
        {
            if (!HasMoreTokens())
                throw new LexerException("Unexpected end of token stream.");
            return m_listTokens[m_iNextToken++];
        }
        /// <summary> 返回第一个Token </summary>
        protected Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new LexerException("Unexpected end of token stream.");
            return m_listTokens[m_iNextToken];
        }
        /// <summary> 回滚Token </summary>
        protected void UndoToken()
        {
            if (m_iNextToken <= 0)
                throw new LexerException("No more tokens to undo.");
            --m_iNextToken;
        }
        /// <summary> 读取, </summary>
        protected void ReadComma()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Comma)
                throw new ParserException("Comma ',' expected.", token);
        }
        /// <summary> 读取 未知字符 </summary>
        protected String ReadIdentifier()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Identifier)
                throw new ParserException("Identifier expected.", token);
            return token.Lexeme.ToString();
        }
        /// <summary> 读取{ </summary>
        protected void ReadLeftBrace()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBrace)
                throw new ParserException("Left brace '{' expected.", token);
        }
        /// <summary> 读取} </summary>
        protected void ReadRightBrace()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBrace)
                throw new ParserException("Right brace '}' expected.", token);
        }
        /// <summary> 读取[ </summary>
        protected void ReadLeftBracket()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBracket)
                throw new ParserException("Left bracket '[' expected for array indexing expression.", token);
        }
        /// <summary> 读取] </summary>
        protected void ReadRightBracket()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBracket)
                throw new ParserException("Right bracket ']' expected for array indexing expression.",token);
        }
        /// <summary> 读取( </summary>
        protected void ReadLeftParenthesis()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftPar)
                throw new ParserException("Left parenthesis '(' expected.", token);
        }
        /// <summary> 读取) </summary>
        protected void ReadRightParenthesis()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightPar)
                throw new ParserException("Right parenthesis ')' expected.", token);
        }
        /// <summary> 读取; </summary>
        protected void ReadSemiColon()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.SemiColon)
                throw new ParserException("SemiColon ';' expected.", token);
        }
        /// <summary> 读取in </summary>
        protected void ReadIn()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.In)
                throw new ParserException("In 'in' expected.", token);
        }
        /// <summary> 读取: </summary>
        protected void ReadColon()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Colon)
                throw new ParserException("Colon ':' expected.", token);
        }
        /// <summary> 读取catch </summary>
        protected void ReadCatch()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Catch)
                throw new ParserException("Catch 'catch' expected.", token);
        }
    }
}
