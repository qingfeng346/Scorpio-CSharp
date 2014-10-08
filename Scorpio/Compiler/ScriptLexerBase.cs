using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.Compiler
{
    public partial class ScriptLexer
    {
        enum LexState
        {
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
            /// <summary> & 并且 </summary>
            And,
            /// <summary> | 或者 </summary>
            Or,
            /// <summary> ! 非或者不等于 </summary>
            NotOrNotEqual,
            /// <summary> > 大于或者大于等于 </summary>
            GreaterOrGreaterEqual,
            /// <summary> < 小于或者小于等于 </summary>
            LessOrLessEqual,
            /// <summary> " 字符串 </summary>
            String,
            /// <summary> \ 格式符 </summary>
            StringEscape,
            /// <summary> @ 开始字符串 </summary>
            SimpleStringStart,
            /// <summary> @" 不格式化的字符串 类似c# @符号 </summary>
            SimpleString,
            /// <summary> 字符串内出现"是引号还是结束符 </summary>
            SimpleStringQuotationMarkOrOver,
            /// <summary> 数字 </summary>
            Number,
            /// <summary> 描述符 </summary>
            Identifier,
        }
        private const int BREVIARY_CHAR = 20;       //摘要的字符数
        private LexState lexState { get { return m_lexState; } set { m_lexState = value; if (m_lexState == LexState.None) { m_strToken = ""; } } }
        bool EndOfSource { get { return m_iSourceLine >= m_listSourceLines.Count; } }
        bool EndOfLine { get { return m_iSourceChar >= m_listSourceLines[m_iSourceLine].Length; } }
        char ReadChar()
        {
            if (EndOfSource)
                throw new LexerException("End of source reached.", m_iSourceLine);
            char ch = m_listSourceLines[m_iSourceLine][m_iSourceChar++];
            if (m_iSourceChar >= m_listSourceLines[m_iSourceLine].Length) {
                m_iSourceChar = 0;
                ++m_iSourceLine;
            }
            return ch;
        }
        void UndoChar()
        {
            if (m_iSourceLine == 0 && m_iSourceChar == 0)
                throw new LexerException("Cannot undo char beyond start of source.", m_iSourceLine);
            --m_iSourceChar;
            if (m_iSourceChar < 0) {
                --m_iSourceLine;
                m_iSourceChar = m_listSourceLines[m_iSourceLine].Length - 1;
            }
        }
        void IgnoreLine()
        {
            ++m_iSourceLine;
            m_iSourceChar = 0;
        }
        void ThrowInvalidCharacterException(char ch)
        {
            throw new ScriptException("Unexpected character [" + ch + "]  Line:" + (m_iSourceLine + 1) + " Column:" + m_iSourceChar + " [" + m_listSourceLines[m_iSourceLine] + "]");
        }
        void AddToken(TokenType type)
        {
            AddToken(type, ch);
        }
        void AddToken(TokenType type, object lexeme)
        {
            m_listTokens.Add(new Token(type, lexeme, m_iSourceLine, m_iSourceChar));
            lexState = LexState.None;
        }
    }
}
