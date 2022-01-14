using System.Collections;
namespace Scorpio.Coroutine {
    public interface ICoroutineProcessor {
        void SetCurrent(object current);
        bool MoveNext(IEnumerator enumerator);
    }
}
