using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Userdata
{
    public class DefaultScriptUserdataFactory : IScriptUserdataFactory
    {
        public ScriptUserdata create(Script script, object obj)
        {
            if (obj is Type && ((Type)obj).IsEnum)
                return new DefaultScriptUserdataEnum(script, obj);
            return new DefaultScriptUserdataObject(script, obj);
        }
    }
}
