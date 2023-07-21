using System;
namespace Scorpio.Coroutine {
    public interface ICoroutine : IDisposable {
        bool IsDone { get; }
        ScriptValue Result { get; }
    }
}
