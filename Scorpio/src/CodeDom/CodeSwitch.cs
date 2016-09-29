using System.Collections.Generic;
using Scorpio.CodeDom.Temp;
namespace Scorpio.CodeDom
{
    //switch语句
    public class CodeSwitch : CodeObject
    {
        internal CodeObject Condition;
        internal TempCase Default;
        internal TempCase[] Cases;
        internal void SetCases(List<TempCase> Cases) {
            this.Cases = Cases.ToArray();
        }
    }
}
