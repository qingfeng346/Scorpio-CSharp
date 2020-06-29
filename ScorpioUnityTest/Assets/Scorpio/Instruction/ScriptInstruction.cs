namespace Scorpio.Instruction {
    //不能使用 struct 编译时会稍后修改内部值 
    //单条指令
    public class ScriptInstruction {
        public OpcodeType optype;   //指令类型
        public Opcode opcode;       //指令类型
        public int opvalue;         //指令值
        public int line;            //代码在多少行
        public ScriptInstruction(int opcode, int opvalue, int line) : this((Opcode)opcode, opvalue, line) { }
        public ScriptInstruction(Opcode opcode, int opvalue, int line) {
            this.optype = OpcodeType.None;
            this.opcode = opcode;
            this.opvalue = opvalue;
            this.line = line;
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode) {
            SetOpcode(opcode, opvalue);
        }
        public void SetOpcode(Opcode opcode, int opvalue) {
            this.opcode = opcode;
            this.opvalue = opvalue;
            if (opcode > Opcode.LoadBegin && opcode < Opcode.LoadEnd) {
                this.optype = OpcodeType.Load;
            } else if (opcode > Opcode.NewBegin && opcode < Opcode.NewEnd) {
                this.optype = OpcodeType.New;
            } else if (opcode > Opcode.StoreBegin && opcode < Opcode.StoreEnd) {
                this.optype = OpcodeType.Store;
            } else if (opcode > Opcode.ComputeBegin && opcode < Opcode.ComputeEnd) {
                this.optype = OpcodeType.Compute;
            } else if (opcode > Opcode.CompareBegin && opcode < Opcode.CompareEnd) {
                this.optype = OpcodeType.Compare;
            } else if (opcode > Opcode.JumpBegin && opcode < Opcode.JumpEnd) {
                this.optype = OpcodeType.Jump;
            }
        }
    }
}
