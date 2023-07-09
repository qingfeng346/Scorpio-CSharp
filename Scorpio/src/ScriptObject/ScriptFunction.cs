namespace Scorpio {
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstance {
        protected Script m_Script;
        public ScriptFunction(Script script) : base(ObjectType.Function, script.TypeFunction) {
            m_Script = script;
        }
        public virtual ScriptValue BindObject => default;
        public abstract ScriptFunction SetBindObject(ScriptValue obj);
        public override string ToString() { return $"Function"; }
    }
}
