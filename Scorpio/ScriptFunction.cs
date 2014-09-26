using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Scorpio.Runtime;
using Scorpio.Variable;

namespace Scorpio
{
    //C#函数指针
    public delegate object ScorpioFunction(object[] Parameters);
    //C#类执行
    public interface ScorpioHandle {
        object Call(object[] Parameters);
    }
    /// <summary> 函数类型 </summary>
    public enum FunstionType
    {
        //脚本函数
        Script,
        //注册的C函数
        Function,
        //注册的C函数
        Handle,
        //动态委托
        Delegate,
        //函数
        Method,
    }
    //脚本函数类型
    public class ScriptFunction : ScriptObject
    {
        public Script Script { get; private set; }                  //所在脚本
        public String Name { get; private set; }                    //函数名字
        public FunstionType FunctionType { get; private set; }      //函数类型 （是 脚本函数 还是 程序函数）

        private ScorpioScriptFunction m_ScriptFunction;             //脚本函数
        private ScorpioFunction m_Function;                         //程序函数指针
        private ScorpioHandle m_Handle;                             //程序函数执行类
        private ScorpioDelegate m_Delegate;                         //程序函数动态委托
        private ScorpioMethod m_Method;                             //程序函数
        public override ObjectType Type { get { return ObjectType.Function; } }
        public ScriptFunction(Script script, ScorpioFunction function) : this(script, function.Method.Name, function) { }
        public ScriptFunction(Script script, String strName, ScorpioFunction function)
        {
            this.m_Function = function;
            Initialize(script, strName, FunstionType.Function);
        }
        public ScriptFunction(Script script, ScorpioHandle handle) : this(script, handle.GetType().Name, handle) { }
        public ScriptFunction(Script script, String strName, ScorpioHandle handle)
        {
            this.m_Handle = handle;
            Initialize(script, strName, FunstionType.Handle);
        }
        public ScriptFunction(Script script, Delegate dele) : this(script, dele.Method.Name, dele) { }
        public ScriptFunction(Script script, String strName, Delegate dele)
        {
            this.m_Delegate = new ScorpioDelegate(dele);
            Initialize(script, strName, FunstionType.Delegate);
        }
        public ScriptFunction(Script script, ScorpioMethod method) : this(script, method.MethodName, method) { }
        public ScriptFunction(Script script, String strName, ScorpioMethod method)
        {
            this.m_Method = method;
            Initialize(script, strName, FunstionType.Method);
        }
        internal ScriptFunction(Script script, String strName, ScorpioScriptFunction function)
        {
            this.m_ScriptFunction = function;
            Initialize(script, strName, FunstionType.Script);
        }
        private void Initialize(Script script, String strName, FunstionType funcType)
        {
            Script = script;
            Name = strName;
            FunctionType = funcType;
        }
        public void SetTable(ScriptTable table)
        {
            if (FunctionType == FunstionType.Script)
                m_ScriptFunction.SetTable(table);
        }
        public void SetParentContext(ScriptContext context)
        {
            if (FunctionType == FunstionType.Script)
                m_ScriptFunction.SetParentContext(context);
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            if (FunctionType == FunstionType.Script) {
                return m_ScriptFunction.Call(parameters);
            } else {
                if (FunctionType == FunstionType.Function) {
                    return Script.CreateObject(m_Function(parameters));
                } else if (FunctionType == FunstionType.Handle) {
                    return Script.CreateObject(m_Handle.Call(parameters));
                } else if (FunctionType == FunstionType.Delegate) {
                    return Script.CreateObject(m_Delegate.Call(parameters));
                } else if (FunctionType == FunstionType.Method) {
                    return Script.CreateObject(m_Method.Call(parameters));
                }
            }
            return null;
        }
        public override string ToString() { return "Function(" + Name + ")"; }
    }
}
