namespace Scorpio.Compile.CodeDom {
    //返回一个类
    public class CodeClass : CodeObject {
        public int index;
        public CodeClass(int index, int line) : base(line) {
            this.index = index;
        }
    }
}
