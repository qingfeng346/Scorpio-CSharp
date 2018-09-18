using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Compiler {

    /// <summary> 脚本语法解析 </summary>
    public partial class ScriptLexer {
        void ReadPlus() {
            char ch = ReadChar();
            if (ch == '+') {
                AddToken(TokenType.Increment, "++");
            } else if (ch == '=') {
                AddToken(TokenType.AssignPlus, "+=");
            } else {
                AddToken(TokenType.Plus, "+");
                UndoChar();
            }
        }
        void ReadMinus() {
            char ch = ReadChar();
            if (ch == '-') {
                AddToken(TokenType.Decrement, "--");
            } else if (ch == '=') {
                AddToken(TokenType.AssignMinus, "-=");
            } else {
                AddToken(TokenType.Minus, "-");
                UndoChar();
            }
        }
        void ReadMultiply() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignMultiply, "*=");
            } else {
                AddToken(TokenType.Multiply, "*");
                UndoChar();
            }
        }
        void ReadDivideOrComment() {
            char ch = ReadChar();
            if (ch == '/') {
                lexState = LexState.LineComment;
            } else if (ch == '*') {
                lexState = LexState.BlockCommentStart;
            } else if (ch == '=') {
                AddToken(TokenType.AssignDivide, "/=");
            } else {
                AddToken(TokenType.Divide, "+");
                UndoChar();
            }
        }
        void ReadModulo() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignModulo, "%=");
            } else {
                AddToken(TokenType.Modulo, "%");
                UndoChar();
            }
        }
        void ReadAssign() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.Equal, "==");
            } else {
                AddToken(TokenType.Assign, "=");
                UndoChar();
            }
        }
        void ReadAnd() {
            char ch = ReadChar();
            if (ch == '&') {
                AddToken(TokenType.And, "&&");
            } else if (ch == '=') {
                AddToken(TokenType.AssignCombine, "&=");
            } else {
                AddToken(TokenType.Combine, "&");
                UndoChar();
            }
        }
        void ReadOr() {
            char ch = ReadChar();
            if (ch == '|') {
                AddToken(TokenType.Or, "||");
            } else if (ch == '=') {
                AddToken(TokenType.AssignInclusiveOr, "|=");
            } else {
                AddToken(TokenType.InclusiveOr, "|");
                UndoChar();
            }
        }
        void ReadNot() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.NotEqual, "!=");
            } else {
                AddToken(TokenType.Not, "!");
                UndoChar();
            }
        }
        void ReadGreater() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.GreaterOrEqual, ">=");
            } else if (ch == '>') {
                lexState = LexState.ShrOrAssignShr;
            } else {
                AddToken(TokenType.Greater, ">");
                UndoChar();
            }
        }
        void ReadLess() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.LessOrEqual, "<=");
            } else if (ch == '<') {
                lexState = LexState.ShiOrAssignShi;
            } else {
                AddToken(TokenType.Less, "<");
                UndoChar();
            }
        }
        void ReadXor() {
            char ch = ReadChar();
            if (ch == '=') {
                AddToken(TokenType.AssignXOR, "^=");
            } else {
                AddToken(TokenType.XOR, "^");
                UndoChar();
            }
        }
        void ReadSimpleString() {
            char ch = ReadChar();
            if (ch != '\"' && ch != '\'') {
                ThrowInvalidCharacterException(ch);
            }
            char c;
            m_iCacheLine = m_iSourceLine;
            while (true) {
                c = ReadChar();
                if (c == ch) {
                    if (PeekChar() == ch) {
                        m_Builder.Append(ch);
                    } else {
                        AddToken(new Token(TokenType.SimpleString, m_Builder.ToString(), m_iCacheLine, m_iSourceChar));
                        break;
                    }
                } else if (c == '$' && PeekChar() == '{') {
                    m_cacheLexState = lexState;
                } else {
                    m_Builder.Append(c);
                }
            }
        }
        void ReadString() {
            char ch;
            while (true) {
                ch = ReadChar();
                if (ch == '\\') {
                    if (PeekChar() == m_ch) {
                        ReadChar();
                        m_Builder.Append(m_ch);
                    } else {
                        m_Builder.Append(ch);
                        m_Builder.Append(ReadChar());
                    }
                } else if (ch == '\n') {
                    ThrowInvalidCharacterException(ch);
                } else if (ch == m_ch) {
                    AddToken(TokenType.String, m_Builder.ToString());
                    break;
                } else {
                    m_Builder.Append(ch);
                }
            }
        }
        void ReadNumberOrHexNumber() {
            if (ReadChar() == 'x') {
                char ch;
                while (true) {
                    ch = ReadChar();
                    if (IsHexDigit(ch)) {
                        m_Builder.Append(ch);
                    } else if (m_Builder.Length == 0){
                        ThrowInvalidCharacterException(ch);
                    } else {
                        long value = long.Parse(m_Builder.ToString(), System.Globalization.NumberStyles.HexNumber);
                        AddToken(TokenType.Number, value);
                        UndoChar();
                        break;
                    }
                }
            } else {
                UndoChar();
                ReadNumber();
            }
        }
        void ReadNumber() {
            m_Builder.Append(m_ch);
            char ch;
            while (true) {
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
            }
        }
        void ReadIdentifier() {
            m_Builder.Append(m_ch);
            char ch;
            while (true) {
                ch = ReadChar();
                if (IsIdentifier(ch)) {
                    m_Builder.Append(ch);
                } else {
                    UndoChar();
                    break;
                }
            }
            TokenType tokenType;
            switch (m_Builder.ToString()) {
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
                AddToken(new Token(tokenType, m_Builder.Equals("true"), m_iSourceLine, m_iSourceChar));
            } else if (tokenType == TokenType.Null) {
                AddToken(new Token(tokenType, null, m_iSourceLine, m_iSourceChar));
            } else {
                AddToken(new Token(tokenType, m_Builder.ToString(), m_iSourceLine, m_iSourceChar));
            }
        }
        void CheckBlockCommentEnd() {
            char ch = ReadChar();
            if (ch == '/') {
                lexState = LexState.None;
            }
        }
        /// <summary> 解析字符串 </summary>
        public List<Token> GetTokens() {
            m_iSourceChar = 0;
            m_iSourceLine = 0;
            m_iIndex = 0;
            lexState = LexState.None;
            m_ch = END_CHAR;
            m_Builder.Clear();
            m_listTokens.Clear();
            for (; m_iIndex < m_iLength; ++m_iIndex,++m_iSourceChar) {
                m_ch = m_strBuffer[m_iIndex];
                if (m_ch == '\n') {
                    m_iSourceChar = 0;
                    ++m_iSourceLine;
                }
                switch(m_lexState) {
                    case LexState.None:
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
                                ReadSimpleString();
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
                        break;
                    case LexState.LineComment:
                        if (m_ch == '\n') {
                            lexState = LexState.None;
                        }
                        break;
                    case LexState.BlockCommentStart:
                        if (m_ch == '*') {
                            CheckBlockCommentEnd();
                        }
                        break;
                }
                
            }
            AddToken(new Token(TokenType.Finished, "", m_iSourceLine, m_iSourceChar));
            return m_listTokens;
        }
    }
}
