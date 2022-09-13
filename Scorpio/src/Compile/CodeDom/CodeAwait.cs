using Scorpio.Compile.CodeDom;

namespace Scorpio.Compile.CodeDom {
    internal class CodeAwait : CodeObject {
        public CodeAwait(int line) : base(line) { }
        public CodeObject codeObject;
    }
}
