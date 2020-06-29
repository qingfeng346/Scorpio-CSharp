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
        public const int PlusIndex = 0;                                 //运算符重载 +
        public const int MinusIndex = 1;                                //运算符重载 -
        public const int MultiplyIndex = 2;                             //运算符重载 *
        public const int DivideIndex = 3;                               //运算符重载 /
        public const int ModuloIndex = 4;                               //运算符重载 %
        public const int InclusiveOrIndex = 5;                          //运算符重载 |
        public const int CombineIndex = 6;                              //运算符重载 &
        public const int XORIndex = 7;                                  //运算符重载 ^
        public const int ShiIndex = 8;                                  //运算符重载 <<
        public const int ShrIndex = 9;                                  //运算符重载 >>
                                                                        
        public const int GreaterIndex = 10;                             //运算符重载 >
        public const int GreaterOrEqualIndex = 11;                      //运算符重载 >=
        public const int LessIndex = 12;                                //运算符重载 <
        public const int LessOrEqualIndex = 13;                         //运算符重载 >=
        public const int EqualIndex = 14;                               //运算符重载 ==
                                                                        
        public const int GetItemIndex = 15;                             //运算符重载 [] get
        public const int SetItemIndex = 16;                             //运算符重载 [] set
        public const int OperatorCount = 17;
        
        

        public const string Plus = "op_Addition";                       //运算符重载 +
        public const string Minus = "op_Subtraction";                   //运算符重载 -
        public const string Multiply = "op_Multiply";                   //运算符重载 *
        public const string Divide = "op_Division";                     //运算符重载 /
        public const string Modulo = "op_Modulus";                      //运算符重载 %
        public const string InclusiveOr = "op_BitwiseOr";               //运算符重载 |
        public const string Combine = "op_BitwiseAnd";                  //运算符重载 &
        public const string XOR = "op_ExclusiveOr";                     //运算符重载 ^
        public const string Shi = "op_LeftShift";                       //运算符重载 <<
        public const string Shr = "op_RightShift";                      //运算符重载 >>

        public const string Greater = "op_GreaterThan";                 //运算符重载 >
        public const string GreaterOrEqual = "op_GreaterThanOrEqual";   //运算符重载 >=
        public const string Less = "op_LessThan";                       //运算符重载 <
        public const string LessOrEqual = "op_LessThanOrEqual";         //运算符重载 >=
        public const string Equal = "op_Equality";                      //运算符重载 ==

        public const string GetItem = "get_Item";                       //运算符重载 [] get
        public const string SetItem = "set_Item";                       //运算符重载 [] set
    }
}
