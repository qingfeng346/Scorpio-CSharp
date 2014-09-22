using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Scorpio.Runtime;
using Scorpio.Collections;
namespace Scorpio.Variable
{
    public class ScorpioScriptFunction
    {
        private Script m_script;                                                        //脚本系统
        private List<String> m_listParameters;                                          //参数
        private ScriptExecutable m_scriptExecutable;                                    //函数执行命令
        private VariableDictionary m_scriptStackObject = new VariableDictionary();      //函数参数
        private ScriptContext m_parentContext;                                          //父级上下文
        private ScriptTable m_scriptTable;                                              //是否是table内部函数(如果不为null则为内部函数)
        public ScorpioScriptFunction(Script script, List<String> listParameters, ScriptExecutable scriptExecutable)
        {
            this.m_script = script;
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
        public int ParameterCount { get { return m_listParameters.Count; } }
        public ReadOnlyCollection<String> Parameters { get { return m_listParameters.AsReadOnly(); } }
        public ScriptObject Call(object[] parameters)
        {
            int length = parameters.Length;
            for (int i = 0; i < m_listParameters.Count; ++i) {
                m_scriptStackObject[m_listParameters[i]] = (parameters != null && length > i) ? ScriptObject.CreateObject(parameters[i]) : ScriptNull.Instance;
            }
            ScriptContext context = new ScriptContext(m_script, m_scriptExecutable, m_parentContext, Executable_Block.Function);
            context.Initialize(m_scriptStackObject);
            return context.Execute();
        }
    }
}
