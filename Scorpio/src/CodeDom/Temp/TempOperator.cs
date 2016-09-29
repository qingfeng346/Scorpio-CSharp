using System.Collections.Generic;
using Scorpio.Compiler;
namespace Scorpio.CodeDom.Temp
{
    public class TempOperator
    {
        const int HighOperate = 6;
        const int LowOperate = 5;
        const int Compare = 4;
        const int BitOperate = 3;
        const int Logical = 2;
        //运算符优先级表 优先级高的 先执行
        private static Dictionary<TokenType, TempOperator> Operators = new Dictionary<TokenType, TempOperator>();
		static TempOperator()
		{
            Operators[TokenType.And] = new TempOperator(TokenType.And, Logical);
            Operators[TokenType.Or] = new TempOperator(TokenType.Or, Logical);

            Operators[TokenType.InclusiveOr] = new TempOperator(TokenType.InclusiveOr, BitOperate);
            Operators[TokenType.Combine] = new TempOperator(TokenType.Combine, BitOperate);
            Operators[TokenType.XOR] = new TempOperator(TokenType.XOR, BitOperate);
            Operators[TokenType.Shi] = new TempOperator(TokenType.Shi, BitOperate);
            Operators[TokenType.Shr] = new TempOperator(TokenType.Shr, BitOperate);

            Operators[TokenType.Equal] = new TempOperator(TokenType.Equal, Compare);
            Operators[TokenType.NotEqual] = new TempOperator(TokenType.NotEqual, Compare);
            Operators[TokenType.Greater] = new TempOperator(TokenType.Greater, Compare);
            Operators[TokenType.GreaterOrEqual] = new TempOperator(TokenType.GreaterOrEqual, Compare);
            Operators[TokenType.Less] = new TempOperator(TokenType.Less, Compare);
            Operators[TokenType.LessOrEqual] = new TempOperator(TokenType.LessOrEqual, Compare);

            Operators[TokenType.Plus] = new TempOperator(TokenType.Plus, LowOperate);
            Operators[TokenType.Minus] = new TempOperator(TokenType.Minus, LowOperate);

            Operators[TokenType.Multiply] = new TempOperator(TokenType.Multiply, HighOperate);
            Operators[TokenType.Divide] = new TempOperator(TokenType.Divide, HighOperate);
            Operators[TokenType.Modulo] = new TempOperator(TokenType.Modulo, HighOperate);
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
