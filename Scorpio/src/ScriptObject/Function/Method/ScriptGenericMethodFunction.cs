using System;
namespace Scorpio.Function {
    //生成后的模板实例函数,第一个参数为对象
    public class ScriptGenericMethodFunction : ScriptMethodFunction {
        public ScriptGenericMethodFunction(Script script) : base(script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var thisValue = parameters[0].Value;
            Array.Copy(parameters, 1, parameters, 0, length - 1);
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, true, thisValue, parameters, length - 1));
        }
        public override void Free() {
            DelRecord();
            MethodName = null;
            Method = null;
            m_Script.Free(this);
        }
        public override string ToString() {
            return $"模板函数 {Method}";
        }
    }
}
