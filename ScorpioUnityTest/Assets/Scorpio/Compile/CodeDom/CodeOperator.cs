using Scorpio.Compile.Compiler;
namespace Scorpio.Compile.CodeDom {
    //运算符号
    public class CodeOperator : CodeObject {
        public CodeObject Left;             //左边值
        public CodeObject Right;            //右边值
        public TokenType TokenType;         //符号类型
        public CodeOperator(CodeObject Right, CodeObject Left, TokenType type, int line) : base(line) {
            this.Left = Left;
            this.Right = Right;
            this.TokenType = type;
        }
    }
}
