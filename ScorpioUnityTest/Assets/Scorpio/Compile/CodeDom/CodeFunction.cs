namespace Scorpio.Compile.CodeDom {
    //返回一个内部函数  内部函数会继承父函数的所有临时变量  function t1() {  return function t2() {}  }
    public class CodeFunction : CodeObject {
        public int func;
        public bool lambada;
        public CodeFunction(int func, int line) : this(func, false, line) { }
        public CodeFunction(int func, bool lambada, int line) : base(line) {
            this.lambada = lambada;
            this.func = func;
        }
    }
}
