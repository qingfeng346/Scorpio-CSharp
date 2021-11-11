using System;
namespace Scorpio.Coroutine {
    internal class CoroutinePoll : ICoroutine {
        private ScriptValue function;
        public CoroutinePoll(ScriptValue function) {
            this.function = function;
        }
        public bool IsDone => function.Call(ScriptValue.Null).IsTrue;
    }
    internal class CoroutineFuncPoll : ICoroutine {
        private Func<bool> function;
        public CoroutineFuncPoll(Func<bool> function) {
            this.function = function;
        }
        public bool IsDone => function();
    }
}
