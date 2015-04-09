using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Compiler;
namespace Scorpio
{
    public partial class ScorpioMaker
    {
        public static byte[] Serialize(string file)
        {
            List<Token> tokens = new ScriptLexer(Util.GetFileString(file, Encoding.UTF8)).GetTokens();
            if (tokens.Count == 0) throw new System.Exception("Token数量小于等于0");
            int sourceLine = 0;
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(tokens.Count);
            for (int i = 0; i < tokens.Count; ++i) {
                var token = tokens[i];
                if (sourceLine != token.SourceLine) {
                    sourceLine = token.SourceLine;
                    writer.Write(LineFlag);
                    writer.Write(token.SourceLine);
                }
                writer.Write((sbyte)token.Type);
                switch (token.Type) {
                    case TokenType.Boolean:
                        writer.Write((bool)token.Lexeme ? (byte)1 : (byte)0);
                        break;
                    case TokenType.String:
                        WriteString(writer, (string)token.Lexeme);
                        break;
                    case TokenType.Identifier:
                        WriteString(writer, (string)token.Lexeme);
                        break;
                    case TokenType.Number:
                        if (token.Lexeme is double) {
                            writer.Write((byte)1);
                            writer.Write((double)token.Lexeme);
                        } else {
                            writer.Write((byte)2);
                            writer.Write((long)token.Lexeme);
                        }
                        break;
                }
            }
            byte[] ret = stream.ToArray();
            writer.Close();
            stream.Close();
            return ret;
        }
    }
}
