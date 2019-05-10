using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.CodeDom {
    //一个需要解析的Object
    public class CodeObject {
        public bool Not;
        public bool Minus;
        public bool Negative;
        public int line;            //所在行
        public CodeObject(int line) {
            this.line = line;
        }
        public int Line { get { return line; } }
    }
}
