using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Compiler;
namespace Scorpio.Serialize
{
    public class ScorpioMaker
    {
        private static sbyte LineFlag = sbyte.MaxValue;
		public static byte[] Serialize(String breviary, string data)
        {
			List<Token> tokens = new ScriptLexer(data, breviary).GetTokens();
            if (tokens.Count == 0) return new byte[0];
            int sourceLine = 0;
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((sbyte)0);          //第一个字符写入一个null 以此判断文件是二进制文件还是字符串文件
            writer.Write(tokens.Count);
            for (int i = 0; i < tokens.Count; ++i)
            {
                var token = tokens[i];
                if (sourceLine != token.SourceLine)
                {
                    sourceLine = token.SourceLine;
                    writer.Write(LineFlag);
                    writer.Write(token.SourceLine);
                }
                writer.Write((sbyte)token.Type);
                switch (token.Type)
                {
                    case TokenType.Boolean:
                        writer.Write((bool)token.Lexeme ? (sbyte)1 : (sbyte)0);
                        break;
                    case TokenType.String:
                    case TokenType.SimpleString:
                        Util.WriteString(writer, (string)token.Lexeme);
                        break;
                    case TokenType.Identifier:
                        Util.WriteString(writer, (string)token.Lexeme);
                        break;
                    case TokenType.Number:
                        if (token.Lexeme is double)
                        {
                            writer.Write((sbyte)1);
                            writer.Write((double)token.Lexeme);
                        }
                        else
                        {
                            writer.Write((sbyte)2);
                            writer.Write((long)token.Lexeme);
                        }
                        break;
                }
            }
            byte[] ret = stream.ToArray();
            stream.Dispose();
#if SCORPIO_UWP && !UNITY_EDITOR
            writer.Dispose();
#else
            writer.Close();
#endif
            return ret;
        }
        public static List<Token> Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            reader.ReadSByte();      //取出第一个null字符
            List<Token> tokens = new List<Token>();
            int count = reader.ReadInt32();
            int sourceLine = 0;
            for (int i = 0; i < count; ++i)
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
                        value = (reader.ReadSByte() == 1);
                        break;
                    case TokenType.String:
                    case TokenType.SimpleString:
                        value = Util.ReadString(reader);
                        break;
                    case TokenType.Identifier:
                        value = Util.ReadString(reader);
                        break;
                    case TokenType.Number:
                        if (reader.ReadSByte() == 1)
                            value = reader.ReadDouble();
                        else
                            value = reader.ReadInt64();
                        break;
                    default:
                        value = type.ToString();
                        break;
                }
                tokens.Add(new Token(type, value, sourceLine - 1, 0));
            }
            return tokens;
        }
        public static string DeserializeToString(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            reader.ReadSByte();      //取出第一个null字符
            StringBuilder builder = new StringBuilder();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                sbyte flag = reader.ReadSByte();
                if (flag == LineFlag)
                {
                    int line = reader.ReadInt32();
                    flag = reader.ReadSByte();
                    int sourceLine = builder.ToString().Split('\n').Length;
                    for (int j = sourceLine; j < line; ++j)
                        builder.Append('\n');
                }
                TokenType type = (TokenType)flag;
                object value = null;
                switch (type)
                {
                    case TokenType.Boolean:
                        value = (reader.ReadSByte() == 1);
                        break;
                    case TokenType.String:
                        value = "\"" + Util.ReadString(reader).Replace("\n", "\\n") + "\"";
                        break;
                    case TokenType.SimpleString:
                        value = "@\"" + Util.ReadString(reader) + "\"";
                        break;
                    case TokenType.Identifier:
                        value = Util.ReadString(reader);
                        break;
                    case TokenType.Number:
                        if (reader.ReadSByte() == 1)
                            value = reader.ReadDouble();
                        else
                            value = reader.ReadInt64() + "L";
                        break;
                    default:
                        value = GetTokenString(type);
                        break;
                }
                builder.Append(value + " ");
            }
            return builder.ToString();
        }
        private static string GetTokenString(TokenType type)
        {
            switch (type)
            {
                case TokenType.Var: return "var";

                case TokenType.LeftBrace: return "{";
                case TokenType.RightBrace: return "}";
                case TokenType.LeftBracket: return "[";
                case TokenType.RightBracket: return "]";
                case TokenType.LeftPar: return "(";
                case TokenType.RightPar: return ")";

                case TokenType.Period: return ".";
                case TokenType.Comma: return ",";
                case TokenType.Colon: return ":";
                case TokenType.SemiColon: return ";";
                case TokenType.QuestionMark: return "?";

                case TokenType.Plus: return "+";
                case TokenType.Increment: return "++";
                case TokenType.AssignPlus: return "+=";
                case TokenType.Minus: return "-";
                case TokenType.Decrement: return "--";
                case TokenType.AssignMinus: return "-=";
                case TokenType.Multiply: return "*";
                case TokenType.AssignMultiply: return "*=";
                case TokenType.Divide: return "/";
                case TokenType.AssignDivide: return "/=";
                case TokenType.Modulo: return "%";
                case TokenType.AssignModulo: return "%=";
                case TokenType.InclusiveOr: return "|";
                case TokenType.AssignInclusiveOr: return "|=";
                case TokenType.Or: return "||";
                case TokenType.Combine: return "&";
                case TokenType.AssignCombine: return "&=";
                case TokenType.And: return "&&";
                case TokenType.XOR: return "^";
                case TokenType.AssignXOR: return "^=";
                case TokenType.Shi: return "<<";
                case TokenType.AssignShi: return "<<=";
                case TokenType.Shr: return ">>";
                case TokenType.AssignShr: return ">>=";
                case TokenType.Not: return "!";
                case TokenType.Assign: return "=";
                case TokenType.Equal: return "==";
                case TokenType.NotEqual: return "!=";
                case TokenType.Greater: return ">";
                case TokenType.GreaterOrEqual: return ">=";
                case TokenType.Less: return "<";
                case TokenType.LessOrEqual: return "<=";

                case TokenType.Params: return "...";
                case TokenType.If: return "if";
                case TokenType.Else: return "else";
                case TokenType.ElseIf: return "elif";
                case TokenType.For: return "for";
                case TokenType.Foreach: return "foreach";
                case TokenType.In: return "in";
                case TokenType.Switch: return "switch";
                case TokenType.Case: return "case";
                case TokenType.Default: return "default";
                case TokenType.Break: return "break";
                case TokenType.Continue: return "continue";
                case TokenType.Return: return "return";
                case TokenType.While: return "while";
                case TokenType.Function: return "function";
                case TokenType.Try: return "try";
                case TokenType.Catch: return "catch";
                case TokenType.Throw: return "throw";
                case TokenType.Null: return "null";
                case TokenType.Eval: return "eval";
            }
            return "";
        }
    }
}
