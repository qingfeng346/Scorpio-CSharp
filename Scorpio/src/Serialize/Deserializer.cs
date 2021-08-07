using System.IO;
using Scorpio.Instruction;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public static class Deserializer {
        [System.Obsolete]
        public static SerializeData[] DeserializeV1(string breviary, byte[] data) {
            using (var stream = new MemoryStream(data)) {
                return DeserializeV1(breviary, stream);
            }
        }
        [System.Obsolete]
        public static SerializeData[] DeserializeV1(string breviary, Stream stream) {
            using (var reader = new ScorpioReader(stream)) {
                reader.ReadByte();
                return new SerializeData[1] { new SerializeData(breviary).Deserialize(reader) };
            }
        }
        public static SerializeData[] Deserialize(byte[] data) {
            using (var stream = new MemoryStream(data)) {
                return Deserialize(stream);
            }
        }
        public static SerializeData[] Deserialize(Stream stream) {
            using (var reader = new ScorpioReader(stream)) {
                reader.ReadBytes(2);    //占位符
                reader.ReadInt16();     //version 解析版本号
                var number = reader.ReadInt32();
                var datas = new SerializeData[number];
                for (var i = 0; i < number; ++i) {
                    datas[i] = new SerializeData(reader.ReadString()).Deserialize(reader);
                }
                return datas;
            }
        }
    }
}
