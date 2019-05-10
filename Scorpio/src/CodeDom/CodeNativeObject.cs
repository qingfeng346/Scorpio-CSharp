using System;
namespace Scorpio.CodeDom {
    public class CodeNativeObject : CodeObject {
        public object obj;
        public CodeNativeObject(object obj, int line) : base(line) {
            this.obj = obj;
        }
    }
}
