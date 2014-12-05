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
        private static Dictionary<TokenType, TempOperator> Operators = new Dictionary<TokenType, TempOperator>();
		static TempOperator()
		{
            Operators[TokenType.InclusiveOr] = new TempOperator(TokenType.InclusiveOr, 1);
            Operators[TokenType.Combine] = new TempOperator(TokenType.Combine, 1);
            Operators[TokenType.XOR] = new TempOperator(TokenType.XOR, 1);
            Operators[TokenType.Shi] = new TempOperator(TokenType.Shi, 1);
            Operators[TokenType.Shr] = new TempOperator(TokenType.Shr, 1);
            Operators[TokenType.And] = new TempOperator(TokenType.And, 1);
            Operators[TokenType.Or] = new TempOperator(TokenType.Or, 1);

            Operators[TokenType.Equal] = new TempOperator(TokenType.Equal, 2);
            Operators[TokenType.NotEqual] = new TempOperator(TokenType.NotEqual, 2);
            Operators[TokenType.Greater] = new TempOperator(TokenType.Greater, 2);
            Operators[TokenType.GreaterOrEqual] = new TempOperator(TokenType.GreaterOrEqual, 2);
            Operators[TokenType.Less] = new TempOperator(TokenType.Less, 2);
            Operators[TokenType.LessOrEqual] = new TempOperator(TokenType.LessOrEqual, 2);

            Operators[TokenType.Plus] = new TempOperator(TokenType.Plus, 3);
            Operators[TokenType.Minus] = new TempOperator(TokenType.Minus, 3);

            Operators[TokenType.Multiply] = new TempOperator(TokenType.Multiply, 4);
            Operators[TokenType.Divide] = new TempOperator(TokenType.Divide, 4);
            Operators[TokenType.Modulo] = new TempOperator(TokenType.Modulo, 4);
		}
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
