using System;
using System.Collections.Generic;
using Scorpio;
public class TestDelegateFactory : IScorpioDelegateFactory {
    public static void Initialize() {
        ScorpioDelegateFactoryManager.SetFactory(new TestDelegateFactory());
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {
        if (delegateType == typeof(System.Action))
            return new System.Action( () => { scriptObject.call(ScriptValue.Null); } );
        if (delegateType == typeof(System.Action<System.Int32,System.Int32,System.String>))
            return new System.Action<System.Int32,System.Int32,System.String>( (arg0,arg1,arg2) => { scriptObject.call(ScriptValue.Null,arg0,arg1,arg2); } );
        if (delegateType == typeof(System.Func<System.Int32,System.Int32,System.String>))
            return new System.Func<System.Int32,System.Int32,System.String>( (arg0,arg1) => { return scriptObject.call(ScriptValue.Null,arg0,arg1).ToString(); } );
        if (delegateType == typeof(System.Func<System.String,System.String>))
            return new System.Func<System.String,System.String>( (arg0) => { return scriptObject.call(ScriptValue.Null,arg0).ToString(); } );
        throw new Exception("Delegate Type is not found : " + delegateType + "  scriptObject : " + scriptObject);
    }
}