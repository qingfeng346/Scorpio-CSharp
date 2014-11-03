using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Compiler
{
    //脚本的表征类型
    public enum TokenType
    {
        /// <summary>
        /// 空类型（没有实际用途）
        /// </summary>
        None = 0,
        /// <summary>
        /// global
        /// </summary>
        Global,
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
        /// +
        /// </summary>
        Plus,
        /// <summary>
        /// ++
        /// </summary>
        Increment,
        /// <summary>
        /// +=
        /// </summary>
        AssignPlus,
        /// <summary>
        /// -
        /// </summary>
        Minus,
        /// <summary>
        /// --
        /// </summary>
        Decrement,
        /// <summary>
        /// -=
        /// </summary>
        AssignMinus,
        /// <summary>
        /// *
        /// </summary>
        Multiply,
        /// <summary>
        /// *=
        /// </summary>
        AssignMultiply,
        /// <summary>
        /// /
        /// </summary>
        Divide,
        /// <summary>
        /// /=
        /// </summary>
        AssignDivide,
        /// <summary>
        /// %
        /// </summary>
        Modulo,
        /// <summary>
        /// %=
        /// </summary>
        AssignModulo,
        /// <summary>
        /// 
        /// </summary>
        Power,
        /// <summary>
        /// !
        /// </summary>
        Not,
        /// <summary>
        /// &&
        /// </summary>
        And,
        /// <summary>
        /// ||
        /// </summary>
        Or,
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
        /// null
        /// </summary>
        Null,
        /// <summary>
        /// require,include,import,using
        /// </summary>
        Require,
        /// <summary>
        /// eval
        /// </summary>
        Eval,
        /// <summary>
        /// 说明符
        /// </summary>
        Identifier,
        /// <summary>
        /// 结束
        /// </summary>
        Finished,
    }
}