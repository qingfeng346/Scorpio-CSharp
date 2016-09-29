using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
namespace Scorpio.CodeDom
{
    //运算符号   
    public class CodeOperator : CodeObject
    {
        public CodeObject Left;             //左边值
        public CodeObject Right;            //右边值
        public TokenType Operator;          //符号类型
        public CodeOperator(CodeObject Right, CodeObject Left, TokenType type, string breviary, int line) : base(breviary, line)
        {
            this.Left = Left;
            this.Right = Right;
            this.Operator = type;
        }
    }
}
