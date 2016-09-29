namespace Scorpio.CodeDom
{
    //区域变量 () 内包括的变量
    public class CodeRegion : CodeObject
    {
        public CodeObject Context;            //变量
        public CodeRegion(CodeObject Context) { this.Context = Context; }
    }
}
