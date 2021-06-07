using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compile.Exception;
namespace Scorpio.Compile.Compiler {
    /// <summary> 脚本语法解析 </summary>
    public partial class ScriptLexer {
        enum FormatString {
            None,                   //无
            SingleQuotes,           //单引号字符串
            DoubleQuotes,           //双引号字符串
            Point,                  //`符号字符串
    
            SimpleSingleQuotes,     //带 @ 单引号字符串
            SimpleDoubleQuotes,     //带 @ 双引号字符串
            SimplePoint,            //带 @ `符号字符串
        }
        private const char END_CHAR = char.MaxValue;    //结尾字符
        private const int BREVIARY_CHAR = 10;           //摘要的字符数

        private char m_ch;                              //当前解析字符
        private char ch;                                //临时保存字符
        private FormatString m_FormatString;            //是否正在格式化字符串
        private StringBuilder m_Builder;                //当前缓存的字符串
        private List<Token> m_listTokens;               //返回的Token列表
        private string m_strBuffer;                     //解析内容
        private int m_iLength = 0;                      //Buffer长度
        private int m_iSourceLine = 0;                  //当前解析的行数
        private int m_iSourceChar = 0;                  //当前行解析
        private int m_iIndex = 0;                       //当前解析
        private int m_iCacheLine = 0;                   //Simple字符串起始行
        public ScriptLexer(string buffer, string breviary) {
            Breviary = breviary == null || breviary.Length == 0 ? buffer.Substring(0, Math.Min(BREVIARY_CHAR, buffer.Length)) : breviary;
            m_FormatString = FormatString.None;
            m_strBuffer = buffer;
            m_iLength = buffer.Length;
            m_Builder = new StringBuilder();
            m_listTokens = new List<Token>();
        }
        /// <summary> 整段字符串的摘要 取第一行字符串的前20个字符</summary>
        public string Breviary { get; private set; }
        public int SourceLine => m_iSourceLine;
        public int SourceChar => m_iSourceChar;
        void ThrowInvalidCharacterException() {
            ThrowInvalidCharacterException(m_ch);
        }
        void ThrowInvalidCharacterException(char ch) {
            throw new LexerException(this, $"Unexpected character [{ch}]  Line:{m_iSourceLine + 1} Column:{m_iSourceChar}");
        }
        void ThrowInvalidCharacterException(string message) {
            throw new LexerException(this, $"{message}  Line:{m_iSourceLine + 1} Column:{m_iSourceChar}");
        }
        char ReadChar() {
            ++m_iIndex;
            ++m_iSourceChar;
            if (m_iIndex < m_iLength) {
                return m_strBuffer[m_iIndex];
            } else if (m_iIndex == m_iLength) {
                return END_CHAR;
            }
            throw new LexerException(this, "End of source reached.");
        }
        char PeekChar() {
            int index = m_iIndex + 1;
            if (index < m_iLength) {
                return m_strBuffer[index];
            } else if (index == m_iLength) {
                return END_CHAR;
            }
            throw new LexerException(this, "End of source reached.");
        }
        void UndoChar() {
            if (m_iIndex == 0)
                throw new LexerException(this, "Cannot undo char beyond start of source.");
            --m_iIndex;
            --m_iSourceChar;
        }
        void AddLine() {
            m_iSourceChar = 0;
            ++m_iSourceLine;
        }
        void AddToken(TokenType type) {
            AddToken(type, m_ch);
        }
        void AddToken(TokenType type, object lexeme) {
            AddToken(type, lexeme, m_iSourceLine, m_iSourceChar);
        }
        void AddToken(TokenType type, object lexeme, int sourceLine, int sourceChar) {
            m_listTokens.Add(new Token(type, lexeme, sourceLine, sourceChar));
            m_Builder.Length = 0;
        }
        bool IsHexDigit(char c) {
            if (char.IsDigit(c))
                return true;
            if ('a' <= c && c <= 'f')
                return true;
            if ('A' <= c && c <= 'F')
                return true;
            return false;
        }
        private bool IsIdentifier(char ch) {
            return (ch == '_' || ch == '$' || char.IsLetterOrDigit(ch));
        }
    }
}
