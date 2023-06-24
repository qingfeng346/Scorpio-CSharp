using Scorpio.Instruction;
namespace Scorpio.Compile.Compiler {
    public class ScriptInstructionCompiler {
        public ScriptInstruction instruction { get; set; }
        public int index { get; private set; }
        public ScriptInstructionCompiler(int index, ScriptInstruction instruction) {
            this.instruction = instruction;
            this.index = index;
        }
        public void SetValue(int opvalue) {
            instruction = new ScriptInstruction(instruction.opcode, opvalue, instruction.line);
            //instruction.opvalue = opvalue;
        }
    }
}
