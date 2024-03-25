using System;
using System.IO;
using Scorpio.Instruction;
namespace Scorpio.Tools {
    using static ScorpioUtil;
    public class ScorpioWriter : BinaryWriter {
        private short version;
#if NETSTANDARD
        public ScorpioWriter(Stream stream, short version) : base(stream, Script.Encoding, true) {
#else
        public ScorpioWriter(Stream stream, short version) : base(stream, Script.Encoding) {
#endif
            this.version = version;
        }
        public override void Write(string value) {
            if (string.IsNullOrEmpty(value)) {
                Write(0);
            } else {
                var bytes = Script.Encoding.GetBytes(value);
                Write(bytes.Length);
                Write(bytes);
            }
        }
        public void WriteFunction(ScriptFunctionData data) {
            if (version >= VersionSize) {
                Write((ushort)data.parameterCount);
                Write(data.param ? (byte)1 : (byte)0);
                Write((ushort)data.variableCount);
                Write((ushort)data.internalCount);
                Write(data.internals.Length);
                Array.ForEach(data.internals, Write);
                Write(data.scriptInstructions.Length);
                int line = 0;
                Array.ForEach(data.scriptInstructions, (value) => {
                    if (value.line > 0 && line != value.line) {
                        line = value.line;
                        Write(-line);
                    }
                    Write((int)value.opvalue);
                    Write((byte)value.opcode);
                });
            } else {
                Write(data.parameterCount);
                Write(data.param ? (byte)1 : (byte)0);
                Write(data.variableCount);
                Write(data.internalCount);
                Write(data.internals.Length);
                Array.ForEach(data.internals, Write);
                Write(data.scriptInstructions.Length);
                Array.ForEach(data.scriptInstructions, (value) => {
                    Write((int)value.opcode);
                    Write((int)value.opvalue);
                    Write((int)value.line);
                });
            }
        }
        public void WriteClass(ScriptClassData data) {
            Write(data.name);
            Write(data.parent);
            Write(data.functions.Length);
            Array.ForEach(data.functions, Write);
        }
        public void WriteNoContext(byte[] noContext) {
            if (version >= VersionNoContext) {
                Write(noContext.Length);
                Array.ForEach(noContext, Write);
            }
        }
    }
}
