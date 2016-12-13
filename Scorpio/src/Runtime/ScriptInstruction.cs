using Scorpio.CodeDom;
namespace Scorpio.Runtime
{
    //一条指令
    public class ScriptInstruction
    {
        public ScriptInstruction(Opcode opcode, CodeObject operand0) : this(opcode, operand0, null) { }
        public ScriptInstruction(Opcode opcode, CodeObject operand0, CodeObject operand1)
        {
            Opcode = opcode;
            Operand0 = operand0;
            Operand1 = operand1;
        }
        public ScriptInstruction(Opcode opcode, string value)
        {
            Opcode = opcode;
            Value = value;
        }
        public Opcode Opcode;          //指令类型
        public CodeObject Operand0;    //指令值1
        public CodeObject Operand1;    //指令值2
        public string Value;           //指令值
    }
}
