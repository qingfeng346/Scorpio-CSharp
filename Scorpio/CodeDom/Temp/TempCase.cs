using System.Collections.Generic;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Runtime;
namespace Scorpio.CodeDom.Temp
{
    /// <summary> switch语句中一个cast条件</summary>
    internal class TempCase {
        public ScriptExecutable Executable;        //指令列表
        public CodeObject[] Allow;             //判断条件
        public TempCase(Script script, List<CodeObject> allow, ScriptExecutable executable) {
            this.Allow = allow != null ? allow.ToArray() : new CodeObject[0];
            this.Executable = executable;
        }
    }
}
