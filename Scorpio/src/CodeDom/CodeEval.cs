using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.CodeDom
{
    //动态执行一段代码 eval("print(123)")  相当于loadstring
    public class CodeEval : CodeObject
    {
        public CodeObject EvalObject;
    }
}
