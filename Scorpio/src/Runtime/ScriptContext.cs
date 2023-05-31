using Scorpio.Instruction;
using System;
using System.Collections.Generic;

namespace Scorpio.Runtime {

    //执行命令
    //注意事项:
    //所有调用另一个程序集的地方 都要new一个新的 否则递归调用会相互影响
    public partial class ScriptContext {
        internal class AsyncValue {
            public ScriptValue[] variable;
            public ScriptValue[] stack;
        }
        internal const int ValueCacheLength = 128;          //函数最大调用层级,超过会堆栈溢出
        internal const int StackValueLength = 256;          //堆栈数据最大数量
        internal const int VariableValueLength = 128;       //局部变量最大数量
        internal const int TryStackLength = 16;             //最多可以嵌套多少层try catch
        
        internal static ScriptValue[][] VariableValues = new ScriptValue[ValueCacheLength][]; //局部变量数据
        internal static ScriptValue[][] StackValues = new ScriptValue[ValueCacheLength][]; //堆栈数据
        internal static int[][] TryStackValues = new int[ValueCacheLength][]; //try catch数据
        internal static int VariableValueIndex = 0;
        internal static Queue<AsyncValue> AsyncValueQueue = new Queue<AsyncValue>();
        static ScriptContext() {
            for (var i = 0; i < ValueCacheLength; ++i) {
                StackValues[i] = new ScriptValue[StackValueLength];
                VariableValues[i] = new ScriptValue[VariableValueLength];
                TryStackValues[i] = new int[TryStackLength];
            }
        }
        private static AsyncValue AllocAsyncValue() {
            if (AsyncValueQueue.Count == 0)
                return new AsyncValue() { variable = new ScriptValue[64], stack = new ScriptValue[64] };
            return AsyncValueQueue.Dequeue();
        }
        private static void FreeAsyncValue(AsyncValue value) {
            Array.ForEach(value.variable, _ => _.Free());
            Array.ForEach(value.stack, _ => _.Free());
            AsyncValueQueue.Enqueue(value);
        }
        private static void Free(ref ScriptValue[] variableObjects, ref ScriptValue[] stackObjects) {
            --VariableValueIndex;
            Array.ForEach(variableObjects, _ => _.Free());
            Array.ForEach(stackObjects, _ => _.Free());
        }
        public Script m_script; //脚本类
        private ScriptGlobal m_global; //global

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
#if SCORPIO_DEBUG
        public void SetScript(Script script) {
            m_script = script;
            m_global = script.Global;
        }
#endif
    }
}