using System.Runtime.InteropServices;

namespace Scorpio.Instruction {
    //不能使用 struct 编译时会稍后修改内部值 
    //单条指令
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ScriptInstruction {
        public int opvalue;         //指令值
        public ushort line;         //代码在多少行
        public OpcodeType optype;   //指令类型
        public Opcode opcode;       //指令类型
        public ScriptInstruction(byte opcode, int opvalue, ushort line) : this((Opcode)opcode, opvalue, line) { }
        public ScriptInstruction(Opcode opcode, int opvalue, ushort line) {
            this.optype = OpcodeType.Nop;
            this.opcode = opcode;
            this.opvalue = opvalue;
            this.line = line;
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode) {
            SetOpcode(opcode, opvalue);
        }
        public ScriptInstruction SetOpcode(Opcode opcode, int opvalue) {
            this.opcode = opcode;
            this.opvalue = opvalue;
            if (opcode == Opcode.Nop) {
                this.optype = OpcodeType.Nop;
            } else if (opcode > Opcode.LoadBegin && opcode < Opcode.NewBegin) {
                this.optype = OpcodeType.Load;
            } else if (opcode > Opcode.NewBegin && opcode < Opcode.StoreBegin) {
                this.optype = OpcodeType.New;
            } else if (opcode > Opcode.StoreBegin && opcode < Opcode.ComputeBegin) {
                this.optype = OpcodeType.Store;
            } else if (opcode > Opcode.ComputeBegin && opcode < Opcode.CompareBegin) {
                this.optype = OpcodeType.Compute;
            } else if (opcode > Opcode.CompareBegin && opcode < Opcode.JumpBegin) {
                this.optype = OpcodeType.Compare;
            } else if (opcode > Opcode.JumpBegin) {
                this.optype = OpcodeType.Jump;
            }
            return this;
        }
        public override string ToString() {
            return $"{opcode} - {opvalue}";
        }
    }
}
