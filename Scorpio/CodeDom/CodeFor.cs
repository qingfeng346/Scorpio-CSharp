using Scorpio.Runtime;
namespace Scorpio.CodeDom
{
    //for循环 for (var i=0;i<10;++i) {}
    public class CodeFor : CodeObject {
        public ScriptExecutable BeginExecutable;            //开始执行
        public CodeObject Condition;                        //跳出条件
        public ScriptExecutable LoopExecutable;             //循环执行
        public ScriptExecutable BlockExecutable;            //for内容
    }
}
