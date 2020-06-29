using Scorpio.Instruction;
namespace Scorpio.Compile.Compiler {
    public class ScriptInstructionCompiler {
        public ScriptInstruction instruction;
        public int index;
        public void SetValue(int opvalue) {
            instruction.opvalue = opvalue;
        }
    }
}
