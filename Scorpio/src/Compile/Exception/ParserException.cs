using Scorpio.Compile.Compiler;
namespace Scorpio.Compile.Exception
{
    //解析语法异常
    public class ParserException : System.Exception {
        public ParserException(string strMessage) : base("解析错误 : " + strMessage) { }
        public ParserException(string strMessage, Token token)
            : base(" Line:" + token.SourceLine + "  Column:" + token.SourceChar + "  Type:" + token.Type + "  value[" + token.Lexeme + "]    " + strMessage) 
        { }
    }
}
