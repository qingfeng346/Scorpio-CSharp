using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
using System.IO;
using System.Reflection;
using System.Diagnostics;

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
        public static string ReadString(BinaryReader reader)
        {
            List<byte> sb = new List<byte>();
            byte ch;
            while ((ch = reader.ReadByte()) != 0)
                sb.Add(ch);
            return Encoding.UTF8.GetString(sb.ToArray());
        }
        public static byte[] Serialize(List<Token> tokens)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(tokens.Count);
            for (int i = 0; i < tokens.Count; ++i) {
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
            byte[] ret = stream.ToArray();
            writer.Close();
            stream.Close();
            return ret;
        }
        public static List<Token> Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            List<Token> tokens = new List<Token>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count;++i )
            {
                TokenType type = (TokenType)reader.ReadByte();
                int sourceLine = reader.ReadInt32();
                int sourceChar = reader.ReadInt32();
                object value = null;
                switch (type)
                {
                    case TokenType.Boolean:
                        value = (reader.ReadByte() == 1);
                        break;
                    case TokenType.String:
                        value = ReadString(reader);
                        break;
                    case TokenType.Number:
                        if (reader.ReadByte() == 1) {
                            value = reader.ReadDouble();
                        } else {
                            value = reader.ReadInt64();
                        }
                        break;
                    case TokenType.Identifier:
                        value = ReadString(reader);
                        break;
                    default:
                        value = type.ToString();
                        break;
                }
                tokens.Add(new Token(type, value, sourceLine, sourceChar));
            }
            return tokens;
        }
        static void Main(string[] args)
        {
            try {
                if (args.Length <= 0) return;
                string str = File.ReadAllText(args[0]);
                ScriptLexer lexer = new ScriptLexer(str);
                File.WriteAllBytes(args[0] + ".ser", Serialize(lexer.GetTokens()));
            } catch (System.Exception ex) {
                Console.WriteLine("error : " + ex.ToString());	
            }
            Console.WriteLine("生成完成，请按任意键继续");
            Console.ReadKey();
        }
    }
}
