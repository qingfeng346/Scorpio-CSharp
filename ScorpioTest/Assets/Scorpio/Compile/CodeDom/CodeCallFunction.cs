using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Compile.CodeDom {
    //调用一个函数 Member:函数对象 Parameters:函数参数   hello(p1,p2,p3)
    public class CodeCallFunction : CodeObject {
        public CodeObject Member;
        public CodeMap Variables;
        public CodeObject[] Parameters;
        public int ParametersCount;
        public CodeCallFunction(CodeObject member, List<CodeObject> parameters, int line) : base(line) {
            this.Member = member;
            this.Parameters = parameters.ToArray();
            this.ParametersCount = parameters.Count;
        }
    }
}
