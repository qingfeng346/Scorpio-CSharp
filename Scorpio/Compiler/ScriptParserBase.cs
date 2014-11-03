using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Compiler
{
    internal partial class ScriptParser
    {
        /// <summary> 是否还有更多需要解析的语法 </summary>
        private bool HasMoreTokens()
        {
            return m_iNextToken < m_listTokens.Count;
        }
        /// <summary> 获得第一个Token </summary>
        private Token ReadToken()
        {
            if (!HasMoreTokens())
                throw new ScriptException("Unexpected end of token stream.");
            return m_listTokens[m_iNextToken++];
        }
        /// <summary> 返回第一个Token </summary>
        private Token PeekToken()
        {
            if (!HasMoreTokens())
                throw new ScriptException("Unexpected end of token stream.");
            return m_listTokens[m_iNextToken];
        }
        /// <summary> 回滚Token </summary>
        private void UndoToken()
        {
            if (m_iNextToken <= 0)
                throw new ScriptException("No more tokens to undo.");
            --m_iNextToken;
        }
        /// <summary> 读取, </summary>
        private void ReadComma()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Comma)
                throw new ParserException("Comma ',' expected.", token);
        }
        /// <summary> 读取 未知字符 </summary>
        private String ReadIdentifier()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Identifier)
                throw new ParserException("Identifier expected.", token);
            return token.Lexeme.ToString();
        }
        /// <summary> 读取{ </summary>
        private void ReadLeftBrace()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBrace)
                throw new ParserException("Left brace '{' expected.", token);
        }
        /// <summary> 读取} </summary>
        private void ReadRightBrace()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBrace)
                throw new ParserException("Right brace '}' expected.", token);
        }
        /// <summary> 读取[ </summary>
        private void ReadLeftBracket()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBracket)
                throw new ParserException("Left bracket '[' expected for array indexing expression.", token);
        }
        /// <summary> 读取] </summary>
        private void ReadRightBracket()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBracket)
                throw new ParserException("Right bracket ']' expected for array indexing expression.",token);
        }
        /// <summary> 读取( </summary>
        private void ReadLeftParenthesis()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftPar)
                throw new ParserException("Left parenthesis '(' expected.", token);
        }
        /// <summary> 读取) </summary>
        private void ReadRightParenthesis()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.RightPar)
                throw new ParserException("Right parenthesis ')' expected.", token);
        }
        /// <summary> 读取; </summary>
        private void ReadSemiColon()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.SemiColon)
                throw new ParserException("SemiColon ';' expected.", token);
        }
        /// <summary> 读取in </summary>
        private void ReadIn()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.In)
                throw new ParserException("In 'in' expected.", token);
        }
        /// <summary> 读取: </summary>
        private void ReadColon()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Colon)
                throw new ParserException("Colon ':' expected.", token);
        }
        /// <summary> 读取catch </summary>
        private void ReadCatch()
        {
            Token token = ReadToken();
            if (token.Type != TokenType.Catch)
                throw new ParserException("Catch 'catch' expected.", token);
        }
    }
}
