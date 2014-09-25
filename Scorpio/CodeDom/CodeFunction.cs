using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;

namespace Scorpio.CodeDom
{
    //返回一个内部函数  内部函数会继承父函数的所有临时变量  function t1() {  return function t2() {}  }
    public class CodeFunction : CodeObject
    {
        public ScriptFunction Func;
        public CodeFunction(ScriptFunction func)
        {
            this.Func = func;
        }
    }
}
