using System;
namespace Scorpio {
    public interface IScorpioDelegateFactory {
        Delegate CreateDelegate(Type delegateType, ScriptValue scriptValue);
    }
}
