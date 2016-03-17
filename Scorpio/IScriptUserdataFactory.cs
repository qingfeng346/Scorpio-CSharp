using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Userdata;
namespace Scorpio
{
    public interface IScriptUserdataFactory
    {
        ScriptUserdata GetEnum(Type type);
        ScriptUserdata GetDelegate(Type type);
        UserdataType GetScorpioType(Type type);
        ScriptUserdata create(Script script, object obj);
    }
}
