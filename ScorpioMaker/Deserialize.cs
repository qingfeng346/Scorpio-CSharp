using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio.Compiler;
namespace Scorpio
{
    public partial class ScorpioMaker
    {
        public static List<Token> Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            List<Token> tokens = new List<Token>();
            int count = reader.ReadInt32();
            int sourceLine = 0;
            for (int i = 0; i < count; ++i) {
                sbyte flag = reader.ReadSByte();
                if (flag == LineFlag) {
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
                    case TokenType.SimpleString:
                        value = ReadString(reader);
                        break;
                    case TokenType.Identifier:
                        value = ReadString(reader);
                        break;
                    case TokenType.Number:
                        if (reader.ReadByte() == 1) {
                            value = reader.ReadDouble();
                        } else {
                            value = reader.ReadInt64();
                        }
                        break;
                    default:
                        value = type.ToString();
                        break;
                }
                tokens.Add(new Token(type, value, sourceLine, 0));
            }
            return tokens;
        }
    }
}
