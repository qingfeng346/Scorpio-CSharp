using Scorpio.Compile.Compiler;
namespace Scorpio.Compile.Exception {
    //解析语法异常
    public class ParserException : System.Exception {
        public ParserException(ScriptParser parser, string message) : this(parser, message, parser.PeekToken()) { }
        public ParserException(ScriptParser parser, string message, Token token)
            : base(string.Format("{0} Line:{1} Column:{2} Token:{3} : {4}", parser.Breviary, token.SourceLine, token.SourceChar, token, message)) { }
    }
}
