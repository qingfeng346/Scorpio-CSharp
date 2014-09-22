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
        object run(object[] Parameters);
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
    }
    //脚本函数类型
    public class ScriptFunction : ScriptObject
    {
        public String Name { get; private set; }                    //函数名字
        public FunstionType FunctionType { get; private set; }      //函数类型 （是 脚本函数 还是 程序函数）

        private ScorpioScriptFunction m_ScriptFunction;             //脚本函数
        private ScorpioFunction m_Function;                         //程序函数指针
        private ScorpioHandle m_Handle;                             //程序函数执行类
        private ScorpioDelegate m_Delegate;                         //程序函数动态委托
        public ScriptFunction(ScorpioFunction function) : this(function.Method.Name, function) { }
        public ScriptFunction(String strName, ScorpioFunction function)
        {
            this.m_Function = function;
            Initialize(strName, FunstionType.Function);
        }
        public ScriptFunction(ScorpioHandle handle) : this(handle.GetType().Name, handle) { }
        public ScriptFunction(String strName, ScorpioHandle handle)
        {
            this.m_Handle = handle;
            Initialize(strName, FunstionType.Handle);
        }
        public ScriptFunction(Delegate dele) : this(dele.Method.Name, dele) { }
        public ScriptFunction(String strName, Delegate dele)
        {
            this.m_Delegate = new ScorpioDelegate(dele);
            Initialize(strName, FunstionType.Delegate);
        }
        public ScriptFunction(String strName, ScorpioScriptFunction function)
        {
            this.m_ScriptFunction = function;
            Initialize(strName, FunstionType.Script);
        }
        private void Initialize(String strName, FunstionType funcType)
        {
            GetType().GetMethod("");
            Type = ObjectType.Function;
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
        public ScriptObject Call(params object[] args)
        {
            if (FunctionType == FunstionType.Script) {
                return m_ScriptFunction.Call(args);
            } else {
                int length = args.Length;
                ScriptObject[] par = new ScriptObject[length];
                for (int i = 0; i < length; ++i) {
                    par[i] = CreateObject(args[i]);
                }
                if (FunctionType == FunstionType.Function) {
                    return CreateObject(m_Function(par));
                } else if (FunctionType == FunstionType.Handle) {
                    return CreateObject(m_Handle.run(par));
                } else if (FunctionType == FunstionType.Delegate) {
                    return CreateObject(m_Delegate.Call(par));
                }
            }
            return null;
        }
        public override string ToString() { return "Function(" + Name + ")"; }
    }
}
