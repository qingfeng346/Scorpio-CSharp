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
        public string Breviary;     // 摘要
        public int Line;            // 起始关键字所在行数
        public CodeObject() { }
        public CodeObject(string breviary, int line)
        {
            this.Breviary = breviary;
            this.Line = line;
        }
    }
}
