using Scorpio.Compile.Compiler;
namespace Scorpio.Compile.Exception {
    //词法分析程序
    public class LexerException : System.Exception {
        public LexerException(ScriptLexer lexer, string message) : base($"{lexer.Breviary} Line:{lexer.SourceLine + 1} Column:{lexer.SourceChar} : {message}") { }
    }
}
