using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
using Scorpio.CodeDom;
namespace Scorpio.CodeDom.Temp
{
    public class TempOperator
    {
        //运算符优先级表，
        private static Dictionary<TokenType, TempOperator> Operators = new Dictionary<TokenType, TempOperator>() { 
            { TokenType.And, new TempOperator(TokenType.And, 1) },
            { TokenType.Or, new TempOperator(TokenType.Or, 1) },

            { TokenType.Equal, new TempOperator(TokenType.Equal, 2) },
            { TokenType.NotEqual, new TempOperator(TokenType.NotEqual, 2) },
            { TokenType.Greater, new TempOperator(TokenType.Greater, 2) },
            { TokenType.GreaterOrEqual, new TempOperator(TokenType.GreaterOrEqual, 2) },
            { TokenType.Less, new TempOperator(TokenType.Less, 2) },
            { TokenType.LessOrEqual, new TempOperator(TokenType.LessOrEqual, 2) },

            { TokenType.Plus, new TempOperator(TokenType.Plus, 3) },
            { TokenType.Minus, new TempOperator(TokenType.Minus, 3) },

            { TokenType.Multiply, new TempOperator(TokenType.Multiply, 4) },
            { TokenType.Divide, new TempOperator(TokenType.Divide, 4) },
            { TokenType.Modulo, new TempOperator(TokenType.Modulo, 4) },
        };

        public TokenType Operator;      //符号类型
        public int Level;               //优先级
        public TempOperator(TokenType oper, int level)
        {
            this.Operator = oper;
            this.Level = level;
        }
        //获得运算符
        public static TempOperator GetOper(TokenType oper)
        {
            if (Operators.ContainsKey(oper))
                return Operators[oper];
            return null;
        }

    }
}
