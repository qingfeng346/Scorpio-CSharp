namespace Scorpio.Compile.CodeDom {
    //区域变量 () 内包括的变量
    public class CodeRegion : CodeObject {
        public CodeObject Context;            //变量
        public CodeRegion(CodeObject Context, int line) : base(line) { this.Context = Context; }
    }
}
