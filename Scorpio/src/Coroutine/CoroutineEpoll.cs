namespace Scorpio.Coroutine {
    internal class CoroutineEpoll : ICoroutine {
        public void Done() {
            IsDone = true;
        }
        public bool IsDone { get; private set; }
    }
}
