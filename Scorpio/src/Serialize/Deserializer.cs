using System.IO;
using Scorpio.Instruction;
using Scorpio.Tools;
namespace Scorpio.Serialize {
    public static class Deserializer {
        public static SerializeData[] Deserialize(byte[] data) {
            using (var stream = new MemoryStream(data)) {
                return Deserialize(stream);
            }
        }
        public static SerializeData[] Deserialize(Stream stream) {
            using (var reader = new ScorpioReader(stream)) {
                reader.ReadByte();      //占位符
                reader.ReadInt32();     //占位符
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
