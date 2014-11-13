using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom.Temp;
namespace Scorpio.CodeDom
{
    public class CodeSwitch : CodeObject
    {
        internal CodeObject Condition;
        internal TempCase Default;
        internal List<TempCase> Cases = new List<TempCase>();
        internal void AddCase(TempCase con)
        {
            Cases.Add(con);
        }
    }
}
