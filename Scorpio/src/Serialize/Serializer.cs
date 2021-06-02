using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
namespace Scorpio.Serialize {
    public class Serializer {
        public static SerializeData Serialize(string breviary, string buffer, string[] ignoreFunctions, string[] defines) {
            var lexer = new ScriptLexer(buffer, breviary);
            breviary = lexer.Breviary;
            var parser = new ScriptParser(lexer.GetTokens().ToArray(), breviary, ignoreFunctions, defines);
            var context = parser.Parse();
            var constString = new string[parser.ConstString.Count];
            for (var i = 0; i < parser.ConstString.Count; ++i) {
                constString[i] = string.Intern(parser.ConstString[i]);
            }
            return new SerializeData(parser.ConstDouble.ToArray(),
                                        parser.ConstLong.ToArray(),
                                        constString,
                                        context,
                                        parser.Functions.ToArray(),
                                        parser.Classes.ToArray());
        }
    }
}
