namespace Scorpio.Coroutine {
    internal class CoroutineCallback : ICoroutine {
        public void Done() {
            IsDone = true;
        }
        public bool IsDone { get; private set; }
    }
}
