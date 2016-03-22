using Scorpio;
using System.Collections.Generic;
namespace Scorpio.Runtime {
    //指令执行列表
    public class ScriptExecutable
    {
        private List<ScriptInstruction> m_listScriptInstructions;       //指令列表
        private int m_count;                                            //指令数量
        private ScriptInstruction[] m_arrayScriptInstructions;          //指令列表
        public ScriptExecutable(Script script, Executable_Block block)
        {
            Script = script;
            Block = block;
            m_listScriptInstructions = new List<ScriptInstruction>();
        }
        public Executable_Block Block { get; private set; }             //模块类型
        public Script Script { get; private set; }                      //所在脚本
        //添加一条指令
        public void AddScriptInstruction(ScriptInstruction val) { 
            m_listScriptInstructions.Add(val); 
        }
        //指令添加完成
        public void EndScriptInstruction() {
            m_count = m_listScriptInstructions.Count;
            m_arrayScriptInstructions = m_listScriptInstructions.ToArray();
        }
        //指令数量
        public int Count { get { return m_count; } }
        //获得一条指令
        public ScriptInstruction this[int index] { get { return m_arrayScriptInstructions[index]; } }
    }
}
