using System.Runtime.InteropServices;

namespace Scorpio.Instruction {
    //不能使用 struct 编译时会稍后修改内部值 
    //单条指令
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct ScriptInstruction {
        [FieldOffset(0)] public int opvalue;         //指令值
        [FieldOffset(4)] public ushort line;         //代码在多少行
        [FieldOffset(6)] public Opcode opcode;       //指令类型
        public ScriptInstruction(int opcode, int opvalue, int line) : this((Opcode)opcode, opvalue, line) { }
        public ScriptInstruction(Opcode opcode, int opvalue, int line) {
            this.opcode = opcode;
            this.opvalue = opvalue;
            this.line = (ushort)line;
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode) {
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode, int opvalue) {
            this.opcode = opcode;
            this.opvalue = opvalue;
        }
    }
}
