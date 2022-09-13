namespace Scorpio.Coroutine {
    public interface ICoroutine {
        bool IsDone { get; }
        object Result { get; }
    }
}
