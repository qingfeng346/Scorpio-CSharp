using Scorpio.Library;
namespace Scorpio {
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstanceBase {
        public string MethodName = "";
        public ScriptFunction(Script script) : base(script.TypeFunction) {
#if SCORPIO_DEBUG
            MethodName = Tools.ScorpioProfiler.RecordStack.ToString();
#endif
        }
        public virtual ScriptValue BindObject => default;
        public abstract ScriptFunction SetBindObject(ScriptValue obj);
        public override string ToString() { return $"Function<{MethodName}>"; }
        internal override void SerializerJson(ScorpioJsonSerializer jsonSerializer) {
            jsonSerializer.Serializer(ToString());
        }
    }
}
