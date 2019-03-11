using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Compiler {

    /// <summary> 脚本语法解析 </summary>
    public partial class ScriptLexer {
        /// <summary> . </summary>
        void ReadDot() {
            ch = ReadChar();
            if (ch == '.') {
                if (ReadChar() == '.') {
                    AddToken(TokenType.Params, "...");
                } else {
                    ThrowInvalidCharacterException();
                }
            } else {
                AddToken(TokenType.Period, ".");
                UndoChar();
            }
        }
        /// <summary> + </summary>
        void ReadPlus() {
            ch = ReadChar();
            if (ch == '+') {
                AddToken(TokenType.Increment, "++");
            } else if (ch == '=') {
                AddToken(TokenType.AssignPlus, "+=");
            } else {
                AddToken(TokenType.Plus, "+");
                UndoChar();
            }
        }
        /// <summary> - </summary>
        void ReadMinus() {
            ch = ReadChar();
            if (ch == '-') {
                AddToken(TokenType.Decrement, "--");
            } else if (ch == '=') {
                AddToken(TokenType.AssignMinus, "-=");
            } else {
                AddToken(TokenType.Minus, "-");
                UndoChar();
            }
        }
        /// <summary> * </summary>
        void ReadMultiply() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignMultiply, "*=");
            } else {
                AddToken(TokenType.Multiply, "*");
                UndoChar();
            }
        }
        /// <summary> / </summary>
        void ReadDivideOrComment() {
            ch = ReadChar();
            if (ch == '/') {
                ReadLineComment();
            } else if (ch == '*') {
                ReadBlockComment();
            } else if (ch == '=') {
                AddToken(TokenType.AssignDivide, "/=");
            } else {
                AddToken(TokenType.Divide, "+");
                UndoChar();
            }
        }
        void ReadLineComment() {
            do {
                ch = ReadChar();
                if (ch == '\n' || ch == END_CHAR) {
                    UndoChar();
                    break;
                }
            } while (true);
        }
        void ReadBlockComment() {
            do {
                ch = ReadChar();
                if (ch == '\n') {
                    AddLine();
                } else if (ch == '*' && PeekChar() == '/') {
                    ReadChar();
                    break;
                }
            } while (true);
        }
        /// <summary> % </summary>
        void ReadModulo() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignModulo, "%=");
            } else {
                AddToken(TokenType.Modulo, "%");
                UndoChar();
            }
        }
        /// <summary> = </summary>
        void ReadAssign() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.Equal, "==");
            } else {
                AddToken(TokenType.Assign, "=");
                UndoChar();
            }
        }
        /// <summary> & </summary>
        void ReadAnd() {
            ch = ReadChar();
            if (ch == '&') {
                AddToken(TokenType.And, "&&");
            } else if (ch == '=') {
                AddToken(TokenType.AssignCombine, "&=");
            } else {
                AddToken(TokenType.Combine, "&");
                UndoChar();
            }
        }
        /// <summary> | </summary>
        void ReadOr() {
            ch = ReadChar();
            if (ch == '|') {
                AddToken(TokenType.Or, "||");
            } else if (ch == '=') {
                AddToken(TokenType.AssignInclusiveOr, "|=");
            } else {
                AddToken(TokenType.InclusiveOr, "|");
                UndoChar();
            }
        }
        /// <summary> ! </summary>
        void ReadNot() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.NotEqual, "!=");
            } else {
                AddToken(TokenType.Not, "!");
                UndoChar();
            }
        }
        /// <summary> > </summary>
        void ReadGreater() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.GreaterOrEqual, ">=");
            } else if (ch == '>') {
                ch = ReadChar();
                if (ch == '=') {
                    AddToken(TokenType.AssignShr, ">>=");
                } else {
                    AddToken(TokenType.Shr, ">>");
                    UndoChar();
                }
            } else {
                AddToken(TokenType.Greater, ">");
                UndoChar();
            }
        }
        /// <summary> < </summary>
        void ReadLess() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.LessOrEqual, "<=");
            } else if (ch == '<') {
                ch = ReadChar();
                if (ch == '=') {
                    AddToken(TokenType.AssignShi, "<<=");
                } else {
                    AddToken(TokenType.Shi, "<<");
                    UndoChar();
                }
            } else {
                AddToken(TokenType.Less, "<");
                UndoChar();
            }
        }
        /// <summary> ^ </summary>
        void ReadXor() {
            ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignXOR, "^=");
            } else {
                AddToken(TokenType.XOR, "^");
                UndoChar();
            }
        }
        /// <summary> 读取数字 </summary>
        void ReadNumber() {
            m_Builder.Append(m_ch);
            do {
                ch = ReadChar();
                if (char.IsDigit(ch) || ch == '.') {
                    m_Builder.Append(ch);
                } else if (ch == 'L') {
                    AddToken(TokenType.Number, long.Parse(m_Builder.ToString()));
                    break;
                } else {
                    AddToken(TokenType.Number, double.Parse(m_Builder.ToString()));
                    UndoChar();
                    break;
                }
            } while (true);
        }
        /// <summary> 读取16进制数字 </summary>
        void ReadNumberOrHexNumber() {
            if (ReadChar() == 'x') {
                do {
                    ch = ReadChar();
                    if (IsHexDigit(ch)) {
                        m_Builder.Append(ch);
                    } else if (m_Builder.Length == 0) {
                        ThrowInvalidCharacterException(ch);
                    } else {
                        AddToken(TokenType.Number, long.Parse(m_Builder.ToString(), System.Globalization.NumberStyles.HexNumber));
                        UndoChar();
                        break;
                    }
                } while (true);
            } else {
                UndoChar();
                ReadNumber();
            }
        }
        /// <summary> 读取 }  </summary>
        void ReadRightBrace() {
            if (m_FormatString == 0) {
                AddToken(TokenType.RightBrace, '}');
            } else {
                AddToken(TokenType.RightPar, ')');
                AddToken(TokenType.Plus, '+');
                if (m_FormatString == 1 || m_FormatString == 2) {
                    m_ch = m_FormatString == 1 ? '\'' : '\"';
                    m_FormatString = 0;
                    ReadString();
                } else {
                    m_ch = m_FormatString == 3 ? '\'' : '\"';
                    m_FormatString = 0;
                    ReadSimpleString(false);
                }
            }
        }
        void ReadSimpleString(bool symbol) {
            if (symbol) {
                m_ch = ReadChar();
                if (m_ch != '\'' && m_ch != '\"') {
                    ThrowInvalidCharacterException();
                }
            }
            m_iCacheLine = m_iSourceLine;
            do {
                ch = ReadChar();
                if (ch == m_ch) {
                    ch = ReadChar();
                    if (ch == m_ch) {
                        m_Builder.Append(ch);
                    } else {
                        UndoChar();
                        AddToken(new Token(TokenType.SimpleString, m_Builder.ToString(), m_iCacheLine, m_iSourceChar));
                        break;
                    }
                } else if (ch == '$') {
                    ch = ReadChar();
                    if (ch == '{') {
                        AddToken(TokenType.String, m_Builder.ToString());
                        AddToken(TokenType.Plus, '+');
                        AddToken(TokenType.LeftPar, '(');
                        m_FormatString = m_ch == '\'' ? 3 : 4;
                        break;
                    } else {
                        UndoChar();
                        m_Builder.Append('$');
                    }
                } else {
                    m_Builder.Append(ch);
                    if (ch == '\n') {
                        AddLine();
                    }
                }
            } while (true);
        }
        void ReadString() {
            do {
                ch = ReadChar();
                if (ch == '\\') {
                    ch = ReadChar();
                    switch (ch) {
                        case '\'': m_Builder.Append('\''); break;
                        case '\"': m_Builder.Append('\"'); break;
                        case '\\': m_Builder.Append('\\'); break;
                        case 'a': m_Builder.Append('\a'); break;
                        case 'b': m_Builder.Append('\b'); break;
                        case 'f': m_Builder.Append('\f'); break;
                        case 'n': m_Builder.Append('\n'); break;
                        case 'r': m_Builder.Append('\r'); break;
                        case 't': m_Builder.Append('\t'); break;
                        case 'v': m_Builder.Append('\v'); break;
                        case '0': m_Builder.Append('\0'); break;
                        case '/': m_Builder.Append("/"); break;
                        default: ThrowInvalidCharacterException(ch); break;
                    }
                } else if (ch == '\n') {
                    ThrowInvalidCharacterException(ch);
                } else if (ch == m_ch) {
                    AddToken(TokenType.String, m_Builder.ToString());
                    break;
                } else if (ch == '$') {
                    ch = ReadChar();
                    if (ch == '{') {
                        AddToken(TokenType.String, m_Builder.ToString());
                        AddToken(TokenType.Plus, '+');
                        AddToken(TokenType.LeftPar, '(');
                        m_FormatString = m_ch == '\'' ? 1 : 2;
                        break;
                    } else {
                        UndoChar();
                        m_Builder.Append('$');
                    }
                } else {
                    m_Builder.Append(ch);
                }
            }
            while (true);
        }
        void ReadIdentifier() {
            m_Builder.Append(m_ch);
            do {
                ch = ReadChar();
                if (IsIdentifier(ch)) {
                    m_Builder.Append(ch);
                } else {
                    UndoChar();
                    break;
                }
            } while (true);
            TokenType tokenType;
            switch (m_Builder.ToString()) {
            case "eval":
                tokenType = TokenType.Eval;
                break;
            case "var":
            case "local":
            case "let":
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
            case "finally":
                tokenType = TokenType.Finally;
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
            case "define":
                tokenType = TokenType.Define;
                break;
            case "ifndef":
                tokenType = TokenType.Ifndef;
                break;
            case "endif":
                tokenType = TokenType.Endif;
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
                AddToken(new Token(tokenType, m_Builder.ToString().Equals("true"), m_iSourceLine, m_iSourceChar));
            } else if (tokenType == TokenType.Null) {
                AddToken(new Token(tokenType, null, m_iSourceLine, m_iSourceChar));
            } else {
                AddToken(new Token(tokenType, m_Builder.ToString(), m_iSourceLine, m_iSourceChar));
            }
        }
        /// <summary> 解析字符串 </summary>
        public List<Token> GetTokens() {
            m_iSourceChar = 0;
            m_iSourceLine = 0;
            m_iIndex = 0;
            m_FormatString = 0;
            m_ch = END_CHAR;
            m_Builder.Length = 0;
            m_listTokens.Clear();
            for (; m_iIndex < m_iLength; ++m_iIndex, ++m_iSourceChar) {
                m_ch = m_strBuffer[m_iIndex];
                if (m_ch == '\n') {
                    AddLine();
                }
                switch (m_ch) {
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
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
                    case '#':
                        AddToken(TokenType.Sharp);
                        break;
                    case '~':
                        AddToken(TokenType.Negative);
                        break;
                    case '}':
                        ReadRightBrace();
                        break;
                    case '.':
                        ReadDot();
                        break;
                    case '+':
                        ReadPlus();
                        break;
                    case '-':
                        ReadMinus();
                        break;
                    case '*':
                        ReadMultiply();
                        break;
                    case '/':
                        ReadDivideOrComment();
                        break;
                    case '%':
                        ReadModulo();
                        break;
                    case '=':
                        ReadAssign();
                        break;
                    case '&':
                        ReadAnd();
                        break;
                    case '|':
                        ReadOr();
                        break;
                    case '!':
                        ReadNot();
                        break;
                    case '>':
                        ReadGreater();
                        break;
                    case '<':
                        ReadLess();
                        break;
                    case '^':
                        ReadXor();
                        break;
                    case '@':
                        ReadSimpleString(true);
                        break;
                    case '\"':
                    case '\'':
                        ReadString();
                        break;
                    case '0':
                        ReadNumberOrHexNumber();
                        break;
                    default:
                        if (char.IsDigit(m_ch)) {
                            ReadNumber();
                        } else if (IsIdentifier(m_ch)) {
                            ReadIdentifier();
                        } else {
                            ThrowInvalidCharacterException(m_ch);
                        }
                        break;
                }
            }
            AddToken(new Token(TokenType.Finished, "", m_iSourceLine, m_iSourceChar));
            return m_listTokens;
        }
    }
}
