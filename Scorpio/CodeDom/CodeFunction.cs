using Scorpio.Function;
namespace Scorpio.CodeDom
{
    //返回一个内部函数  内部函数会继承父函数的所有临时变量  function t1() {  return function t2() {}  }
    public class CodeFunction : CodeObject
    {
        public ScriptScriptFunction Func;
        public CodeFunction(ScriptScriptFunction func)
        {
            this.Func = func;
        }
        public CodeFunction(ScriptScriptFunction func, string breviary, int line) : base(breviary, line)
        {
            this.Func = func;
        }
    }
}
