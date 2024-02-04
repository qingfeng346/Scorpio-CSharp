using System;
using System.Collections.Generic;
using Scorpio;
public class TestDelegateFactory : IScorpioDelegateFactory {
    public static void Initialize() {
        ScorpioDelegateFactoryManager.AddFactory(new TestDelegateFactory());
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {
        if (delegateType == typeof(System.Action)) {
            #if SCORPIO_DEBUG
            var value = new ScorpioDelegateReference(scriptObject, typeof(System.Action));
            return new System.Action( () => { value.call(); } );
            #else
            return new System.Action( () => { scriptObject.call(ScriptValue.Null); } );
            #endif
        }
        if (delegateType == typeof(System.Action<System.Int32,System.Int32,System.String>)) {
            #if SCORPIO_DEBUG
            var value = new ScorpioDelegateReference(scriptObject, typeof(System.Action<System.Int32,System.Int32,System.String>));
            return new System.Action<System.Int32,System.Int32,System.String>( (arg0,arg1,arg2) => { value.call(arg0,arg1,arg2); } );
            #else
            return new System.Action<System.Int32,System.Int32,System.String>( (arg0,arg1,arg2) => { scriptObject.call(ScriptValue.Null,arg0,arg1,arg2); } );
            #endif
        }
        if (delegateType == typeof(System.Func<System.Int32,System.Int32,System.String>)) {
            #if SCORPIO_DEBUG
            var value = new ScorpioDelegateReference(scriptObject, typeof(System.Func<System.Int32,System.Int32,System.String>));
            return new System.Func<System.Int32,System.Int32,System.String>( (arg0,arg1) => { return value.call(arg0,arg1).ToString(); } );
            #else
            return new System.Func<System.Int32,System.Int32,System.String>( (arg0,arg1) => { return scriptObject.call(ScriptValue.Null,arg0,arg1).ToString(); } );
            #endif
        }
        if (delegateType == typeof(System.Func<System.String,System.String>)) {
            #if SCORPIO_DEBUG
            var value = new ScorpioDelegateReference(scriptObject, typeof(System.Func<System.String,System.String>));
            return new System.Func<System.String,System.String>( (arg0) => { return value.call(arg0).ToString(); } );
            #else
            return new System.Func<System.String,System.String>( (arg0) => { return scriptObject.call(ScriptValue.Null,arg0).ToString(); } );
            #endif
        }
        return null;
    }
}