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
        private static sbyte LineFlag = sbyte.MaxValue;
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
            if (tokens.Count == 0) throw new Exception("Token数量小于等于0");
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
            int sourceLine = 0;
            for (int i = 0; i < count;++i )
            {
                sbyte flag = reader.ReadSByte();
                if (flag == LineFlag)
                {
                    sourceLine = reader.ReadInt32();
                    flag = reader.ReadSByte();
                }
                TokenType type = (TokenType)flag;
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
                tokens.Add(new Token(type, value, sourceLine, 0));
            }
            return tokens;
        }
        static void Main(string[] args)
        {
            string source = "";
            string target = "";
             try {
                for (int i = 0; i < args.Length; ++i) {
                    if (args[i] == "-s") {
                        source = args[i + 1];
                    } else if (args[i] == "-o") {
                        target = args[i + 1];
                    }
                }
            } catch (System.Exception ex) {
                Console.WriteLine("参数出错 -s [源文件] -o [输出文件] error : " + ex.ToString());
                goto exit;
            }
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) {
                 Console.WriteLine("参数出错 -s [源文件] -o [输出文件] ");
                 goto exit;
            }
            source = Path.Combine(Environment.CurrentDirectory, source);
            target = Path.Combine(Environment.CurrentDirectory, target);
            try {
                ScriptLexer lexer = new ScriptLexer(File.ReadAllText(source));
                File.WriteAllBytes(target, Serialize(lexer.GetTokens()));
            } catch (System.Exception ex) {
                Console.WriteLine("解析出错 error : " + ex.ToString());	
            }
exit:
            Console.WriteLine("生成完成，请按任意键继续");
            Console.ReadKey();
        }
    }
}
