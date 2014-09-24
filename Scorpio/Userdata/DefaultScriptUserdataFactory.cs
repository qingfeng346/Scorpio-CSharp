using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Userdata
{
    public class DefaultScriptUserdataFactory : IScriptUserdataFactory
    {
        public ScriptUserdata create(Script script, object obj)
        {
            return new DefaultScriptUserdata(script, obj);
        }
    }
}
