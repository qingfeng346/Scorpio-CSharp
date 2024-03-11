using System;
using Scorpio.Compile.Compiler;
using Scorpio.Runtime;
using Scorpio.Tools;
namespace Scorpio.Instruction {
    public class SerializeData {
        public string Breviary { get; private set; }
        public int GlobalCacheIndex { get; private set; }
        public GlobalCache GlobalCache { get; private set; }
        public double[] ConstDouble => GlobalCache.ConstDouble;
        public long[] ConstLong => GlobalCache.ConstLong;
        public string[] ConstString => GlobalCache.ConstString;
        [Obsolete("已无用,后续不使用")]
        public byte[] NoContext { get; private set; }                   //兼容老版本,暂不删除
        public ScriptFunctionData Context { get; private set; }
        public ScriptFunctionData[] Functions { get; private set; }     //定义的所有 function
        public ScriptClassData[] Classes { get; private set; }          //定义的所有class
        public SerializeData(string breviary) {
            this.Breviary = string.Intern(breviary);
        }
        public SerializeData(ScriptParser parser) {
            this.Breviary = parser.Breviary;
            this.GlobalCacheIndex = parser.GlobalCache.Index;
            this.GlobalCache = parser.GlobalCache.GlobalCache;
            this.Context = parser.Context;
            this.Functions = parser.Functions.ToArray();
            this.Classes = parser.Classes.ToArray();
            this.NoContext = Array.Empty<byte>();
        }
        public SerializeData Serialize(ScorpioWriter writer, bool writeConst) {
            if (writeConst) {
                GlobalCache.WriteConst(writer);
            }
            writer.WriteFunction(Context);
            writer.Write(Functions.Length);
            Array.ForEach(Functions, writer.WriteFunction);
            writer.Write(Classes.Length);
            Array.ForEach(Classes, writer.WriteClass);
            if (writeConst) {
                writer.WriteNoContext(NoContext);
            }
            return this;
        }
        public SerializeData Deserialize(ScorpioReader reader, GlobalCache globalCache) {
            var readConst = globalCache == null;
            if (readConst) {
                GlobalCache = new GlobalCache().ReadConst(reader);
            } else {
                GlobalCache = globalCache;
            }
            Context = reader.ReadFunction();
            var length = reader.ReadInt32();
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
            if (readConst) {
                NoContext = reader.ReadNoContext();
            }
            return this;
        }
    }
}
