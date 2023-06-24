namespace Scorpio.Instruction {
    //单条指令
    public struct ScriptInstruction {
        public int opvalue;         //指令值
        public ushort line;         //代码在多少行
        public Opcode opcode;       //指令类型
        public ScriptInstruction(int opcode, int opvalue, int line) : this((Opcode)opcode, opvalue, line) { }
        public ScriptInstruction(Opcode opcode, int opvalue, int line) {
            this.opcode = opcode;
            this.opvalue = opvalue;
            this.line = (ushort)line;
        }
        public void Set(Opcode opcode) {
            this.opcode = opcode;
        }
        public void Set(Opcode opcode, int opvalue) {
            this.opcode = opcode;
            this.opvalue = opvalue;
        }
        public override string ToString() {
            return $"{opcode} {opvalue} - {line}";
        }
    }
}
