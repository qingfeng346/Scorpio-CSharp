using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Compiler
{
    /// <summary> 脚本语法解析 </summary>
    public partial class ScriptLexer
    {
        private LexState m_lexState;                //当前解析状态
        private String m_strToken = null;           //字符串token
        private int m_iSourceLine;                  //当前解析行数
        private int m_iSourceChar;                  //当前解析字符
        private String m_strBreviary;               //字符串的摘要 取第一行字符串的前20个字符
        private List<String> m_listSourceLines;     //所有行
        private List<Token> m_listTokens;           //解析后所得Token
        private char ch;                            //当前的解析的字符
        public ScriptLexer(String buffer)
        {
            m_listSourceLines = new List<string>();
            m_listTokens = new List<Token>();
            String strSource = buffer.Replace("\r\n", "\r");
            string[] strLines = strSource.Split('\r');
            m_strBreviary = strLines.Length > 0 ? strLines[0] : "";
            if (m_strBreviary.Length > BREVIARY_CHAR) m_strBreviary = m_strBreviary.Substring(0, BREVIARY_CHAR);
            foreach (String strLine in strLines)
                m_listSourceLines.Add(strLine + "\r\n");
            m_iSourceLine = 0;
            m_iSourceChar = 0;
            lexState = LexState.None;
        }
        /// <summary> 获得整段字符串的摘要 </summary>
        public String GetBreviary()
        {
            return m_strBreviary;
        }
        /// <summary> 解析字符串 </summary>
        public List<Token> GetTokens()
        {
            m_iSourceLine = 0;
            m_iSourceChar = 0;
            lexState = LexState.None;
            m_listTokens.Clear();
            while (!EndOfSource)
            {
                if (EndOfLine)
                {
                    IgnoreLine();
                    continue;
                }
                ch = ReadChar();
                switch (lexState)
                {
                    case LexState.None:
                        switch (ch)
                        {
                            case ' ':
                            case '\t':
                            case '\n':
                            case '\r':
                                break;
                            case '(':
                                AddToken(TokenType.LeftPar);
                                break;
                            case ')':
                                AddToken(TokenType.RightPar);
                                break;
                            case '[':
                                AddToken(TokenType.LeftBracket);
                                break;
                            case ']':
                                AddToken(TokenType.RightBracket);
                                break;
                            case '{':
                                AddToken(TokenType.LeftBrace);
                                break;
                            case '}':
                                AddToken(TokenType.RightBrace);
                                break;
                            case ',':
                                AddToken(TokenType.Comma);
                                break;
                            case ':':
                                AddToken(TokenType.Colon);
                                break;
                            case ';':
                                AddToken(TokenType.SemiColon);
                                break;
                            case '?':
                                AddToken(TokenType.QuestionMark);
                                break;
                            case '.':
                                lexState = LexState.PeriodOrParams;
                                break;
                            case '+':
                                lexState = LexState.PlusOrIncrementOrAssignPlus;
                                break;
                            case '-':
                                lexState = LexState.MinusOrDecrementOrAssignMinus;
                                break;
                            case '*':
                                lexState = LexState.MultiplyOrAssignMultiply;
                                break;
                            case '/':
                                lexState = LexState.CommentOrDivideOrAssignDivide;
                                break;
                            case '%':
                                lexState = LexState.ModuloOrAssignModulo;
                                break;
                            case '=':
                                lexState = LexState.AssignOrEqual;
                                break;
                            case '&':
                                lexState = LexState.AndOrCombine;
                                break;
                            case '|':
                                lexState = LexState.OrOrInclusiveOr;
                                break;
                            case '!':
                                lexState = LexState.NotOrNotEqual;
                                break;
                            case '>':
                                lexState = LexState.GreaterOrGreaterEqual;
                                break;
                            case '<':
                                lexState = LexState.LessOrLessEqual;
                                break;
                            case '^':
                                lexState = LexState.XorOrAssignXor;
                                break;
                            case '@':
                                lexState = LexState.SimpleStringStart;
                                break;
                            case '\"':
                                lexState = LexState.String;
                                break;
                            case '\'':
                                lexState = LexState.SingleString;
                                break;
                            default:
                                if (ch == '_' || char.IsLetter(ch)) {
                                    lexState = LexState.Identifier;
                                    m_strToken = "" + ch;
                                } else if (ch == '0') {
                                    lexState = LexState.NumberOrHexNumber;
                                    m_strToken = "";
                                } else if (char.IsDigit(ch)) {
                                    lexState = LexState.Number;
                                    m_strToken = "" + ch;
                                } else {
                                    ThrowInvalidCharacterException(ch);
                                }
                                break;
                        }
                        break;
                    case LexState.PeriodOrParams:
                        if (ch == '.') {
                            lexState = LexState.Params;
                        } else {
                            AddToken(TokenType.Period, ".");
                            UndoChar();
                        }
                        break;
                    case LexState.Params:
                        if (ch == '.') {
                            AddToken(TokenType.Params, "...");
                        } else {
                            ThrowInvalidCharacterException(ch);
                        }
                        break;
                    case LexState.PlusOrIncrementOrAssignPlus:
                        if (ch == '+') {
                            AddToken(TokenType.Increment, "++");
                        } else if (ch == '=') {
                            AddToken(TokenType.AssignPlus, "+=");
                        } else {
                            AddToken(TokenType.Plus, "+");
                            UndoChar();
                        }
                        break;
                    case LexState.MinusOrDecrementOrAssignMinus:
                        if (ch == '-') {
                            AddToken(TokenType.Decrement, "--");
                        } else if (ch == '=') {
                            AddToken(TokenType.AssignMinus, "-=");
                        } else {
                            AddToken(TokenType.Minus, "-");
                            UndoChar();
                        }
                        break;
                    case LexState.MultiplyOrAssignMultiply:
                        if (ch == '=') {
                            AddToken(TokenType.AssignMultiply, "*=");
                        } else {
                            AddToken(TokenType.Multiply, "*");
                            UndoChar();
                        }
                        break;
                    case LexState.CommentOrDivideOrAssignDivide:
                        switch (ch) {
                            case '/':
                                lexState = LexState.LineComment;
                                break;
                            case '*':
                                lexState = LexState.BlockCommentStart;
                                break;
                            case '=':
                                AddToken(TokenType.AssignDivide, "/=");
                                break;
                            default:
                                AddToken(TokenType.Divide, "/");
                                UndoChar();
                                break;
                        }
                        break;
                    case LexState.ModuloOrAssignModulo:
                        if (ch == '=') {
                            AddToken(TokenType.AssignModulo, "%=");
                        } else {
                            AddToken(TokenType.AssignModulo, "%");
                            UndoChar();
                        }
                        break;
                    case LexState.LineComment:
                        if (ch == '\n')
                            lexState = LexState.None;
                        break;
                    case LexState.BlockCommentStart:
                        if (ch == '*')
                            lexState = LexState.BlockCommentEnd;
                        break;
                    case LexState.BlockCommentEnd:
                        if (ch == '/')
                            lexState = LexState.None;
                        else
                            lexState = LexState.BlockCommentStart;
                        break;
                    case LexState.AssignOrEqual:
                        if (ch == '=') {
                            AddToken(TokenType.Equal, "==");
                        } else {
                            AddToken(TokenType.Assign, "=");
                            UndoChar();
                        }
                        break;
                    case LexState.AndOrCombine:
                        if (ch == '&') {
                            AddToken(TokenType.And, "&&");
                        } else if (ch == '=') {
                            AddToken(TokenType.AssignCombine, "&=");
                        } else {
                            AddToken(TokenType.Combine, "&");
                            UndoChar();
                        }
                        break;
                    case LexState.OrOrInclusiveOr:
                        if (ch == '|') {
                            AddToken(TokenType.Or, "||");
                        } else if (ch == '=') {
                            AddToken(TokenType.AssignInclusiveOr, "|=");
                        } else {
                            AddToken(TokenType.InclusiveOr, "|");
                            UndoChar();
                        }
                        break;
                    case LexState.XorOrAssignXor:
                        if (ch == '=') {
                            AddToken(TokenType.AssignXOR, "^=");
                        } else {
                            AddToken(TokenType.XOR, "^");
                            UndoChar();
                        }
                        break;
                    case LexState.GreaterOrGreaterEqual:
                        if (ch == '=') {
                            AddToken(TokenType.GreaterOrEqual, ">=");
                        } else if (ch == '>') {
                            lexState = LexState.ShrOrAssignShr;
                        } else {
                            AddToken(TokenType.Greater, ">");
                            UndoChar();
                        }
                        break;
                    case LexState.LessOrLessEqual:
                        if (ch == '=') {
                            AddToken(TokenType.LessOrEqual, "<=");
                        } else if (ch == '<') {
                            lexState = LexState.ShiOrAssignShi;
                        } else {
                            AddToken(TokenType.Less, "<");
                            UndoChar();
                        }
                        break;
                    case LexState.ShrOrAssignShr:
                        if (ch == '=') {
                            AddToken(TokenType.AssignShr, ">>=");
                        } else {
                            AddToken(TokenType.Shr, ">>");
                            UndoChar();
                        }
                        break;
                    case LexState.ShiOrAssignShi:
                        if (ch == '=') {
                            AddToken(TokenType.AssignShi, "<<=");
                        } else {
                            AddToken(TokenType.Shi, "<<");
                            UndoChar();
                        }
                        break;
                    case LexState.NotOrNotEqual:
                        if (ch == '=') {
                            AddToken(TokenType.NotEqual, "!=");
                        } else {
                            AddToken(TokenType.Not, "!");
                            UndoChar();
                        }
                        break;
                    case LexState.String:
                        if (ch == '\"') {
                            AddToken(TokenType.String, m_strToken);
                        } else if (ch == '\\') {
                            lexState = LexState.StringEscape;
                        } else if (ch == '\r' || ch == '\n') {
                            ThrowInvalidCharacterException(ch);
                        } else {
                            m_strToken += ch;
                        }
                        break;
                    case LexState.StringEscape:
                        if (ch == '\\' || ch == '\"') {
                            m_strToken += ch;
                            lexState = LexState.String;
                        } else if (ch == 't') {
                            m_strToken += '\t';
                            lexState = LexState.String;
                        } else if (ch == 'r') {
                            m_strToken += '\r';
                            lexState = LexState.String;
                        } else if (ch == 'n') {
                            m_strToken += '\n';
                            lexState = LexState.String;
                        } else {
                            ThrowInvalidCharacterException(ch);
                        }
                        break;
                    case LexState.SingleString:
                        if (ch == '\'') {
                            AddToken(TokenType.String, m_strToken);
                        } else if (ch == '\\') {
                            lexState = LexState.SingleStringEscape;
                        } else if (ch == '\r' || ch == '\n') {
                            ThrowInvalidCharacterException(ch);
                        } else {
                            m_strToken += ch;
                        }
                        break;
                    case LexState.SingleStringEscape:
                        if (ch == '\\' || ch == '\'') {
                            m_strToken += ch;
                            lexState = LexState.SingleString;
                        } else if (ch == 't') {
                            m_strToken += '\t';
                            lexState = LexState.SingleString;
                        } else if (ch == 'r') {
                            m_strToken += '\r';
                            lexState = LexState.SingleString;
                        } else if (ch == 'n') {
                            m_strToken += '\n';
                            lexState = LexState.SingleString;
                        } else {
                            ThrowInvalidCharacterException(ch);
                        }
                        break;
                    case LexState.SimpleStringStart:
                        if (ch == '\"') {
                            lexState = LexState.SimpleString;
                        } else if (ch == '\'') {
                            lexState = LexState.SingleSimpleString;
                        } else {
                            ThrowInvalidCharacterException(ch);
                        }
                        break;
                    case LexState.SimpleString:
                        if (ch == '\"') {
                            lexState = LexState.SimpleStringQuotationMarkOrOver;
                        } else {
                            m_strToken += ch;
                        }
                        break;
                    case LexState.SimpleStringQuotationMarkOrOver:
                        if (ch == '\"') {
                            m_strToken += '\"';
                            lexState = LexState.SimpleString;
                        } else {
                            AddToken(TokenType.String, m_strToken);
                            UndoChar();
                        }
                        break;
                    case LexState.SingleSimpleString:
                        if (ch == '\'') {
                            lexState = LexState.SingleSimpleStringQuotationMarkOrOver;
                        } else {
                            m_strToken += ch;
                        }
                        break;
                    case LexState.SingleSimpleStringQuotationMarkOrOver:
                        if (ch == '\'') {
                            m_strToken += '\'';
                            lexState = LexState.SingleSimpleString;
                        } else {
                            AddToken(TokenType.String, m_strToken);
                            UndoChar();
                        }
                        break;
                    case LexState.NumberOrHexNumber:
                        if (ch == 'x') {
                            lexState = LexState.HexNumber;
                        } else {
                            m_strToken = "0";
                            lexState = LexState.Number;
                            UndoChar();
                        }
                        break;
                    case LexState.Number:
                        if (char.IsDigit(ch) || ch == '.') {
                            m_strToken += ch;
                        } else if (ch == 'L') {
                            long value = long.Parse(m_strToken);
                            AddToken(TokenType.Number, value);
                        } else {
                            double value = double.Parse(m_strToken);
                            AddToken(TokenType.Number, value);
                            UndoChar();
                        }
                        break;
                    case LexState.HexNumber:
                        if (IsHexDigit(ch)) {
                            m_strToken += ch;
                        } else {
                            if (Util.IsNullOrEmpty(m_strToken))
                                ThrowInvalidCharacterException(ch);
                            long value = long.Parse(m_strToken, System.Globalization.NumberStyles.HexNumber);
                            AddToken(TokenType.Number, value);
                            UndoChar();
                        }
                        break;
                    case LexState.Identifier:
                        if (ch == '_' || char.IsLetterOrDigit(ch)) {
                            m_strToken += ch;
                        } else {
                            TokenType tokenType;
                            switch (m_strToken)
                            {
                                case "eval":
                                    tokenType = TokenType.Eval;
                                    break;
                                case "var":
                                case "local":
                                    tokenType = TokenType.Var;
                                    break;
                                case "function":
                                    tokenType = TokenType.Function;
                                    break;
                                case "if":
                                    tokenType = TokenType.If;
                                    break;
                                case "elseif":
                                case "elif":
                                    tokenType = TokenType.ElseIf;
                                    break;
                                case "else":
                                    tokenType = TokenType.Else;
                                    break;
                                case "while":
                                    tokenType = TokenType.While;
                                    break;
                                case "for":
                                    tokenType = TokenType.For;
                                    break;
                                case "foreach":
                                    tokenType = TokenType.Foreach;
                                    break;
                                case "in":
                                    tokenType = TokenType.In;
                                    break;
                                case "switch":
                                    tokenType = TokenType.Switch;
                                    break;
                                case "case":
                                    tokenType = TokenType.Case;
                                    break;
                                case "default":
                                    tokenType = TokenType.Default;
                                    break;
                                case "try":
                                    tokenType = TokenType.Try;
                                    break;
                                case "catch":
                                    tokenType = TokenType.Catch;
                                    break;
                                case "throw":
                                    tokenType = TokenType.Throw;
                                    break;
                                case "continue":
                                    tokenType = TokenType.Continue;
                                    break;
                                case "break":
                                    tokenType = TokenType.Break;
                                    break;
                                case "return":
                                    tokenType = TokenType.Return;
                                    break;
                                case "null":
                                case "nil":
                                    tokenType = TokenType.Null;
                                    break;
                                case "true":
                                case "false":
                                    tokenType = TokenType.Boolean;
                                    break;
                                default:
                                    tokenType = TokenType.Identifier;
                                    break;
                            }
                            if (tokenType == TokenType.Boolean) {
                                m_listTokens.Add(new Token(tokenType, m_strToken == "true", m_iSourceLine, m_iSourceChar));
                            } else if (tokenType == TokenType.Null) {
                                m_listTokens.Add(new Token(tokenType, null, m_iSourceLine, m_iSourceChar));
                            } else {
                                m_listTokens.Add(new Token(tokenType, m_strToken, m_iSourceLine, m_iSourceChar));
                            }
                            UndoChar();
                            lexState = LexState.None;
                        }
                        break;
                }
            }
            m_listTokens.Add(new Token(TokenType.Finished, "", m_iSourceLine, m_iSourceChar));
            return m_listTokens;
        }
    }
}
