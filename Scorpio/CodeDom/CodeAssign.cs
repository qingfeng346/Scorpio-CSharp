using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
namespace Scorpio.CodeDom
{
    //复制变量 = += -= /= *= %=
    public class CodeAssign : CodeObject
    {
        public CodeMember member;
        public CodeObject value;
        public TokenType AssignType;
        public CodeAssign(CodeMember member, CodeObject value, TokenType assignType, string breviary, int line) : base(breviary, line)
        {
            this.member = member;
            this.value = value;
            this.AssignType = assignType;
        }
    }
}
