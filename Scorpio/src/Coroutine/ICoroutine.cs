namespace Scorpio.Coroutine {
    public interface ICoroutine {
        bool IsDone { get; }
        ScriptValue Result { get; }
    }
}
