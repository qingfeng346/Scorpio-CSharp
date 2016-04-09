using System.Collections.Generic;
namespace Scorpio.Runtime {
    //指令执行列表
    public class ScriptExecutable
    {
        private List<ScriptInstruction> m_listScriptInstructions;       //指令列表
        private ScriptInstruction[] m_arrayScriptInstructions;          //指令列表
        public Executable_Block m_Block;
        public ScriptExecutable(Executable_Block block) {
            m_Block = block;
            m_listScriptInstructions = new List<ScriptInstruction>();
        }
        //添加一条指令
        public void AddScriptInstruction(ScriptInstruction val) { 
            m_listScriptInstructions.Add(val); 
        }
        //指令添加完成
        public void EndScriptInstruction() {
            m_arrayScriptInstructions = m_listScriptInstructions.ToArray();
        }
        public ScriptInstruction[] ScriptInstructions { get { return m_arrayScriptInstructions; } }
    }
}
