using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Scorpio.Runtime;
using Scorpio.Collections;
namespace Scorpio.Variable
{
    internal class ScorpioScriptFunction
    {
        private Script m_script;                            //脚本系统
        private List<String> m_listParameters;              //参数
        private ScriptExecutable m_scriptExecutable;        //函数执行命令
        private ScriptContext m_parentContext;              //父级上下文
        private ScriptContext m_Context;                    //执行上下文
        public ScorpioScriptFunction(Script script, List<String> listParameters, ScriptExecutable scriptExecutable)
        {
            this.m_script = script;
            this.m_listParameters = new List<string>(listParameters);
            this.m_scriptExecutable = scriptExecutable;
            this.m_Context = new ScriptContext(m_script, m_scriptExecutable, null, Executable_Block.Function);
        }
        public void SetParentContext(ScriptContext context)
        {
            m_parentContext = context;
        }
        public int ParameterCount { get { return m_listParameters.Count; } }
        public ReadOnlyCollection<String> Parameters { get { return m_listParameters.AsReadOnly(); } }
        public ScriptObject Call(VariableDictionary objs, ScriptObject[] parameters)
        {
            int length = parameters.Length;
            for (int i = 0; i < m_listParameters.Count; ++i) {
                objs[m_listParameters[i]] = (parameters != null && length > i) ? parameters[i] : ScriptNull.Instance;
            }
            m_Context.Initialize(m_parentContext, objs);
            return m_Context.Execute();
        }
    }
}
