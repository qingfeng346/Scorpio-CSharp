using System;
using System.IO;
using System.Text;
using Scorpio.Instruction;
namespace Scorpio.Tools {
    public class ScorpioReader : BinaryReader {
#if NETSTANDARD
        public ScorpioReader(Stream stream) : base(stream, Script.Encoding, true) { }
#else
        public ScorpioReader(Stream stream) : base(stream, Script.Encoding) { }
#endif
        //private Stream m_stream;
        //private byte[] m_buffer;
        //public ScorpioReader(Stream input){
        //    m_stream = input;
        //    m_buffer = new byte[8];
        //}
        //public void Dispose() {
        //    m_stream = null;
        //    m_buffer = null;
        //}
        //protected virtual void FillBuffer(int numBytes) {
        //    if (numBytes == 1) {
        //        m_buffer[0] = (byte)m_stream.ReadByte();
        //        return;
        //    }
        //    int bytesRead = 0;
        //    do {
        //        bytesRead += m_stream.Read(m_buffer, bytesRead, numBytes - bytesRead);
        //    } while (bytesRead < numBytes);
        //}
        //public virtual byte[] ReadBytes(int count) {
        //    byte[] result = new byte[count];
        //    int numRead = 0;
        //    do {
        //        int n = m_stream.Read(result, numRead, count);
        //        if (n == 0)
        //            break;
        //        numRead += n;
        //        count -= n;
        //    } while (count > 0);
        //    return result;
        //}
        //public byte ReadByte() {
        //    return (byte)m_stream.ReadByte();
        //}
        //public short ReadInt16() {
        //    FillBuffer(2);
        //    return (short)(m_buffer[0] | m_buffer[1] << 8);
        //}
        //public int ReadInt32() {
        //    FillBuffer(4);
        //    return m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24;
        //}
        //public virtual long ReadInt64() {
        //    FillBuffer(8);
        //    uint lo = (uint)(m_buffer[0] | m_buffer[1] << 8 |
        //                     m_buffer[2] << 16 | m_buffer[3] << 24);
        //    uint hi = (uint)(m_buffer[4] | m_buffer[5] << 8 |
        //                     m_buffer[6] << 16 | m_buffer[7] << 24);
        //    return (long)((ulong)hi) << 32 | lo;
        //}
        //public double ReadDouble() {
        //    FillBuffer(8);
        //    uint lo = (uint)(m_buffer[0] | m_buffer[1] << 8 |
        //        m_buffer[2] << 16 | m_buffer[3] << 24);
        //    uint hi = (uint)(m_buffer[4] | m_buffer[5] << 8 |
        //        m_buffer[6] << 16 | m_buffer[7] << 24);

        //    ulong tmpBuffer = ((ulong)hi) << 32 | lo;
        //    //return *((double*)&tmpBuffer);
        //    return (double)tmpBuffer;
        //}
        public override string ReadString() {
            var length = ReadInt32();
            if (length == 0) { return ""; }
            return Script.Encoding.GetString(ReadBytes(length));
        }
        public ScriptFunctionData ReadFunction() {
            var parameterCount = ReadInt32();
            var param = ReadByte() == 1;
            var variableCount = ReadInt32();
            var internalCount = ReadInt32();
            var internals = new int[ReadInt32()];
            for (var i = 0; i < internals.Length; ++i) {
                internals[i] = ReadInt32();
            }
            var instructions = new ScriptInstruction[ReadInt32()];
            for (var i = 0; i < instructions.Length; ++i) {
                instructions[i] = new ScriptInstruction(ReadInt32(), ReadInt32(), ReadInt32());
            }
            return new ScriptFunctionData() {
                parameterCount = parameterCount,
                param = param,
                variableCount = variableCount,
                internalCount = internalCount,
                internals = internals,
                scriptInstructions = instructions
            };
        }
        public ScriptClassData ReadClass() {
            var name = ReadInt32();
            var parent = ReadInt32();
            var functions = new long[ReadInt32()];
            for (var i = 0; i < functions.Length; ++i) {
                functions[i] = ReadInt64();
            }
            return new ScriptClassData() {
                name = name,
                parent = parent,
                functions = functions
            };
        }
    }
}
