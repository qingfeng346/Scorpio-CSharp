using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Compile.CodeDom {
    public class CodeFunctionParameter {
        public bool unfold = false;     //参数是否需要展开
        public CodeObject obj;          //参数
    }
    //调用一个函数 Member:函数对象 Parameters:函数参数   hello(p1,p2,p3)
    public class CodeCallFunction : CodeObject {
        public CodeObject Member;
        public CodeMap Variables;
        public bool nullTo = false;     ///?. 调用函数
        public List<CodeFunctionParameter> Parameters;
        public CodeCallFunction(CodeObject member, List<CodeFunctionParameter> parameters, int line) : base(line) {
            this.Member = member;
            this.Parameters = parameters;
        }
    }
}
