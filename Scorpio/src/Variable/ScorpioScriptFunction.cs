using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Scorpio;
using Scorpio.Runtime;
namespace Scorpio.Variable
{
    /// <summary> 脚本函数 </summary>
    internal class ScorpioScriptFunction
    {
        private Script m_Script;                            //脚本系统
        private String[] m_ListParameters;                  //参数
        private ScriptExecutable m_ScriptExecutable;        //函数执行命令
        private int m_ParameterCount;                       //参数个数
        private bool m_Params;                              //是否是不定参函数
        public int GetParameterCount() { return m_ParameterCount; }
        public bool IsParams() { return m_Params; }
        public ScriptArray GetParameters() {
            ScriptArray ret = m_Script.CreateArray();
            foreach (String par in m_ListParameters) {
                ret.Add(m_Script.CreateString(par));
            }
            return ret;
        }
        public ScorpioScriptFunction(Script script, List<String> listParameters, ScriptExecutable scriptExecutable, bool bParams) {
            this.m_Script = script;
            this.m_ListParameters = listParameters.ToArray();
            this.m_ScriptExecutable = scriptExecutable;
            this.m_ParameterCount = listParameters.Count;
            this.m_Params = bParams;
        }
        public ScriptObject Call(ScriptContext parentContext, Dictionary<String, ScriptObject> objs, ScriptObject[] parameters) {
            int length = parameters.Length;
            if (m_Params) {
                ScriptArray paramsArray = m_Script.CreateArray();
                for (int i = 0; i < m_ParameterCount - 1; ++i) {
                    objs[m_ListParameters[i]] = (parameters != null && length > i) ? parameters[i] : m_Script.Null;
                }
                for (int i = m_ParameterCount - 1; i < length; ++i) {
                    paramsArray.Add(parameters[i]);
                }
                objs[m_ListParameters[m_ParameterCount - 1]] = paramsArray;
            } else {
                for (int i = 0; i < m_ParameterCount; ++i) {
                    objs[m_ListParameters[i]] = (parameters != null && length > i) ? parameters[i] : m_Script.Null;
                }
            }
            ScriptContext context = new ScriptContext(m_Script, m_ScriptExecutable, parentContext, Executable_Block.Function);
            context.Initialize(objs);
            return context.Execute();
        }
    }
}
