using System;
using System.Collections.Generic;
using Scorpio;
public class TestDelegateFactory : IScorpioDelegateFactory {
    public static void Initialize() {
        ScorpioDelegateFactoryManager.SetFactory(new TestDelegateFactory());
    }
    public Delegate CreateDelegate(Script script, Type delegateType, ScriptValue scriptValue) {
        if (delegateType == typeof(System.Action)) {
            var value = new ScorpioDelegateReference(script, scriptValue);
            return new System.Action( () => value.call() );
        }
        if (delegateType == typeof(System.Action<System.Int32,System.Int32,System.String>)) {
            var value = new ScorpioDelegateReference(script, scriptValue);
            return new System.Action<System.Int32,System.Int32,System.String>( (arg0, arg1, arg2) => value.call(arg0, arg1, arg2) );
        }
        if (delegateType == typeof(System.Func<System.Int32,System.Int32,System.String>)) {
            var value = new ScorpioDelegateReference(script, scriptValue);
            return new System.Func<System.Int32,System.Int32,System.String>( (arg0, arg1) => value.call(arg0, arg1).ToString() );
        }
        throw new Exception("Delegate Type is not found : " + delegateType + "  scriptValue : " + scriptValue);
    }
}