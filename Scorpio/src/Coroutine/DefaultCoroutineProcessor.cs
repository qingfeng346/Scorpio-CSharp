using System.Collections;
namespace Scorpio.Coroutine {
    public class DefaultCoroutineProcessor : ICoroutineProcessor {
        ICoroutine coroutine;
        public void SetCurrent(object current) {
            if (coroutine == current) { return; }
            coroutine = current as ICoroutine;
        }
        public bool MoveNext(IEnumerator enumerator) {
            if (coroutine != null && !coroutine.IsDone) {
                return true;
            }
            return enumerator.MoveNext();
        }
    }
}
