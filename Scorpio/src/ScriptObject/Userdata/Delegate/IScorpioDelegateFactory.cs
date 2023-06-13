using System;
namespace Scorpio {
    public interface IScorpioDelegateFactory {
        Delegate CreateDelegate(Script script, Type delegateType, ScriptValue scriptValue);
    }
}
