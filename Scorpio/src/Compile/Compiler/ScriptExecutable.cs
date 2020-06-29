using System.Collections.Generic;
using Scorpio.Instruction;
namespace Scorpio.Compile.Compiler {
    //指令块
    public enum ExecutableBlock {
        Context,    //全局运行块
        Function,   //函数
        Block,      //普通代码块 {}
        If,         //If
        ForBegin,   //for begin
        ForEnd,     //for end
        For,        //for循环
        Foreach,    //foreach
        While,      //while
        Switch,     //switch
    }
    public class ScriptIndexs {
        private int m_VariableIndex = 0;                                                        //当前索引
        private Dictionary<string, int> m_VariableIndexs = new Dictionary<string, int>();       //索引表
        public int AddIndex(string str) {
            if (!m_VariableIndexs.ContainsKey(str)) {
                m_VariableIndexs.Add(str, m_VariableIndex);
                return m_VariableIndex++;
            } else {
                return m_VariableIndexs[str];
            }
        }
        public bool HasIndex(string str) {
            return m_VariableIndexs.ContainsKey(str);
        }
        public int GetIndex(string str) {
            return m_VariableIndexs.ContainsKey(str) ? m_VariableIndexs[str] : -1;
        }
        public int Count { get { return m_VariableIndexs.Count; } }
    }
    //指令执行列表
    public class ScriptExecutable {
        private List<ScriptInstruction> m_listScriptInstructions;                           //指令列表
        private ScriptIndexs m_VariableIndexs;                                              //索引表
        private int m_StackIndex = 0;                                                       //当前层级
        private Dictionary<int, int> m_Stacks = new Dictionary<int, int>();                 //当前层级
        private Dictionary<int, int> m_VariableToInternal = new Dictionary<int, int>();     //临时变量转为内部变量
        private List<int> m_ParentInternal = new List<int>();                               //父级内部变量到本地内部变量的对应关系

        private ScriptIndexs m_InternalIndexs;                                              //内部变量索引

        public ExecutableBlock Block { get { return Blocks.Peek(); } }                      //当前块类型
        public Stack<ExecutableBlock> Blocks { get; private set; }                          //所有块类型
        public int VariableCount { get { return m_VariableIndexs.Count - m_VariableToInternal.Count; } }     //临时变量个数
        public int InternalCount { get { return m_InternalIndexs.Count; } }                 //内部变量个数
        public ScriptExecutable(ExecutableBlock block) {
            m_StackIndex = 0;
            m_Stacks[m_StackIndex] = 0;
            m_VariableIndexs = new ScriptIndexs();
            m_InternalIndexs = new ScriptIndexs();
            m_listScriptInstructions = new List<ScriptInstruction>();
            Blocks = new Stack<ExecutableBlock>();
            Blocks.Push(block);
            AddIndex("this");
        }
        //使用了父级的临时变量
        public int GetInternalIndex(string str) {
            return m_InternalIndexs.GetIndex("this_" + str);
        }
        //添加一个父级临时变量
        public int AddInternalIndex(string str, int parentIndex) {
            int index = m_InternalIndexs.AddIndex("this_" + str);
            m_ParentInternal.Add(parentIndex << 16 | index);
            return index;
        }
        //是否包含临时变量
        public int GetParentInternalIndex(string str) {
            int index = GetIndex(str);
            if (index >= 0) {
                if (m_VariableToInternal.ContainsKey(index)) {
                    return m_VariableToInternal[index];
                } else {
                    m_VariableToInternal[index] = m_InternalIndexs.AddIndex("parent_" + str);
                    return m_VariableToInternal[index];
                }
            }
            return -1;
        }
        public void BeginStack() {
            BeginStack(ExecutableBlock.Block);
        }
        public void BeginStack(ExecutableBlock block) {
            ++m_StackIndex;
            if (m_Stacks.ContainsKey(m_StackIndex)) {
                m_Stacks[m_StackIndex] = m_Stacks[m_StackIndex] + 1;
            } else {
                m_Stacks[m_StackIndex] = 0;
            }
            Blocks.Push(block);
        }
        public void EndStack() {
            --m_StackIndex;
            Blocks.Pop();
        }
        public ScriptInstructionCompiler AddScriptInstruction(Opcode opcode, int opvalue, int line) {
            return AddScriptInstruction(new ScriptInstruction(opcode, opvalue, line));
        }
        //添加一条指令
        ScriptInstructionCompiler AddScriptInstruction(ScriptInstruction instruction) {
            m_listScriptInstructions.Add(instruction);
            return new ScriptInstructionCompiler() { instruction = instruction, index = m_listScriptInstructions.Count - 1};
        }
        public int AddIndex(string str) {
            int count = m_Stacks[m_StackIndex];
            return m_VariableIndexs.AddIndex($"{m_StackIndex}_{count}_{str}");
        }
        public int AddTempIndex() {
            return AddIndex(System.Guid.NewGuid().ToString());
        }
        public bool HasIndex(string str) {
            return GetIndex(str) > -1;
        }
        public int GetIndex(string str) {
            for (var i = m_StackIndex; i >= 0; --i) {
                int count = m_Stacks[i];
                var key = $"{i}_{count}_{str}";
                if (m_VariableIndexs.HasIndex(key)) {
                    return m_VariableIndexs.GetIndex(key);
                }
            }
            return -1;
        }
        public int Count() {
            return m_listScriptInstructions.Count;
        }
        public ScriptInstruction[] ScriptInstructions { get { return m_listScriptInstructions.ToArray(); } }
        public int[] ScriptInternals { get { return m_ParentInternal.ToArray(); } }
        public void Finished() {
            //计算局部变量是否是内部引用变量，并修改为 内部变量赋值 Opcode
            foreach (var instruction in m_listScriptInstructions) {
                if (m_VariableToInternal.TryGetValue(instruction.opvalue, out var internalValue)) {
                    if (instruction.opcode == Opcode.LoadLocal) {
                        instruction.SetOpcode(Opcode.LoadInternal, internalValue);
                    } else if (instruction.opcode == Opcode.StoreLocal) {
                        instruction.SetOpcode(Opcode.StoreInternal, internalValue);
                    } else if (instruction.opcode == Opcode.StoreLocalAssign) {
                        instruction.SetOpcode(Opcode.StoreInternalAssign, internalValue);
                    }
                }
            }
            //重新计算操作局部变量 索引
            foreach (var instruction in m_listScriptInstructions) {
                if (instruction.opcode == Opcode.LoadLocal || instruction.opcode == Opcode.StoreLocal || instruction.opcode == Opcode.StoreLocalAssign) {
                    var count = 0;
                    foreach (var pair in m_VariableToInternal) {
                        if (instruction.opvalue > pair.Key) {
                            ++count;
                        }
                    }
                    if (count > 0) {
                        instruction.opvalue -= count;
                    }
                }
            }
        }
    }
}
