using Scorpio.Instruction;
using System;

namespace Scorpio.Runtime {

    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public partial class ScriptContext {
        internal const int ValueCacheLength = 128; //函数最大调用层级,超过会堆栈溢出
        internal const int StackValueLength = 256; //堆栈数据最大数量
        internal const int VariableValueLength = 128; //局部变量最大数量
        internal const int TryStackLength = 16; //最多可以嵌套多少层try catch
        
        internal static ScriptValue[][] VariableValues = new ScriptValue[ValueCacheLength][]; //局部变量数据
        internal static ScriptValue[][] StackValues = new ScriptValue[ValueCacheLength][]; //堆栈数据
        internal static int[][] TryStackValues = new int[ValueCacheLength][]; //try catch数据
        internal static int VariableValueIndex = 0;
        static ScriptContext() {
            for (var i = 0; i < StackValues.Length; ++i) {
                StackValues[i] = new ScriptValue[StackValueLength];
            }
            for (var i = 0; i < VariableValues.Length; ++i) {
                VariableValues[i] = new ScriptValue[VariableValueLength];
            }
            for (var i = 0; i < TryStackValues.Length; ++i) {
                TryStackValues[i] = new int[TryStackLength];
            }
        }

        public readonly Script m_script; //脚本类
        private readonly ScriptGlobal m_global; //global

        private readonly double[] constDouble; //double常量
        private readonly long[] constLong; //long常量
        private readonly string[] constString; //string常量
        private readonly ScriptContext[] constContexts; //所有定义的函数
        private readonly ScriptClassData[] constClasses; //定义所有的类
        public readonly int internalCount; //内部变量数量

        private readonly string m_Breviary; //摘要
        private readonly ScriptFunctionData m_FunctionData; //函数数据
        private readonly ScriptInstruction[] m_scriptInstructions; //指令集

        public ScriptContext(Script script, string breviary, ScriptFunctionData functionData, double[] constDouble, long[] constLong, string[] constString, ScriptContext[] constContexts, ScriptClassData[] constClasses) {
            m_script = script;
            m_global = script.Global;
            this.constDouble = constDouble;
            this.constLong = constLong;
            this.constString = constString;
            this.constContexts = constContexts;
            this.constClasses = constClasses;
            this.internalCount = functionData.internalCount;

            m_Breviary = breviary;
            m_FunctionData = functionData;
            m_scriptInstructions = functionData.scriptInstructions;
        }
    }
}