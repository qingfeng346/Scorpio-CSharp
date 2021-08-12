using System.IO;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public class Serializer {
        [System.Obsolete]
        public static SerializeData SerializeV1(string breviary, string buffer, string[] ignoreFunctions, string[] defines) {
            var datas = Serialize(breviary, buffer, ignoreFunctions, defines, null);
            return datas[datas.Length - 1];
        }
        [System.Obsolete]
        public static byte[] SerializeV1Bytes(string breviary, string buffer, string[] ignoreFunctions, string[] defines) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream)) {
                    writer.Write((byte)0);
                    SerializeV1(breviary, buffer, ignoreFunctions, defines).Serialize(writer);
                    return stream.ToArray();
                }
            }
        }
        public static SerializeData[] Serialize(string breviary, string buffer, string[] ignoreFunctions, string[] defines, string[] searchPaths) {
            var parsers = new List<ScriptParser>();
            parsers.Add(new ScriptParser(new ScriptLexer(buffer, breviary), ignoreFunctions, defines, searchPaths, parsers).Parse());
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
        public static byte[] SerializeBytes(string breviary, string buffer, string[] ignoreFunctions, string[] defines, string[] searchPaths) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream)) {
                    writer.Write((byte)0);      //ռλ��
                    writer.Write(int.MaxValue); //ռλ��
                    writer.Write((short)2);     //�汾��
                    var datas = Serialize(breviary, buffer, ignoreFunctions, defines, searchPaths);
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
