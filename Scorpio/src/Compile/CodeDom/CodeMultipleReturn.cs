using System.Collections.Generic;
namespace Scorpio.Compile.CodeDom {
    public class CodeMultipleReturn : CodeObject {
        public List<string> Returns;
        public CodeObject Value;
        public CodeMultipleReturn(int line) : base(line) { }
    }
}
