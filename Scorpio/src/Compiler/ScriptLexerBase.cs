using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Compiler {
    public partial class ScriptLexer {
        private const char END_CHAR = (char)0;      //结尾字符
        private const int BREVIARY_CHAR = 10;       //摘要的字符数
        private char m_ch;                          //当前解析字符
        private char ch;                            //临时保存字符
        private int m_FormatString;                 //是否正在格式化字符串 0 没有 1普通字符串单引号 2普通字符串双引号 3单纯字符串单引号 4单传字符串双引号
        private StringBuilder m_Builder;            //当前缓存的字符串
        private List<Token> m_listTokens;           //返回的Token列表
        private String m_strBreviary;               //字符串的摘要 取第一行字符串的前20个字符
        private String m_strBuffer;                 //解析内容
        private int m_iLength = 0;                  //Buffer长度
        private int m_iSourceLine = 0;              //当前解析的行数
        private int m_iSourceChar = 0;              //当前行解析
        private int m_iIndex = 0;                   //当前解析
        private int m_iCacheLine = 0;               //Simple字符串起始行
        public ScriptLexer(String buffer, String strBreviary) {
            if (Util.IsNullOrEmpty(strBreviary)) {
                m_strBreviary = buffer.Length >= BREVIARY_CHAR ? buffer : buffer.Substring(0, BREVIARY_CHAR);
            } else {
                m_strBreviary = strBreviary;
            }
            m_strBuffer = buffer;
            m_iLength = buffer.Length;
            m_Builder = new StringBuilder();
            m_listTokens = new List<Token>();
        }

        char ReadChar() {
            ++m_iIndex;
            ++m_iSourceChar;
            if (m_iIndex < m_iLength) {
                return m_strBuffer[m_iIndex];
            } else if (m_iIndex == m_iLength) {
                return END_CHAR;
            }
            throw new LexerException("End of source reached.");
        }
        char PeekChar() {
            int index = m_iIndex + 1;
            if (index < m_iLength) {
                return m_strBuffer[index];
            } else if (index == m_iLength) {
                return END_CHAR;
            }
            throw new LexerException("End of source reached.");
        }
        /// <summary> 获得整段字符串的摘要 </summary>
        public String GetBreviary() {
            return m_strBreviary;
        }
        void UndoChar() {
            if (m_iIndex == 0)
                throw new LexerException("Cannot undo char beyond start of source.");
            --m_iIndex;
            --m_iSourceChar;
        }
        void ThrowInvalidCharacterException() {
            ThrowInvalidCharacterException(m_ch);
        }
        void ThrowInvalidCharacterException(char ch) {
            throw new LexerException(m_strBreviary + ":" + (m_iSourceLine + 1) + "  Unexpected character [" + ch + "]  Line:" + (m_iSourceLine + 1) + " Column:" + m_iSourceChar);
        }
        void AddLine() {
            m_iSourceChar = 0;
            ++m_iSourceLine;
        }
        void AddToken(TokenType type) {
            AddToken(type, m_ch);
        }
        void AddToken(TokenType type, object lexeme) {
            AddToken(new Token(type, lexeme, m_iSourceLine, m_iSourceChar));
        }
        void AddToken(Token token) {
            m_listTokens.Add(token);
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
            return (ch == '_' || char.IsLetterOrDigit(ch));
        }
    }
}
