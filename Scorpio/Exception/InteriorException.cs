using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Exception
{
    public class InteriorException : System.Exception
    {
        public ScriptObject obj;
        public InteriorException(ScriptObject obj)
        {
            this.obj = obj;
        }
    }
}
