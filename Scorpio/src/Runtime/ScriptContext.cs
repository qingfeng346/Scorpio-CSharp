using Scorpio.Instruction;

namespace Scorpio.Runtime {

    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public partial class ScriptContext {
        internal class AsyncValue {
            public ScriptValue[] variable;
            public ScriptValue[] stack;
            public AsyncValue() {
                variable = new ScriptValue[AsyncValueLength];
                stack = new ScriptValue[AsyncValueLength];
            }
        }
        internal const int AsyncValueLength = 64;           //异步函数的最大栈和局部变量最大数量

        internal const int ValueCacheLength = 64;           //函数最大调用层级,超过会堆栈溢出
        internal const int StackValueLength = 256;          //堆栈数据最大数量
        internal const int VariableValueLength = 128;       //局部变量最大数量
        internal const int TryStackLength = 16;             //最多可以嵌套多少层try catch

        internal static ScriptValue[][] VariableValues = new ScriptValue[ValueCacheLength][]; //局部变量数据
        internal static ScriptValue[][] StackValues = new ScriptValue[ValueCacheLength][]; //堆栈数据
        internal static int[][] TryStackValues = new int[ValueCacheLength][]; //try catch数据
        internal static int VariableValueIndex = 0;

        internal static int AsyncValuePoolLength = 0;
        internal static AsyncValue[] AsyncValuePool = new AsyncValue[0];
        static ScriptContext() {
            for (var i = 0; i < ValueCacheLength; ++i) {
                StackValues[i] = new ScriptValue[StackValueLength];
                VariableValues[i] = new ScriptValue[VariableValueLength];
                TryStackValues[i] = new int[TryStackLength];
            }
        }
        public Script m_script; //脚本类
        private readonly double[] constDouble; //double常量
        private readonly long[] constLong; //long常量
        private readonly ScriptContext[] constContexts; //所有定义的函数
        private readonly ScriptClassData[] constClasses; //定义所有的类
        private readonly string m_Breviary; //摘要
        public readonly ScriptFunctionData m_FunctionData; //函数数据

        public ScriptContext(Script script, string breviary, ScriptFunctionData functionData, double[] constDouble, long[] constLong, ScriptContext[] constContexts, ScriptClassData[] constClasses) {
            m_script = script;
            this.constDouble = constDouble;
            this.constLong = constLong;
            this.constContexts = constContexts;
            this.constClasses = constClasses;
            m_Breviary = breviary;
            m_FunctionData = functionData;
        }
    }
}