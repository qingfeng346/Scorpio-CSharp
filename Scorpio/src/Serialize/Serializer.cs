using System.IO;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
namespace Scorpio.Serialize
{
    public class Serializer {
        public const short DefaultVersion = 4;
        public static SerializeData[] Serialize(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption, GlobalCacheCompiler globalCache = null, short version = DefaultVersion) {
            var parsers = new List<ScriptParser>();
            parsers.Add(new ScriptParser(buffer.ParseTokens(), breviary, breviary, searchPaths, compileOption, parsers, globalCache, version).Parse());
            var datas = new SerializeData[parsers.Count];
            for (var i = 0; i < datas.Length; ++i) {
                datas[i] = new SerializeData(parsers[i]);
            }
            return datas;
        }
        public static byte[] SerializeBytes(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption, GlobalCacheCompiler globalCache = null, short version = DefaultVersion) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream, version)) {
                    writer.Write((byte)0);      //占位符
                    writer.Write(int.MaxValue); //占位符
                    writer.Write(version);      //版本号
                    var datas = Serialize(breviary, buffer, searchPaths, compileOption, globalCache, version);
                    if (version >= ScorpioUtil.VersionGlobalCache) {
                        writer.Write(datas[0].GlobalCacheIndex);
                        if (datas[0].GlobalCacheIndex == -1) {
                            datas[0].GlobalCache.WriteConst(writer);
                        }
                    }
                    int length = datas.Length;
                    writer.Write(length);
                    for (var i = 0; i < length; ++i) {
                        var data = datas[i];
                        writer.Write(data.Breviary);
                        data.Serialize(writer, version >= ScorpioUtil.VersionGlobalCache);
                    }
                    return stream.ToArray();
                }
            }
        }
    }
}
