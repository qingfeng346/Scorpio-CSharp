using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    public interface IScriptUserdataFactory
    {
        ScriptUserdata GetEnum(Type type);
        ScriptUserdata GetDelegate(Type type);
        ScriptUserdata create(Script script, object obj);
    }
}
