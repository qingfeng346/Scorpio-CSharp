using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
namespace Scorpio.CodeDom
{
    //一个需要解析的Object
    public class CodeObject
    {
        public bool Not;            // ! 标识（非xxx）
        public bool Negative;       // - 标识（负数）
        public StackInfo StackInfo;     // 堆栈数据
        public CodeObject() { }
        public CodeObject(string breviary, int line)
        {
            StackInfo = new StackInfo(breviary, line);
        }
    }
}
