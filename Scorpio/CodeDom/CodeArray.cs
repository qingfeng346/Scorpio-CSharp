using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.CodeDom
{
    //数组类型 [xxx,xxx,xxx]
    public class CodeArray : CodeObject
    {
        public List<CodeObject> _Elements = new List<CodeObject>();
        public CodeObject[] Elements;
        public void Init() {
            Elements = _Elements.ToArray();
        }
    }
}
