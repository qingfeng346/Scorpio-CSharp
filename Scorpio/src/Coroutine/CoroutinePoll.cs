using System;
namespace Scorpio.Coroutine {
    internal class CoroutinePoll : ICoroutine {
        private ScriptValue function;
        private ScriptValue result;
        public CoroutinePoll(ScriptValue function, ScriptValue result) {
            this.function = function.Reference();
            this.result = result.Reference();
        }
        public bool IsDone => function.Call(ScriptValue.Null).IsTrue;
        public ScriptValue Result => result.Call(ScriptValue.Null);
        public void Dispose() {
            function.Free();
            result.Free();
        }
    }
    internal class CoroutineFuncPoll : ICoroutine {
        private Func<bool> function;
        public CoroutineFuncPoll(Func<bool> function) {
            this.function = function;
        }
        public bool IsDone => function();
        public ScriptValue Result => ScriptValue.Null;
        public void Dispose() { }
    }
}
