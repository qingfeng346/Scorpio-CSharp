namespace Scorpio.Compile.CodeDom {
    //返回一个类
    public class CodeClass : CodeObject {
        public int index;
        public bool async;
        public CodeClass(int index, bool async, int line) : base(line) {
            this.index = index;
            this.async = async;
        }
    }
}
