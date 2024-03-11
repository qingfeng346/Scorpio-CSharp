using System.IO;
using System.Collections.Generic;
using Scorpio.Instruction;
using Scorpio.Compile.Compiler;
using Scorpio.Tools;
using Scorpio.Runtime;
namespace Scorpio.Serialize {
    public class Serializer {
        public static short DefaultVersion = 4;
        public static SerializeData[] Serialize(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption, short? version = null, GlobalCacheCompiler globalCache = null) {
            short ver = version ?? DefaultVersion;
            var parsers = new List<ScriptParser>();
            parsers.Add(new ScriptParser(buffer.ParseTokens(), breviary, breviary, searchPaths, compileOption, parsers, globalCache, ver).Parse());
            var datas = new SerializeData[parsers.Count];
            for (var i = 0; i < datas.Length; ++i) {
                datas[i] = new SerializeData(parsers[i]);
            }
            return datas;
        }
        public static byte[] SerializeBytes(string breviary, string buffer, IEnumerable<string> searchPaths, CompileOption compileOption, short? version = null, GlobalCacheCompiler globalCache = null) {
            short ver = version ?? DefaultVersion;
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream, ver)) {
                    writer.Write((byte)0);      //占位符
                    writer.Write(int.MaxValue); //占位符
                    writer.Write(ver);          //版本号
                    var datas = Serialize(breviary, buffer, searchPaths, compileOption, ver, globalCache);
                    if (ver >= ScorpioUtil.VersionGlobalCache) {
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
                        data.Serialize(writer, ver < ScorpioUtil.VersionGlobalCache);
                    }
                    return stream.ToArray();
                }
            }
        }
        public static byte[] SerializeGlobalCache(GlobalCache globalCache) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ScorpioWriter(stream, ScorpioUtil.VersionGlobalCache)) {
                    globalCache.WriteConst(writer);
                    return stream.ToArray();
                }
            }
        }
    }
}
