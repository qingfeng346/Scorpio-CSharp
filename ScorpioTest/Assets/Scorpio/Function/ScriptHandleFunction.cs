using System;
using Scorpio.Exception;
namespace Scorpio.Function {
    public class ScriptHandleFunction : ScriptFunction {
        protected ScorpioHandle m_Handle;                                         //程序函数执行类
        public ScriptHandleFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().FullName, handle) { }
        public ScriptHandleFunction(Script script, string name, ScorpioHandle handle) : base(script, name) {
            m_Handle = handle;
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            try {
                return m_Handle.Call(thisObject, parameters, length);
            } catch (System.Exception ex) {
                throw new ExecutionException("CallFunction [" + FunctionName + "] is error : " + ex.ToString());
            }
        }
        public override ScriptFunction SetBindObject(ScriptValue obj) {
            var ret = new ScriptHandleBindFunction(m_Script, FunctionName, m_Handle);
            ret.m_BindObject = obj;
            return ret;
        }
    }
    public class ScriptHandleBindFunction : ScriptHandleFunction {
        public ScriptHandleBindFunction(Script script, string name, ScorpioHandle handle) : base(script, name, handle) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            try {
                return m_Handle.Call(m_BindObject.valueType == ScriptValue.nullValueType ? thisObject : m_BindObject, parameters, length);
            } catch (System.Exception ex) {
                throw new ExecutionException("CallFunction [" + FunctionName + "] is error : " + ex.ToString());
            }
        }
    }
}
