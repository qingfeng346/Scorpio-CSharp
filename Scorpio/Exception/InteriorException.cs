using System;
using Scorpio;
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
