namespace Scorpio.Compile.CodeDom {
    //一个需要解析的Object
    public class CodeObject {
        public bool Not;            //是否有 ! 标识
        public bool Minus;          //是否有 - 标识
        public bool Negative;       //是否有 ~ 标识
        public int line;            //所在行
        public CodeObject(int line) {
            this.line = line;
        }
        public int Line { get { return line; } }
    }
}
