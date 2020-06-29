
using System.Collections.Generic;
namespace Scorpio.Compile.CodeDom {
    //数组类型 [xxx,xxx,xxx]
    public class CodeArray : CodeObject {
        public List<CodeObject> Elements = new List<CodeObject>();
        public CodeArray(int line) : base(line) { }
    }
}
