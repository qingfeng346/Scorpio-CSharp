using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Compiler;
namespace Scorpio
{
    public partial class ScorpioMaker
    {
        public static string DeserializeToString(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            StringBuilder builder = new StringBuilder();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i) {
                sbyte flag = reader.ReadSByte();
                if (flag == LineFlag) {
                    int line = reader.ReadInt32();
                    flag = reader.ReadSByte();
                    int sourceLine = builder.ToString().Split('\n').Length;
                    for (int j = sourceLine; j < line; ++j) {
                        builder.Append('\n');
                    }
                }
                TokenType type = (TokenType)flag;
                object value = null;
                switch (type)
                {
                    case TokenType.Boolean:
                        value = (reader.ReadByte() == 1);
                        break;
                    case TokenType.String:
                        value = "\"" + ReadString(reader).Replace("\n", "\\n") + "\"";
                        break;
                    case TokenType.SimpleString:
                        value = "@\"" + ReadString(reader) + "\"";
                        break;
                    case TokenType.Identifier:
                        value = ReadString(reader);
                        break;
                    case TokenType.Number:
                        if (reader.ReadByte() == 1) {
                            value = reader.ReadDouble();
                        } else {
                            value = reader.ReadInt64() + "L";
                        }
                        break;
                    default:
                        value = GetTokenString(type);
                        break;
                }
                builder.Append(value + " ");
            }
            return builder.ToString();
        }
        public static string GetTokenString(TokenType type)
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
