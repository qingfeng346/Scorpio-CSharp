using System;
namespace Scorpio.Compile.Compiler {
    //脚本的表征类型
    public enum TokenType {
        /// <summary>
        /// 空类型（没有实际用途）
        /// </summary>
        None = 0,
        /// <summary>
        /// var
        /// </summary>
        Var,
        /// <summary>
        /// {
        /// </summary>
        LeftBrace,
        /// <summary>
        /// }
        /// </summary>
        RightBrace,
        /// <summary>
        /// (
        /// </summary>
        LeftPar,
        /// <summary>
        /// )
        /// </summary>
        RightPar,
        /// <summary>
        /// [
        /// </summary>
        LeftBracket,
        /// <summary>
        /// ]
        /// </summary>
        RightBracket,
        /// <summary>
        /// .
        /// </summary>
        Period,
        /// <summary>
        /// ,
        /// </summary>
        Comma,
        /// <summary>
        /// :
        /// </summary>
        Colon,
        /// <summary>
        /// ;
        /// </summary>
        SemiColon,
        /// <summary>
        /// ?
        /// </summary>
        QuestionMark,
        /// <summary>
        /// ??
        /// </summary>
        EmptyRet,
        /// <summary>
        /// ?.
        /// </summary>
        QuestionMarkDot,
        /// <summary>
        /// #
        /// </summary>
        Sharp,
        /// <summary>
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// +=
        /// </summary>
        PlusAssign,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// -=
        /// </summary>
        MinusAssign,
        /// <summary>
        /// *
        /// </summary>
        Multiply,
        /// <summary>
        /// *=
        /// </summary>
        MultiplyAssign,
        /// <summary>
        /// /
        /// </summary>
        Divide,
        /// <summary>
        /// /=
        /// </summary>
        DivideAssign,
        /// <summary>
        /// % 模运算
        /// </summary>
        Modulo,
        /// <summary>
        /// %=
        /// </summary>
        ModuloAssign,
        /// <summary>
        /// | 或运算
        /// </summary>
        InclusiveOr,
        /// <summary>
        /// |=
        /// </summary>
        InclusiveOrAssign,
        /// <summary>
        /// ||
        /// </summary>
        Or,
        /// <summary>
        /// & 并运算
        /// </summary>
        Combine,
        /// <summary>
        /// &=
        /// </summary>
        CombineAssign,
        /// <summary>
        /// &&
        /// </summary>
        And,
        /// <summary>
        /// ^ 异或
        /// </summary>
        XOR,
        /// <summary>
        /// ^=
        /// </summary>
        XORAssign,
        /// <summary> 
        /// ~ 取反操作
        /// </summary>
        Negative,
        /// <summary>
        /// <<左移
        /// </summary>
        Shi,
        /// <summary>
        /// <<=
        /// </summary>
        ShiAssign,
        /// <summary>
        /// >> 右移
        /// </summary>
        Shr,
        /// <summary>
        /// >>=
        /// </summary>
        ShrAssign,
        /// <summary>
        /// !
        /// </summary>
        Not,
        /// <summary>
        /// =
        /// </summary>
        Assign,
        /// <summary>
        /// ==
        /// </summary>
        Equal,
        /// <summary>
        /// !=
        /// </summary>
        NotEqual,
        /// <summary>
        /// >
        /// </summary>
        Greater,
        /// <summary>
        /// >=
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        ///  <
        /// </summary>
        Less,
        /// <summary>
        /// <=
        /// </summary>
        LessOrEqual,
        /// <summary>
        /// ...
        /// </summary>
        Params,
        /// <summary>
        /// =>
        /// </summary>
        Lambda,
        /// <summary>
        /// if
        /// </summary>
        If,
        /// <summary>
        /// else
        /// </summary>
        Else,
        /// <summary>
        /// elif
        /// </summary>
        ElseIf,
        /// <summary>
        /// ifndef
        /// </summary>
        Ifndef,
        /// <summary>
        /// endif
        /// </summary>
        Endif,
        /// <summary>
        /// for
        /// </summary>
        For,
        /// <summary>
        /// foreach
        /// </summary>
        Foreach,
        /// <summary>
        /// in
        /// </summary>
        In,
        /// <summary>
        /// switch
        /// </summary>
        Switch,
        /// <summary>
        /// case
        /// </summary>
        Case,
        /// <summary>
        /// default
        /// </summary>
        Default,
        /// <summary>
        /// break
        /// </summary>
        Break,
        /// <summary>
        /// continue
        /// </summary>
        Continue,
        /// <summary>
        /// return
        /// </summary>
        Return,
        /// <summary>
        /// while
        /// </summary>
        While,
        /// <summary>
        /// function
        /// </summary>
        Function,
        /// <summary>
        /// try
        /// </summary>
        Try,
        /// <summary>
        /// catch
        /// </summary>
        Catch,
        /// <summary>
        /// throw
        /// </summary>
        Throw,
        /// <summary>
        /// finally
        /// </summary>
        Finally,
        /// <summary>
        /// define
        /// </summary>
        Define,
        /// <summary>
        /// bool true false
        /// </summary>
        Boolean,
        /// <summary>
        /// int float
        /// </summary>
        Number,
        /// <summary>
        /// string
        /// </summary>
        String,
        /// <summary>
        /// @"" @'' string
        /// </summary>
        //SimpleString,
        /// <summary>
        /// null
        /// </summary>
        Null,
        /// <summary>
        /// eval
        /// </summary>
        Eval,
        /// <summary>
        /// class
        /// </summary>
        Class,
        /// <summary>
        /// const
        /// </summary>
        Const,
        /// <summary>
        /// 说明符
        /// </summary>
        Identifier,
        /// <summary>
        /// 结束
        /// </summary>
        Finished,
    }

    //脚本的表征
    public class Token {
        public TokenType Type { get; private set; }         //标记类型
        public object Lexeme { get; private set; }          //标记值
        public int SourceLine { get; private set; }         //所在行
        public int SourceChar { get; private set; }         //所在列
        public Token(TokenType tokenType, object lexeme, int sourceLine, int sourceChar) {
            this.Type = tokenType;
            this.Lexeme = lexeme;
            this.SourceLine = sourceLine + 1;
            this.SourceChar = sourceChar;
        }
        public override String ToString() {
            return $"{Type}({Lexeme})";
        }
    }
}
