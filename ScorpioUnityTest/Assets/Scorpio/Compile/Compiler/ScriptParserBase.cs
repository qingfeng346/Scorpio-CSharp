using System;
using System.Collections.Generic;
using Scorpio.Compile.Exception;
namespace Scorpio.Compile.Compiler {
    /// <summary> 编译脚本 </summary>
    public partial class ScriptParser {
        private Token[] m_listTokens;           //token列表
        private int m_indexToken;               //当前读到token
        private string[] ignoreFunctions;       //编译忽略的全局函数
        private HashSet<string> defines;        //define
        public ScriptParser(Token[] listTokens, string strBreviary, string[] ignoreFunctions, string[] defines) {
            this.Breviary = strBreviary;
            this.m_listTokens = listTokens;
            this.ignoreFunctions = ignoreFunctions;
            this.defines = new HashSet<string>();
            if (defines != null) { this.defines.UnionWith(defines); }
        }
        /// <summary> 当前解析的脚本摘要 </summary>
        public string Breviary { get; private set; }
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
                throw new ParserException(this, "ReadToken - 没有更多的Token");
            return m_listTokens[m_indexToken++];
        }
        /// <summary> 返回第一个Token </summary>
        Token PeekToken() {
            if (!HasMoreTokens())
                throw new ParserException(this, "PeekToken - 没有更多的Token");
            return m_listTokens[m_indexToken];
        }
        Token LastToken() {
            if (m_indexToken <= 0)
                throw new ParserException(this, "LastToken - 没有更早的Token");
            return m_listTokens[m_indexToken - 1];
        }
        /// <summary> 回滚Token </summary>
        void UndoToken() {
            if (m_indexToken <= 0)
                throw new ParserException(this, "UndoToken - 没有更早的Token");
            --m_indexToken;
        }
        /// <summary> 读取, </summary>
        void ReadComma() {
            Token token = ReadToken();
            if (token.Type != TokenType.Comma)
                throw new ParserException(this, "Comma ',' expected.", token);
        }
        /// <summary> 读取 变量字符 </summary>
        string ReadIdentifier() {
            Token token = ReadToken();
            if (token.Type != TokenType.Identifier)
                throw new ParserException(this, "Identifier expected.", token);
            return token.Lexeme.ToString();
        }
        /// <summary> 读取 { </summary>
        void ReadLeftBrace() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBrace)
                throw new ParserException(this, "Left brace '{' expected.", token);
        }
        /// <summary> 读取 } </summary>
        void ReadRightBrace() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBrace)
                throw new ParserException(this, "Right brace '}' expected.", token);
        }
        /// <summary> 读取 [ </summary>
        void ReadLeftBracket() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftBracket)
                throw new ParserException(this, "Left bracket '[' expected for array indexing expression.", token);
        }
        /// <summary> 读取 ] </summary>
        void ReadRightBracket() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightBracket)
                throw new ParserException(this, "Right bracket ']' expected for array indexing expression.", token);
        }
        /// <summary> 读取 ( </summary>
        void ReadLeftParenthesis() {
            Token token = ReadToken();
            if (token.Type != TokenType.LeftPar)
                throw new ParserException(this, "Left parenthesis '(' expected.", token);
        }
        /// <summary> 读取 ) </summary>
        void ReadRightParenthesis() {
            Token token = ReadToken();
            if (token.Type != TokenType.RightPar)
                throw new ParserException(this, "Right parenthesis ')' expected.", token);
        }
        /// <summary> 读取 ; </summary>
        void ReadSemiColon() {
            Token token = ReadToken();
            if (token.Type != TokenType.SemiColon)
                throw new ParserException(this, "SemiColon ';' expected.", token);
        }
        /// <summary> 读取 in </summary>
        void ReadIn() {
            Token token = ReadToken();
            if (token.Type != TokenType.In)
                throw new ParserException(this, "In 'in' expected.", token);
        }
        /// <summary> 读取 : </summary>
        void ReadColon() {
            Token token = ReadToken();
            if (token.Type != TokenType.Colon)
                throw new ParserException(this, "Colon ':' expected.", token);
        }
        /// <summary> 读取 catch </summary>
        void ReadCatch() {
            Token token = ReadToken();
            if (token.Type != TokenType.Catch)
                throw new ParserException(this, "Catch 'catch' expected.", token);
        }
    }
}
