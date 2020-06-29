namespace Scorpio.Compile.CodeDom {
    //三目运算符 true ? xxx : xxx
    public class CodeTernary : CodeObject {
        public CodeObject Allow; //判断条件
        public CodeObject True; //成立返回
        public CodeObject False; //不成立返回
        public CodeTernary(int line) : base(line) { }
    }
}
