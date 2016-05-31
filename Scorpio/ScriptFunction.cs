using System;
namespace Scorpio
{
    //C#函数指针
    public delegate object ScorpioFunction(Script script, ScriptObject[] Parameters);
    //C#类执行
    public interface ScorpioHandle {
        object Call(ScriptObject[] Parameters);
    }
    /// <summary> 函数类型 </summary>
    //脚本函数类型
    public class ScriptFunction : ScriptObject {
        public override ObjectType Type { get { return ObjectType.Function; } }
        public ScriptFunction(Script script, String name) : base(script) {
            Name = name;
        }
        public virtual int GetParamCount() { return 0; }
        public virtual bool IsParams() { return false; }
        public virtual bool IsStatic() { return false; }
        public virtual ScriptArray GetParams() { return m_Script.CreateArray(); }
        public override string ToString() { return "Function(" + Name + ")"; }
        public override string ToJson() { return "\"Function\""; }
    }
}
