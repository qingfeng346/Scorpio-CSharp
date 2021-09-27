using System.IO;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public class Serializer {
        [System.Obsolete]
        public static SerializeData SerializeV1(string breviary, string buffer, CompileOption compileOption) {
            var datas = Serialize(breviary, buffer, null, compileOption);
            return datas[datas.Length - 1];
        }
        [System.Obsolete]
        public static byte[] SerializeV1Bytes(string breviary, string buffer, CompileOption compileOption) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream)) {
                    writer.Write((byte)0);
                    SerializeV1(breviary, buffer, compileOption).Serialize(writer);
                    return stream.ToArray();
                }
            }
        }
        public static SerializeData[] Serialize(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption) {
            var parsers = new List<ScriptParser>();
            parsers.Add(new ScriptParser(new ScriptLexer(buffer, breviary), searchPaths, compileOption, parsers).Parse());
            var datas = new SerializeData[parsers.Count];
            for (var i = 0; i < datas.Length; ++i) {
                var parser = parsers[i];
                var constString = parser.ConstString.ConvertAll(_ => string.Intern(_)).ToArray();
                datas[i] = new SerializeData(parser.Breviary,
                        parser.ConstDouble.ToArray(),
                        parser.ConstLong.ToArray(),
                        constString,
                        parser.Context,
                        parser.Functions.ToArray(),
                        parser.Classes.ToArray());
            }
            return datas;
        }
        public static byte[] SerializeBytes(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream)) {
                    writer.Write((byte)0);      //占位符
                    writer.Write(int.MaxValue); //占位符
                    writer.Write((short)2);     //版本号
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
