using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Compiler {
    public partial class ScriptLexer {
        enum LexState {
            /// <summary> 没有关键字 </summary>
            None,
            /// <summary> = 等于或者相等 </summary>
            AssignOrEqual,
            /// <summary> / 注释或者除号 </summary>
            CommentOrDivideOrAssignDivide,
            /// <summary> 行注释 </summary>
            LineComment,
            /// <summary> 区域注释开始 </summary>
            BlockCommentStart,
            /// <summary> 区域注释结束 </summary>
            BlockCommentEnd,
            /// <summary> .或者多参符(...) </summary>
            PeriodOrParams,
            /// <summary> 多参符(...) </summary>
            Params,
            /// <summary> + 或者 ++ 或者 += </summary>
            PlusOrIncrementOrAssignPlus,
            /// <summary> - 或者 -= </summary>
            MinusOrDecrementOrAssignMinus,
            /// <summary> * 或者 *= </summary>
            MultiplyOrAssignMultiply,
            /// <summary> % 或者 %= </summary>
            ModuloOrAssignModulo,
            /// <summary> & 或者 &= 或者 && </summary>
            AndOrCombine,
            /// <summary> | 或者 |= 或者 || </summary>
            OrOrInclusiveOr,
            /// <summary> ^ 或者 ^= </summary>
            XorOrAssignXor,
            /// <summary> << 或者 <<= </summary>
            ShiOrAssignShi,
            /// <summary> >> 或者 >>= </summary>
            ShrOrAssignShr,
            /// <summary> ! 非或者不等于 </summary>
            NotOrNotEqual,
            /// <summary> > 大于或者大于等于 </summary>
            GreaterOrGreaterEqual,
            /// <summary> < 小于或者小于等于 </summary>
            LessOrLessEqual,
            /// <summary> 字符串 双引号 单引号 字符串都可以</summary>
            String,
            /// <summary> \ 格式符 </summary>
            StringEscape,
            /// <summary> @ 开始字符串 </summary>
            SimpleStringStart,
            /// <summary> @" 不格式化的字符串 类似c# @符号 </summary>
            SimpleString,
            /// <summary> 字符串内出现"是引号还是结束符 </summary>
            SimpleStringQuotationMarkOrOver,
            /// <summary> ${} 格式化字符串 </summary>
            StringFormat,
            /// <summary> 十进制数字或者十六进制数字 </summary>
            NumberOrHexNumber,
            /// <summary> 十进制数字 </summary>
            Number,
            /// <summary> 十六进制数字 </summary>
            HexNumber,
            /// <summary> 描述符 </summary>
            Identifier,
        }
        private const char END_CHAR = (char)0;      //结尾字符
        private const int BREVIARY_CHAR = 10;       //摘要的字符数
        private char m_ch;                          //当前解析字符
        private StringBuilder m_Builder;            //
        private LexState m_lexState;                //当前解析状态
        private LexState m_cacheLexState;           //缓存解析状态
        private List<Token> m_listTokens;           //
        private String m_strBreviary;               //字符串的摘要 取第一行字符串的前20个字符
        private String m_strBuffer;                 //解析内容
        private int m_iLength = 0;                  //Buffer长度
        private int m_iSourceLine = 0;              //当前解析的行数
        private int m_iSourceChar = 0;              //当前行解析
        private int m_iIndex = 0;                   //当前解析
        private int m_iCacheLine = 0;               //Simple字符串起始行
        private LexState lexState { get { return m_lexState; } set { m_lexState = value; if (m_lexState == LexState.None) { m_Builder.Clear(); } } }
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
        }
        void ThrowInvalidCharacterException(char ch) {
            throw new LexerException(m_strBreviary + ":" + (m_iSourceLine + 1) + "  Unexpected character [" + ch + "]  Line:" + (m_iSourceLine + 1) + " Column:" + m_iSourceChar);
        }
        void AddToken(TokenType type) {
            AddToken(type, m_ch);
        }
        void AddToken(TokenType type, object lexeme) {
            m_listTokens.Add(new Token(type, lexeme, m_iSourceLine, m_iSourceChar));
            lexState = LexState.None;
        }
        void AddToken(Token token) {
            m_listTokens.Add(token);
            lexState = LexState.None;
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
