using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Scorpio
{
    public partial class ScorpioMaker
    {
        private static sbyte LineFlag = sbyte.MaxValue;
        private static void WriteString(BinaryWriter writer, string str)
        {
            if (string.IsNullOrEmpty(str)) {
                writer.Write((byte)0);
            } else {
                writer.Write(Encoding.UTF8.GetBytes(str));
                writer.Write((byte)0);
            }
        }
        private static string ReadString(BinaryReader reader)
        {
            List<byte> sb = new List<byte>();
            byte ch;
            while ((ch = reader.ReadByte()) != 0)
                sb.Add(ch);
            return Encoding.UTF8.GetString(sb.ToArray());
        }
    }
}
