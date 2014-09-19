using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;

namespace Scorpio.CodeDom
{
    public class CodeObject
    {
        public bool Not;            // ! 标识（非xxx）
        public bool Negative;       // - 标识（负数）
        public Token Token;         // 变量起始关键字
    }
}
