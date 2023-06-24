using System;
using Scorpio;
public class ScorpioInterface_TestInterface : TestInterface {
    private ScriptValue m_value;
    public ScriptValue Value {
        get => m_value;
        set { m_value.CopyFrom(value); }
    }
    public void Shutdown() {
        m_value.Free();    
    }
    public ScriptValue __Call(string functionName, params object[] args) {
        var func = m_value.GetValue(functionName);
        if (func.valueType == ScriptValue.scriptValueType) {
            try {
                var value = func.call(m_value, args);
                value.Release();
                return value;
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
    public System.String Func4(System.String a, System.String b, System.String c) {
        return __Call("Func4", a, b, c).ToString();
    }
}