using System;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
namespace Scorpio.Instruction {
    public class SerializeData {
        public string Breviary { get; private set; }
        public double[] ConstDouble { get; private set; }
        public long[] ConstLong { get; private set; }
        public string[] ConstString { get; private set; }
        public byte[] NoContext { get; private set; }
        public ScriptFunctionData Context { get; private set; }
        public ScriptFunctionData[] Functions { get; private set; }     //定义的所有 function
        public ScriptClassData[] Classes { get; private set; }          //定义的所有class
        public SerializeData(string breviary) {
            this.Breviary = string.Intern(breviary);
        }
        public SerializeData(ScriptParser parser) {
            this.Breviary = parser.Breviary;
            this.ConstDouble = parser.ConstDouble.ToArray();
            this.ConstLong = parser.ConstLong.ToArray();
            this.ConstString = parser.ConstString.ToArray();
            this.Context = parser.Context;
            this.Functions = parser.Functions.ToArray();
            this.Classes = parser.Classes.ToArray();
            this.NoContext = parser.GetNoContext();
        }
        public SerializeData Serialize(ScorpioWriter writer) {
            writer.Write(ConstDouble.Length);
            Array.ForEach(ConstDouble, writer.Write);
            writer.Write(ConstLong.Length);
            Array.ForEach(ConstLong, writer.Write);
            writer.Write(ConstString.Length);
            Array.ForEach(ConstString, writer.Write);
            writer.WriteFunction(Context);
            writer.Write(Functions.Length);
            Array.ForEach(Functions, writer.WriteFunction);
            writer.Write(Classes.Length);
            Array.ForEach(Classes, writer.WriteClass);
            writer.WriteNoContext(NoContext);
            return this;
        }
        public SerializeData Deserialize(ScorpioReader reader) {
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

            Context = reader.ReadFunction();
            length = reader.ReadInt32();
            var functions = new ScriptFunctionData[length];
            for (var i = 0; i < length; ++i) {
                functions[i] = reader.ReadFunction();
            }
            Functions = functions;

            length = reader.ReadInt32();
            var classes = new ScriptClassData[length];
            for (var i = 0; i < length; ++i) {
                classes[i] = reader.ReadClass();
            }
            Classes = classes;
            NoContext = reader.ReadNoContext();
            return this;
        }
    }
}
