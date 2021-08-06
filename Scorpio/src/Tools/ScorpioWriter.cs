using System;
using System.IO;
using System.Text;
using Scorpio.Instruction;
namespace Scorpio.Tools {
    public class ScorpioWriter : BinaryWriter {
#if NETSTANDARD
        public ScorpioWriter(Stream stream) : base(stream, Encoding.UTF8, true) { }
#else
        public ScorpioWriter(Stream stream) : base(stream) { }
#endif
        //public ScorpioWriter(Stream output) {
        //    m_stream = output;
        //    m_buffer = new byte[8];
        //}
        //private Stream m_stream;
        //private byte[] m_buffer;
        //public ScorpioWriter(Stream output) {
        //    m_stream = output;
        //    m_buffer = new byte[8];
        //}
        //public void Dispose() {
        //    m_stream = null;
        //    m_buffer = null;
        //}
        //public void Write(byte value) {
        //    m_stream.WriteByte(value);
        //}
        //public void Write(byte[] buffer) {
        //    m_stream.Write(buffer, 0, buffer.Length);
        //}
        //public void Write(double value) {
        //    ulong TmpValue = *(ulong*)&value;
        //    _buffer[0] = (byte)TmpValue;
        //    _buffer[1] = (byte)(TmpValue >> 8);
        //    _buffer[2] = (byte)(TmpValue >> 16);
        //    _buffer[3] = (byte)(TmpValue >> 24);
        //    _buffer[4] = (byte)(TmpValue >> 32);
        //    _buffer[5] = (byte)(TmpValue >> 40);
        //    _buffer[6] = (byte)(TmpValue >> 48);
        //    _buffer[7] = (byte)(TmpValue >> 56);
        //    OutStream.Write(_buffer, 0, 8);
        //}
        public override void Write(string value) {
            if (string.IsNullOrEmpty(value)) {
                Write(0);
            } else {
                var bytes = Encoding.UTF8.GetBytes(value);
                Write(bytes.Length);
                Write(bytes);
            }
        }
        public void Write(ScriptFunctionData data) {
            Write(data.parameterCount);
            Write(data.param ? (byte)1 : (byte)0);
            Write(data.variableCount);
            Write(data.internalCount);
            Write(data.internals.Length);
            Array.ForEach(data.internals, (value) => Write(value));
            Write(data.scriptInstructions.Length);
            Array.ForEach(data.scriptInstructions, (value) => {
                Write((int)value.opcode);
                Write(value.opvalue);
                Write(value.line);
            });
        }
        public void Write(ScriptClassData data) {
            Write(data.name);
            Write(data.parent);
            Write(data.functions.Length);
            Array.ForEach(data.functions, (value) => {
                Write(value);
            });
        }
    }
}
