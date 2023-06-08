using System;
namespace Scorpio.Function {
    //生成后的模板实例函数,第一个参数为对象
    public class ScriptGenericMethodFunction : ScriptMethodFunction {
        public ScriptGenericMethodFunction(Script script) : base(script) { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var args = new ScriptValue[length - 1];
            Array.Copy(parameters, 1, args, 0, args.Length);
            return ScriptValue.CreateValue(m_Script, Method.Call(m_Script, true, parameters[0].Value, args, args.Length));
        }
        public override void Free() {
            Method = null;
            m_Script.Free(this);
        }
    }
}
