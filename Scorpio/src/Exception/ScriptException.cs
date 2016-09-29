using System;
namespace Scorpio.Exception
{
    //脚本异常
    public class ScriptException : System.Exception
    {
        public ScriptException(String strMessage) : base(strMessage) { }
    }
}
