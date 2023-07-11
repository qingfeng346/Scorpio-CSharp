namespace Scorpio
{
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstance {
        public ScriptFunction(Script script) : base(script, ObjectType.Function) { }
        public virtual ScriptValue BindObject => ScriptValue.Null;
        public abstract ScriptFunction SetBindObject(ScriptValue obj);
        public override void Alloc() {
            SetPrototype(script.TypeFunction);
        }
        public abstract override void Free();
        public override string ToString() { return $"Function"; }
    }
}
