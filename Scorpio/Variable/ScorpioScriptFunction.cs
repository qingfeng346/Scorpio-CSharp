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
        private int m_ParameterCount;                       //参数个数
        private ScriptArray m_ParamsArray;                  //不定参数组
        private bool m_Params;                              //是否是不定参函数
        public ScorpioScriptFunction(Script script, List<String> listParameters, ScriptExecutable scriptExecutable, bool bParams)
        {
            this.m_script = script;
            this.m_listParameters = new List<string>(listParameters);
            this.m_scriptExecutable = scriptExecutable;
            this.m_ParameterCount = listParameters.Count;
            this.m_Params = bParams;
            this.m_ParamsArray = bParams ? new ScriptArray() : null;
            this.m_Context = new ScriptContext(m_script, m_scriptExecutable, null, Executable_Block.Function);
        }
        public void SetParentContext(ScriptContext context)
        {
            m_parentContext = context;
        }
        public bool Params { get { return m_Params; } }
        public int ParameterCount { get { return m_ParameterCount; } }
        public ReadOnlyCollection<String> Parameters { get { return m_listParameters.AsReadOnly(); } }
        public ScriptObject Call(VariableDictionary objs, ScriptObject[] parameters)
        {
            int length = parameters.Length;
            if (m_Params) {
                m_ParamsArray.Clear();
                for (int i = 0; i < m_ParameterCount - 1; ++i) {
                    objs[m_listParameters[i]] = (parameters != null && length > i) ? parameters[i] : ScriptNull.Instance;
                }
                for (int i = m_ParameterCount - 1; i < length; ++i) {
                    m_ParamsArray.Add(parameters[i]);
                }
                objs[m_listParameters[m_ParameterCount - 1]] = m_ParamsArray;
            } else {
                for (int i = 0; i < m_ParameterCount; ++i) {
                    objs[m_listParameters[i]] = (parameters != null && length > i) ? parameters[i] : ScriptNull.Instance;
                }
            }
            m_Context.Initialize(m_parentContext, objs);
            return m_Context.Execute();
        }
    }
}
