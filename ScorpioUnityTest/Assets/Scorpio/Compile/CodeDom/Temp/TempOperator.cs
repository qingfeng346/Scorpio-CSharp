using System.Collections.Generic;
using Scorpio.Compile.Compiler;
using Scorpio.Instruction;
namespace Scorpio.Compile.CodeDom.Temp {
    public class TempOperator {
        const int HighOperate = 6;
        const int LowOperate = 5;
        const int Compare = 4;
        const int BitOperate = 3;
        const int Logical = 2;
        //运算符优先级表 优先级高的 先执行
        private static Dictionary<TokenType, TempOperator> Operators = new Dictionary<TokenType, TempOperator>();
        static TempOperator() {
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
        public TempOperator(TokenType oper, int level) {
            this.Operator = oper;
            this.Level = level;
        }
        //获得运算符
        public static TempOperator GetOperator(TokenType oper) {
            if (Operators.TryGetValue(oper, out var ret))
                return ret;
            return null;
        }

        public static Opcode GetOpcode(TokenType type) {
            switch (type) {
                case TokenType.Plus:
                case TokenType.PlusAssign:
                    return Opcode.Plus;
                case TokenType.Minus:
                case TokenType.MinusAssign:
                    return Opcode.Minus;
                case TokenType.Multiply:
                case TokenType.MultiplyAssign:
                    return Opcode.Multiply;
                case TokenType.Divide:
                case TokenType.DivideAssign:
                    return Opcode.Divide;
                case TokenType.Modulo:
                case TokenType.ModuloAssign:
                    return Opcode.Modulo;
                case TokenType.InclusiveOr:
                case TokenType.InclusiveOrAssign:
                    return Opcode.InclusiveOr;
                case TokenType.Combine:
                case TokenType.CombineAssign:
                    return Opcode.Combine;
                case TokenType.XOR:
                case TokenType.XORAssign:
                    return Opcode.XOR;
                case TokenType.Shi:
                case TokenType.ShiAssign:
                    return Opcode.Shi;
                case TokenType.Shr:
                case TokenType.ShrAssign:
                    return Opcode.Shr;
                case TokenType.Greater: return Opcode.Greater;
                case TokenType.GreaterOrEqual: return Opcode.GreaterOrEqual;
                case TokenType.Less: return Opcode.Less;
                case TokenType.LessOrEqual: return Opcode.LessOrEqual;
                case TokenType.Equal: return Opcode.Equal;
                case TokenType.NotEqual: return Opcode.NotEqual;
                default: return Opcode.None;
            }
        }
    }
}
