using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    interface IScriptUserdataFactory
    {
        ScriptUserdata create(Script script, object obj);
    }
}
