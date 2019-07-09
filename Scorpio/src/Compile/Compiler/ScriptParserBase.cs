using System;
using System.Collections.Generic;
using Scorpio.Compile.Exception;
namespace Scorpio.Compile.Compiler {
    /// <summary> 编译脚本 </summary>
    public partial class ScriptParser {
        private string m_strBreviary;           //当前解析的脚本摘要
        private Token[] m_listTokens;           //token列表
        private int m_indexToken;               //当前读到token
        public ScriptParser(List<Token> listTokens, string strBreviary) {
            m_strBreviary = strBreviary;
            m_listTokens = listTokens.ToArray();
        }
        /// <summary> 是否还有更多需要解析的语法 </summary>
        bool HasMoreTokens() {
            return m_indexToken < m_listTokens.Length;
        }
        int GetSourceLine() {
            return PeekToken().SourceLine;
        }
        /// <summary> 获得第一个Token </summary>
        Token ReadToken() {
            if (!HasMoreTokens())
                throw new LexerException("Unexpected end of token stream.");
            return m_listTokens[m_indexToken++];
        }
        /// <summary> 返回第一个Token </summary>
        Token PeekToken() {
            if (!HasMoreTokens())
                throw new LexerException("Unexpected end of token stream.");
            return m_listTokens[m_indexToken];
        }
        /// <summary> 回滚Token </summary>
        void UndoToken() {
            if (m_indexToken <= 0)
                throw new LexerException("No more tokens to undo.");
            --m_indexToken;
        }
        /// <summary> 读取, </summary>
        void ReadComma() {
            Token token = ReadToken();
            if (token.Type != TokenType.Comma)
                throw new ParserException("Comma ',' expected.", token);
        }
        /// <summary> 读取 变量字符 </summary>
        String ReadIdentifier() {
            Token token = ReadToken();
            if (token.Type != TokenType.Identifier)
                throw new ParserException("Identifier expected.", token);
            return token.Lexeme.ToString();
        }
        /// <summary> 读取{ </summary>
        void ReadLeftBrace() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBrace)
                throw new ParserException("Left brace '{' expected.", token);
        }
        /// <summary> 读取} </summary>
        void ReadRightBrace() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBrace)
                throw new ParserException("Right brace '}' expected.", token);
        }
        /// <summary> 读取[ </summary>
        void ReadLeftBracket() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBracket)
                throw new ParserException("Left bracket '[' expected for array indexing expression.", token);
        }
        /// <summary> 读取] </summary>
        void ReadRightBracket() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBracket)
                throw new ParserException("Right bracket ']' expected for array indexing expression.", token);
        }
        /// <summary> 读取( </summary>
        void ReadLeftParenthesis() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftPar)
                throw new ParserException("Left parenthesis '(' expected.", token);
        }
        /// <summary> 读取) </summary>
        void ReadRightParenthesis() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightPar)
                throw new ParserException("Right parenthesis ')' expected.", token);
        }
        /// <summary> 读取; </summary>
        void ReadSemiColon() {
            Token token = ReadToken();
            if (token.Type != TokenType.SemiColon)
                throw new ParserException("SemiColon ';' expected.", token);
        }
        /// <summary> 读取in </summary>
        void ReadIn() {
            Token token = ReadToken();
            if (token.Type != TokenType.In)
                throw new ParserException("In 'in' expected.", token);
        }
        /// <summary> 读取: </summary>
        void ReadColon() {
            Token token = ReadToken();
            if (token.Type != TokenType.Colon)
                throw new ParserException("Colon ':' expected.", token);
        }
        /// <summary> 读取catch </summary>
        void ReadCatch() {
            Token token = ReadToken();
            if (token.Type != TokenType.Catch)
                throw new ParserException("Catch 'catch' expected.", token);
        }
    }
}
