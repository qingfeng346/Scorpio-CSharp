using System.IO;
using Scorpio.Instruction;
using Scorpio.Runtime;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public static class Deserializer {
        public static SerializeData[] Deserialize(byte[] data, GlobalCache[] globalCaches) {
            return Deserialize(data, 0, data.Length, globalCaches);
        }
        public static SerializeData[] Deserialize(byte[] data, int index, int count, GlobalCache[] globalCaches) {
            using (var stream = new MemoryStream(data, index, count)) {
                return Deserialize(stream, globalCaches);
            }
        }
        public static SerializeData[] Deserialize(Stream stream, GlobalCache[] globalCaches) {
            using (var reader = new ScorpioReader(stream)) {
                reader.ReadByte();      //占位符
                reader.ReadInt32();     //占位符
                reader.version = reader.ReadInt16();        //version 解析版本号
                if (reader.version >= ScorpioUtil.VersionGlobalCache) {
                    var globalCacheIndex = reader.ReadInt32();
                    var globalCache = globalCacheIndex == -1 ? new GlobalCache().ReadConst(reader) : globalCaches[globalCacheIndex];
                    var number = reader.ReadInt32();            //文件数量
                    var datas = new SerializeData[number];
                    for (var i = 0; i < number; ++i) {
                        datas[i] = new SerializeData(reader.ReadString()).Deserialize(reader, globalCache);
                    }
                    return datas;
                } else {
                    var number = reader.ReadInt32();            //文件数量
                    var datas = new SerializeData[number];
                    for (var i = 0; i < number; ++i) {
                        datas[i] = new SerializeData(reader.ReadString()).Deserialize(reader, null);
                    }
                    return datas;
                }
            }
        }
        public static GlobalCache DeserializeGlobalCache(byte[] data) {
            return DeserializeGlobalCache(data, 0, data.Length);
        }
        public static GlobalCache DeserializeGlobalCache(byte[] data, int index, int count) {
            using (var stream = new MemoryStream(data, index, count)) {
                return DeserializeGlobalCache(stream);
            }
        }
        public static GlobalCache DeserializeGlobalCache(Stream stream) {
            using (var reader = new ScorpioReader(stream)) {
                return new GlobalCache().ReadConst(reader);
            }
        }
    }
}
