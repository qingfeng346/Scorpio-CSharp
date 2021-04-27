using System;
using System.IO;
using System.Text;
namespace Scorpio.Instruction {
    public class SerializeData {
        public double[] ConstDouble { get; private set; }
        public long[] ConstLong { get; private set; }
        public string[] ConstString { get; private set; }
        public ScriptFunctionData Context { get; private set; }
        public ScriptFunctionData[] Functions { get; private set; }     //定义的所有 function
        public ScriptClassData[] Classes { get; private set; }          //定义的所有class
        public SerializeData() { }
        public SerializeData(double[] constDouble, long[] constLong, string[] constString, ScriptFunctionData context, ScriptFunctionData[] functions, ScriptClassData[] classes) {
            this.ConstDouble = constDouble;
            this.ConstLong = constLong;
            this.ConstString = constString;
            this.Context = context;
            this.Functions = functions;
            this.Classes = classes;
        }
        public byte[] ToArray() {
            using (var stream = new MemoryStream()) {
                using (var writer = new BinaryWriter(stream)) {
                    writer.Write((byte)0);
                    writer.Write(ConstDouble.Length);
                    Array.ForEach(ConstDouble, (value) => writer.Write(value));
                    writer.Write(ConstLong.Length);
                    Array.ForEach(ConstLong, (value) => writer.Write(value));
                    writer.Write(ConstString.Length);
                    Array.ForEach(ConstString, (value) => WriteString(writer, value));
                    WriterFunction(writer, Context);
                    writer.Write(Functions.Length);
                    Array.ForEach(Functions, (value) => WriterFunction(writer, value));
                    writer.Write(Classes.Length);
                    Array.ForEach(Classes, (value) => WriteClass(writer, value));
                    return stream.ToArray();
                }
            }
        }
        public SerializeData Parser(byte[] data) {
            using (var stream = new MemoryStream(data)) {
                using (var reader = new BinaryReader(stream)) {
                    reader.ReadByte();
                    ConstDouble = new double[reader.ReadInt32()];
                    for (var i = 0; i < ConstDouble.Length; ++i) {
                        ConstDouble[i] = reader.ReadDouble();
                    }
                    ConstLong = new long[reader.ReadInt32()];
                    for (var i = 0; i < ConstLong.Length; ++i) {
                        ConstLong[i] = reader.ReadInt64();
                    }
                    ConstString = new string[reader.ReadInt32()];
                    for (var i = 0; i < ConstString.Length; ++i) {
                        //string.Intern 减少内存
                        ConstString[i] = string.Intern(ReadString(reader));
                    }
                    Context = ReadFunction(reader);
                    Functions = new ScriptFunctionData[reader.ReadInt32()];
                    for (var i = 0; i < Functions.Length; ++i) {
                        Functions[i] = ReadFunction(reader);
                    }
                    Classes = new ScriptClassData[reader.ReadInt32()];
                    for (var i = 0; i < Classes.Length; ++i) {
                        Classes[i] = ReadClass(reader);
                    }
                    return this;
                }
            }
        }
        void WriteString(BinaryWriter writer, string str) {
            if (string.IsNullOrEmpty(str)) {
                writer.Write(0);
            } else {
                var bytes = Encoding.UTF8.GetBytes(str);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }
        void WriterFunction(BinaryWriter writer, ScriptFunctionData data) {
            writer.Write(data.parameterCount);
            writer.Write(data.param ? (byte)1 : (byte)0);
            writer.Write(data.variableCount);
            writer.Write(data.internalCount);
            writer.Write(data.internals.Length);
            Array.ForEach(data.internals, (value) => writer.Write(value));
            writer.Write(data.scriptInstructions.Length);
            Array.ForEach(data.scriptInstructions, (value) => {
                writer.Write((int)value.opcode);
                writer.Write(value.opvalue);
                writer.Write(value.line);
            });
        }
        void WriteClass(BinaryWriter writer, ScriptClassData data) {
            writer.Write(data.name);
            writer.Write(data.parent);
            writer.Write(data.functions.Length);
            Array.ForEach(data.functions, (value) => {
                writer.Write(value);
            });
        }
        string ReadString(BinaryReader reader) {
            var length = reader.ReadInt32();
            if (length == 0) { return ""; }
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }
        ScriptFunctionData ReadFunction(BinaryReader reader) {
            var parameterCount = reader.ReadInt32();
            var param = reader.ReadByte() == 1;
            var variableCount = reader.ReadInt32();
            var internalCount = reader.ReadInt32();
            var internals = new int[reader.ReadInt32()];
            for (var i = 0; i < internals.Length; ++i) {
                internals[i] = reader.ReadInt32();
            }
            var instructions = new ScriptInstruction[reader.ReadInt32()];
            for (var i = 0; i < instructions.Length; ++i) {
                instructions[i] = new ScriptInstruction(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
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
        ScriptClassData ReadClass(BinaryReader reader) {
            var name = reader.ReadInt32();
            var parent = reader.ReadInt32();
            var functions = new long[reader.ReadInt32()];
            for (var i = 0; i < functions.Length; ++i) {
                functions[i] = reader.ReadInt64();
            }
            return new ScriptClassData() {
                name = name,
                parent = parent,
                functions = functions
            };
        }
    }
}
