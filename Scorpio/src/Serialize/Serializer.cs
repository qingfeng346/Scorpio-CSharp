using System.IO;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public class Serializer {
        private const short DefaultVersion = 2;
        public static SerializeData[] Serialize(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption) {
            var parsers = new List<ScriptParser>();
            parsers.Add(new ScriptParser(new ScriptLexer(buffer, breviary), searchPaths, compileOption, parsers).Parse());
            var datas = new SerializeData[parsers.Count];
            for (var i = 0; i < datas.Length; ++i) {
                datas[i] = new SerializeData(parsers[i]);
            }
            return datas;
        }
        public static byte[] SerializeBytes(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption) {
            return SerializeBytes(breviary, buffer, searchPaths, compileOption, DefaultVersion);
        }
        public static byte[] SerializeBytes(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption, short version) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream, version)) {
                    writer.Write((byte)0);      //占位符
                    writer.Write(int.MaxValue); //占位符
                    writer.Write(version);     //版本号
                    var datas = Serialize(breviary, buffer, searchPaths, compileOption);
                    int length = datas.Length;
                    writer.Write(length);
                    for (var i = 0; i < length; ++i) {
                        var data = datas[i];
                        writer.Write(data.Breviary);
                        data.Serialize(writer);
                    }
                    return stream.ToArray();
                }
            }
        }
    }
}
