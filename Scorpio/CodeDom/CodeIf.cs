using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom.Temp;

namespace Scorpio.CodeDom
{
    //if语句  if(true) {} elseif () {} else {}
    public class CodeIf : CodeObject
    {
        internal TempCondition If;
        internal TempCondition Else;
        internal TempCondition[] ElseIf;
        internal int ElseIfCount;
        internal void Init(List<TempCondition> ElseIf) {
            this.ElseIf = ElseIf.ToArray();
            this.ElseIfCount = ElseIf.Count;
        }
    }
}
