using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    //try catch 语句
    public class CodeTry : CodeObject
    {
        public ScriptExecutable TryExecutable;      //try指令执行
        public ScriptExecutable CatchExecutable;    //catch指令执行
        public string Identifier;                   //异常对象
    }
}
