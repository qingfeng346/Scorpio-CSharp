namespace Scorpio {
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstance {
        public string MethodName = "";
        public ScriptFunction(Script script) : base(script, ObjectType.Function, script.TypeFunction) { }
        public virtual ScriptValue BindObject => default;
        public abstract ScriptFunction SetBindObject(ScriptValue obj);
        public override string ToString() { return $"Function<{MethodName}>"; }
    }
}
