using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Scorpio.Runtime;
using Scorpio.Collections;
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
        Spcript,
        //注册的C函数
        Function,
        //注册的C函数
        Handle,
    }
    //脚本函数类型
    public class ScriptFunction : ScriptObject
    {
        public String Name { get; private set; }                    //函数名字
        public FunstionType FunctionType { get; private set; }      //函数类型 （是 脚本函数 还是 程序函数）
        private List<String> m_listParameters;                      //参数
        private ScorpioFunction m_function;                          //程序函数指针
        private ScorpioHandle m_handle;                              //程序函数执行类
        private Script m_script;                                    //脚本系统
        private ScriptContext m_parentContext;                      //父级上下文
        private ScriptExecutable m_scriptExecutable;                //函数执行命令
        private VariableDictionary m_scriptStackObject = new VariableDictionary();              //函数参数
        private ScriptTable m_scriptTable;                          //是否是table内部函数(如果不为null则为内部函数)
        public ScriptFunction(ScorpioFunction function) : this("", function) { }
        public ScriptFunction(String strName, ScorpioFunction function)
        {
            this.Type = ObjectType.Function;
            this.FunctionType = FunstionType.Function;
            this.Name = strName;
            this.m_function = function;
        }
        public ScriptFunction(ScorpioHandle handle) : this("", handle) { }
        public ScriptFunction(String strName, ScorpioHandle handle)
        {
            this.Type = ObjectType.Function;
            this.FunctionType = FunstionType.Handle;
            this.Name = strName;
            this.m_handle = handle;
        }
        public ScriptFunction(Script script, String strName, List<String> listParameters, ScriptExecutable scriptExecutable)
        {
            this.Type = ObjectType.Function;
            this.FunctionType = FunstionType.Spcript;
            this.m_script = script;
            this.Name = strName;
            this.m_listParameters = new List<string>(listParameters);
            this.m_scriptExecutable = scriptExecutable;
        }
        public void SetTable(ScriptTable table)
        {
            m_scriptTable = table;
            m_scriptStackObject["this"] = m_scriptTable;
            m_scriptStackObject["self"] = m_scriptTable;
        }
        public void SetParentContext(ScriptContext context)
        {
            m_parentContext = context;
        }
        public ScriptObject Call(params object[] args)
        {
            int length = args.Length;
            if (FunctionType == FunstionType.Function) {
                ScriptObject[] par = new ScriptObject[length];
                for (int i = 0; i < length; ++i) {
                    par[i] = CreateObject(args[i]);
                }
                return CreateObject(m_function(par));
            } else if (FunctionType == FunstionType.Handle) {
                ScriptObject[] par = new ScriptObject[length];
                for (int i = 0; i < length; ++i) {
                    par[i] = CreateObject(args[i]);
                }
                return CreateObject(m_handle.run(par));
            } else {
                for (int i = 0; i < m_listParameters.Count; ++i) {
                    m_scriptStackObject[m_listParameters[i]] = (args != null && length > i) ? CreateObject(args[i]) : ScriptNull.Instance;
                }
                ScriptContext context = new ScriptContext(m_script, m_scriptExecutable, m_parentContext, Executable_Block.Function);
                context.Initialize(m_scriptStackObject);
                return context.Execute();
            }
        }
        public int ParameterCount { get { return m_listParameters.Count; } }
        public ReadOnlyCollection<String> Parameters { get { return m_listParameters.AsReadOnly(); } }
        public override string ToString() { return "Function(" + Name + ")"; }
    }
}
