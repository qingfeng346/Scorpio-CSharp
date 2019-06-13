using System;
namespace Scorpio
{
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstance {
        protected Script m_Script;
        protected ScriptValue m_BindObject = ScriptValue.Null;
        public ScriptFunction(Script script, String name) : base(ObjectType.Function, script.TypeFunctionValue) {
            m_Script = script;
            FunctionName = name;
        }
        public string FunctionName { get; private set; }
        public ScriptValue BindObject { get { return m_BindObject; } }
        public abstract ScriptFunction SetBindObject(ScriptValue obj);
        public override string ToString() { return $"Function<{FunctionName}>"; }
    }
}
