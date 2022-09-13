using System;
namespace Scorpio.Coroutine {
    internal class CoroutinePoll : ICoroutine {
        private ScriptValue function;
        private ScriptValue result;
        public CoroutinePoll(ScriptValue function, ScriptValue result) {
            this.function = function;
            this.result = result;
        }
        public bool IsDone => function.Call(ScriptValue.Null).IsTrue;
        public object Result => result.Call(ScriptValue.Null);
    }
    internal class CoroutineFuncPoll : ICoroutine {
        private Func<bool> function;
        public CoroutineFuncPoll(Func<bool> function) {
            this.function = function;
        }
        public bool IsDone => function();
        public object Result => null;
    }
}
