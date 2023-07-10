//using System;
//using Scorpio;
//public class ScorpioInterface_TestInterface : TestInterface {
//    private Script m_script;
//    private ScriptValue m_value;
//    public ScriptValue Value {
//        get => m_value;
//        set {
//            Shutdown();
//            m_value = value.Reference();
//            m_script = m_value.Get<ScriptInstance>()?.script;
//        }
//    }
//    public void Shutdown() {
//        if (m_script != null && !m_script.IsShutdown) {
//            m_value.Free();
//        } else {
//            m_value = ScriptValue.Null;
//        }
//    }
//    public ScriptValue __Call(string functionName, params object[] args) {
//        if (m_script == null || m_script.IsShutdown) return ScriptValue.Null;
//        var func = m_value.GetValue(functionName);
//        if (func.valueType == ScriptValue.scriptValueType) {
//            try {
//                var value = func.call(m_value, args);
//                value.Release();
//                return value;
//            } catch (System.Exception e) {
//                throw new System.Exception($"ScriptInterce Call is error Type:ScorpioInterface_TestInterface  Function:{functionName} error:{e.ToString()}");
//            }
//        }
//        return ScriptValue.Null;
//    }
//    public void Func1() {
//        __Call("Func1");
//    }
//    public void Func2() {
//        __Call("Func2");
//    }
//    public System.Int32 Func3() {
//        return (System.Int32)Convert.ChangeType(__Call("Func3").Value, typeof(System.Int32));
//    }
//    public System.String Func4() {
//        return __Call("Func4").ToString();
//    }
//}