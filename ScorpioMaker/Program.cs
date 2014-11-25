using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
using System.IO;
using System.Reflection;

namespace ScorpioMaker
{
    class Program
    {
        public static void WriteString(BinaryWriter writer, string str)
        {
            if (string.IsNullOrEmpty(str)) {
                writer.Write((byte)0);
            } else {
                writer.Write(Encoding.UTF8.GetBytes(str));
                writer.Write((byte)0);
            }
        }
        static void Main(string[] args)
        {
            try {
                if (args.Length <= 0) return;
                string str = File.ReadAllText(args[0]);
                ScriptLexer lexer = new ScriptLexer(str);
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(stream);
                List<Token> tokens = lexer.GetTokens();
                for (int i = 0; i < tokens.Count; ++i)
                {
                    var token = tokens[i];
                    writer.Write((byte)token.Type);
                    writer.Write(token.SourceLine);
                    writer.Write(token.SourceChar);
                    switch (token.Type)
                    {
                        case TokenType.Boolean:
                            writer.Write((bool)token.Lexeme ? (byte)1 : (byte)0);
                            break;
                        case TokenType.String:
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
                        case TokenType.Identifier:
                            WriteString(writer, (string)token.Lexeme);
                            break;
                    }
                }
                File.WriteAllBytes(args[0] + ".sco", stream.ToArray());
                writer.Close();
                stream.Close();
            } catch (System.Exception ex) {
                Console.WriteLine("error : " + ex.ToString());	
            }
            Console.ReadKey();
        }
    }
}
