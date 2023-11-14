using System;
using System.IO;
using Scorpio.Instruction;
namespace Scorpio.Tools {
    using static ScorpioUtil;
    public class ScorpioReader : BinaryReader {
#if NETSTANDARD
        public ScorpioReader(Stream stream) : base(stream, Script.Encoding, true) {
#else
        public ScorpioReader(Stream stream) : base(stream, Script.Encoding) {
#endif
        }
        public short version;
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
        public byte[] ReadNoContext() {
            if (version >= VersionNoContext) {
                var length = ReadInt32();
                var noContext = new byte[length];
                for (var i = 0; i < length; ++i) {
                    noContext[i] = ReadByte();
                }
                return noContext;
            } else {
                return Array.Empty<byte>();
            }
        }
    }
}
