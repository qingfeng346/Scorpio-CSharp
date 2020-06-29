using Scorpio.Instruction;
namespace Scorpio.Serialize {
    public class Deserializer {
        public static SerializeData Deserialize(byte[] data) {
            return new SerializeData().Parser(data);
        }
    }
}
