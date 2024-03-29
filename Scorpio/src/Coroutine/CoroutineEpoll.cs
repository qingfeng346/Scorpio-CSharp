﻿namespace Scorpio.Coroutine {
    internal class CoroutineEpoll : ICoroutine {
        public void Done(ScriptValue result) {
            IsDone = true;
            Result = result;
        }
        public bool IsDone { get; private set; }
        public ScriptValue Result { get; private set; }
    }
}
