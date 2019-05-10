using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio {
    //运算符重载
    public class ScriptOperator {
        public const string Plus = "+";
        public const string Minus = "-";
        public const string Multiply = "*";
        public const string Divide = "/";
        public const string Modulo = "%";
        public const string InclusiveOr = "|";
        public const string Combine = "&";
        public const string XOR = "^";
        public const string Shi = "<<";
        public const string Shr = ">>";
        public const string Greater = ">";
        public const string GreaterOrEqual = ">=";
        public const string Less = "<";
        public const string LessOrEqual = "<=";
        public const string Equal = "==";
        public const string NotEqual = "!=";
        public const string Invoke = "()";
        public const string Constructor = "constructor";       //构造函数
    }
    //c#运算符重载
    public class UserdataOperator {
        public const string Plus = "op_Addition";
        public const string Minus = "op_Subtraction";
        public const string Multiply = "op_Multiply";
        public const string Divide = "op_Division";
        public const string Modulo = "op_Modulus";
        public const string InclusiveOr = "op_BitwiseOr";
        public const string Combine = "op_BitwiseAnd";
        public const string XOR = "op_ExclusiveOr";
        public const string Shi = "op_LeftShift";
        public const string Shr = "op_RightShift";

        public const string Greater = "op_GreaterThan";
        public const string GreaterOrEqual = "op_GreaterThanOrEqual";
        public const string Less = "op_LessThan";
        public const string LessOrEqual = "op_LessThanOrEqual";
    }
}
