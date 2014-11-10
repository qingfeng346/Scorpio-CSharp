using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    public class StackInfo
    {
        public string Breviary = "";     // 文件摘要
        public int Line = 1;             // 起始关键字所在行数
        public StackInfo() { }
        public StackInfo(string breviary, int line)
        {
            Breviary = breviary;
            Line = line;
        }
    }
}
