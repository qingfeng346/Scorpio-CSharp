using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.CodeDom
{
    //数组类型 [xxx,xxx,xxx]
    public class CodeArray : CodeObject
    {
        public List<CodeObject> Elements = new List<CodeObject>();
    }
}
