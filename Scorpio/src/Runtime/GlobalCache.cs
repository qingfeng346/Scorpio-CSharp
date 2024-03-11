using System;
using Scorpio.Tools;
namespace Scorpio.Runtime {
    public class GlobalCache {
        public double[] ConstDouble;
        public long[] ConstLong;
        public string[] ConstString;
        internal void WriteConst(ScorpioWriter writer) {
            writer.Write(ConstDouble.Length);
            Array.ForEach(ConstDouble, writer.Write);
            writer.Write(ConstLong.Length);
            Array.ForEach(ConstLong, writer.Write);
            writer.Write(ConstString.Length);
            Array.ForEach(ConstString, writer.Write);
        }
        internal GlobalCache ReadConst(ScorpioReader reader) {
            var length = reader.ReadInt32();
            var constDouble = new double[length];
            for (var i = 0; i < length; ++i) {
                constDouble[i] = reader.ReadDouble();
            }
            ConstDouble = constDouble;

            length = reader.ReadInt32();
            var constLong = new long[length];
            for (var i = 0; i < length; ++i) {
                constLong[i] = reader.ReadInt64();
            }
            ConstLong = constLong;

            length = reader.ReadInt32();
            var constString = new string[length];
            for (var i = 0; i < length; ++i) {
                constString[i] = reader.ReadString();
            }
            ConstString = constString;
            return this;
        }
    }
}
