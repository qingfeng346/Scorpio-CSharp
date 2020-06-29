using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
namespace Scorpio.Serialize {
    public class Serializer {
        public static SerializeData Serialize(string breviary, string buffer, string[] ignoreFunctions) {
            var lexer = new ScriptLexer(buffer, breviary);
            breviary = lexer.Breviary;
            var parser = new ScriptParser(lexer.GetTokens().ToArray(), breviary, ignoreFunctions);
            var context = parser.Parse();
            return new SerializeData(parser.ConstDouble.ToArray(),
                                        parser.ConstLong.ToArray(),
                                        parser.ConstString.ToArray(),
                                        context,
                                        parser.Functions.ToArray(),
                                        parser.Classes.ToArray());
        }
    }
}
