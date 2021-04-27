using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
namespace Scorpio.Serialize {
    public class Serializer {
        public static SerializeData Serialize(string breviary, string buffer, string[] ignoreFunctions, bool intern = false) {
            var lexer = new ScriptLexer(buffer, breviary);
            breviary = lexer.Breviary;
            var parser = new ScriptParser(lexer.GetTokens().ToArray(), breviary, ignoreFunctions);
            var context = parser.Parse();
            string[] constString;
            if (intern) {
                constString = new string[parser.ConstString.Count];
                for (var i = 0; i < parser.ConstString.Count; ++i) {
                    constString[i] = string.Intern(parser.ConstString[i]);
                }
            } else {
                constString = parser.ConstString.ToArray();
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
