using System.Collections.Generic;
namespace Scorpio.Compile.Compiler {
    public static class CompilerUtil {
        public static void SetValue(this List<ScriptInstructionCompiler> instructionCompilers, int value) {
            foreach (var instructionCompiler in instructionCompilers) {
                instructionCompiler.SetValue(value);
            }
        }
        public static void SetValue(this List<ScriptInstructionCompiler> instructionCompilers, ScriptInstructionCompiler value) {
            foreach (var instructionCompiler in instructionCompilers) {
                instructionCompiler.SetValue(value.index);
            }
        }
    }
}
