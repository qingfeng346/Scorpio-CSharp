using System;
namespace Scorpio
{
    //C#类执行
    public interface ScorpioHandle {
        ScriptValue Call(ScriptValue obj, ScriptValue[] Parameters, int length);
    }
    /// <summary> 函数类型 </summary>
    public abstract class ScriptFunction : ScriptInstance {
        public ScriptFunction(Script script, String name) : base(script, ObjectType.Function, script.TypeFunction) {
            FunctionName = name;
        }
        public string FunctionName { get; private set; }
        public ScriptValue BindObject { get; private set; }
        public ScriptFunction SetBindObject(ScriptValue obj) {
            var ret = Clone() as ScriptFunction;
            ret.BindObject = obj;
            return ret;
        }
        public override string ToString() { return $"Function<{FunctionName}>"; }
    }
}
