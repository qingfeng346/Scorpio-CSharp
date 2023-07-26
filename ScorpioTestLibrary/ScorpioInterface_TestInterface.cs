using System;
using Scorpio;
public class ScorpioInterface_TestInterface : TestInterface {
    public ScriptValue Value { get; set; }
    public ScriptValue __Call(string functionName, params object[] args) {
        var func = Value.GetValue(functionName);
        if (func.valueType == ScriptValue.scriptValueType) {
            try {
                return func.call(Value, args);
            } catch (System.Exception e) {
                throw new System.Exception($"ScriptInterce Call is error Type:ScorpioInterface_TestInterface  Function:{functionName} error:{e.ToString()}");
            }
        }
        return ScriptValue.Null;
    }
    public void Func1() {
        __Call("Func1");
    }
    public void Func2() {
        __Call("Func2");
    }
    public System.Int32 Func3() {
        return (System.Int32)Convert.ChangeType(__Call("Func3").Value, typeof(System.Int32));
    }
    public System.String Func4() {
        return __Call("Func4").ToString();
    }
}