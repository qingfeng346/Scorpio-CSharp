namespace Scorpio.Compile.CodeDom {
    //空返回 null ?? xxx
    public class CodeEmptyRet : CodeObject {
        public CodeObject Emtpy;    //判断条件
        public CodeObject Ret;      //成立返回
        public CodeEmptyRet(int line) : base(line) { }
    }
}
