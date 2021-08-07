using System;
using Scorpio.Tools;
namespace Scorpio.Instruction {
    public class SerializeData {
        public string Breviary { get; private set; }
        public double[] ConstDouble { get; private set; }
        public long[] ConstLong { get; private set; }
        public string[] ConstString { get; private set; }
        public ScriptFunctionData Context { get; private set; }
        public ScriptFunctionData[] Functions { get; private set; }     //定义的所有 function
        public ScriptClassData[] Classes { get; private set; }          //定义的所有class
        public SerializeData(string breviary) {
            this.Breviary = string.Intern(breviary);
        }
        public SerializeData(string breviary, double[] constDouble, long[] constLong, string[] constString, ScriptFunctionData context, ScriptFunctionData[] functions, ScriptClassData[] classes) {
            this.Breviary = string.Intern(breviary);
            this.ConstDouble = constDouble;
            this.ConstLong = constLong;
            this.ConstString = constString;
            this.Context = context;
            this.Functions = functions;
            this.Classes = classes;
        }
        public SerializeData Serialize(ScorpioWriter writer) {
            writer.Write(ConstDouble.Length);
            Array.ForEach(ConstDouble, (value) => writer.Write(value));
            writer.Write(ConstLong.Length);
            Array.ForEach(ConstLong, (value) => writer.Write(value));
            writer.Write(ConstString.Length);
            Array.ForEach(ConstString, (value) => writer.Write(value));
            writer.Write(Context);
            writer.Write(Functions.Length);
            Array.ForEach(Functions, (value) => writer.Write(value));
            writer.Write(Classes.Length);
            Array.ForEach(Classes, (value) => writer.Write(value));
            return this;
        }
        public SerializeData Deserialize(ScorpioReader reader) {
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
                ConstString[i] = string.Intern(reader.ReadString());
            }
            Context = reader.ReadFunction();
            Functions = new ScriptFunctionData[reader.ReadInt32()];
            for (var i = 0; i < Functions.Length; ++i) {
                Functions[i] = reader.ReadFunction();
            }
            Classes = new ScriptClassData[reader.ReadInt32()];
            for (var i = 0; i < Classes.Length; ++i) {
                Classes[i] = reader.ReadClass();
            }
            return this;
        }
    }
}
