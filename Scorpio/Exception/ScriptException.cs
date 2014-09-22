using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    public class ScriptException : System.Exception
    {
        public ScriptException(String strMessage) : base(strMessage) { }
    }
}
