using Scorpio.Instruction;
using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    public class ScriptInstructionCompiler {
        private List<ScriptInstruction> Instructions;
        public int index { get; private set; }
        public ScriptInstructionCompiler(List<ScriptInstruction> instructions) {
            this.Instructions = instructions;
            this.index = instructions.Count - 1;
        }
        public void SetValue(int opvalue) {
            var instruction = Instructions[index];
            Instructions[index] = new ScriptInstruction(instruction.opcode, opvalue, instruction.line);
        }
    }
}
