using System.Text;

namespace Scorpio.Compiler {
    //单个函数的信息
    public class ScriptFunctionData {
        public ScriptInstruction[] scriptInstructions;      //命令列表
        public int parameterCount;                          //参数个数
        public bool param;                                  //是否是变长参数
        public int variableCount;                           //局部变量数量
        public int internalCount;                           //内部变量数量
        public int[] internals;                             //内部变量赋值
        public string ToString(double[] constDouble, long[] constLong, string[] constString) {
            var builder = new StringBuilder();
            if (internals != null) {
                foreach (var inter in internals) {
                    int source = (inter >> 16);
                    int target = (inter & 0xffff);
                    builder.AppendLine($"internal  {source} => {target}");
                }
            }
            for (int i = 0; i < scriptInstructions.Length; ++i) {
                var instruction = scriptInstructions[i];
                var opcode = instruction.opcode.ToString();
                if (opcode.Length < 20) {
                    for (int j = opcode.Length; j < 20; ++j) {
                        opcode += " ";
                    }
                }
                builder.Append($"{i.ToString("D5")} {opcode} ");
                var value = "";
                switch (instruction.opcode) {
                    case Opcode.LoadConstDouble: value = constDouble[instruction.opvalue].ToString(); break;
                    case Opcode.LoadConstLong: value = constLong[instruction.opvalue].ToString(); break;
                    case Opcode.LoadConstString:
                    case Opcode.LoadGlobalString:
                    case Opcode.StoreGlobalString:
                    case Opcode.LoadValueString:
                    case Opcode.StoreValueString:
                        value = constString[instruction.opvalue].ToString();
                        break;
                    case Opcode.LoadInternal:
                        value = "load_internal_" + instruction.opvalue;
                        break;
                    case Opcode.LoadLocal:
                        value = "load_local_" + instruction.opvalue;
                        break;
                    case Opcode.LoadGlobal:
                        value = "load_global_" + instruction.opvalue;
                        break;
                    case Opcode.StoreInternal:
                        value = "store_internal_" + instruction.opvalue;
                        break;
                    case Opcode.StoreLocal:
                        value = "store_local_" + instruction.opvalue;
                        break;
                    case Opcode.StoreGlobal:
                        value = "store_global_" + instruction.opvalue;
                        break;
                    case Opcode.Jump:
                    case Opcode.FalseTo:
                    case Opcode.TrueTo:
                    case Opcode.TrueLoadTrue:
                    case Opcode.FalseLoadFalse:
                        value = instruction.opvalue.ToString("D5");
                        break;
                    case Opcode.LoadConstNull: value = "null"; break;
                    case Opcode.LoadConstTrue: value = "true"; break;
                    case Opcode.LoadConstFalse: value = "false"; break;
                    case Opcode.LoadValueObject:
                    case Opcode.CopyStackTop:
                    case Opcode.Plus:
                    case Opcode.Pop:
                        break;
                    default: value = instruction.opvalue.ToString(); break;
                }
                builder.AppendLine(value);
            }
            return builder.ToString();
        }
    }
}
