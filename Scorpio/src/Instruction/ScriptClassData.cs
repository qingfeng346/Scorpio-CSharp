namespace Scorpio.Instruction {
    //单个类的信息
    public class ScriptClassData {
        public int name;            //类名
        public int parent;          //父级字符串索引，如果是-1则无父级
        public long[] functions;    //所有的函数 前32位是名字字符串索引 后32位为函数索引
    }
}
