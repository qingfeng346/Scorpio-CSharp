using Scorpio.Compiler;
namespace Scorpio.Serialize {
    public class SerializeUtil {
        public static SerializeData Serialize(string breviary, string buffer) {
            var lexer = new ScriptLexer(buffer, breviary);
            breviary = lexer.Breviary;
            var parser = new ScriptParser(lexer.GetTokens(), breviary);
            var context = parser.Parse();
            return new SerializeData(parser.ConstDouble.ToArray(), 
                                        parser.ConstLong.ToArray(), 
                                        parser.ConstString.ToArray(), 
                                        context, 
                                        parser.Functions.ToArray(), 
                                        parser.Classes.ToArray());
        }
        public static SerializeData Deserialize(byte[] data) {
            return new SerializeData().Parser(data);
        }
    }
}
