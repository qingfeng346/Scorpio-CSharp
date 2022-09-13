using System.Collections;
namespace Scorpio.Coroutine {
    public class DefaultCoroutineProcessor : ICoroutineProcessor {
        ICoroutine coroutine;
        public void SetCurrent(object current) {
            if (coroutine == current) { return; }
            coroutine = current as ICoroutine;
        }
        public bool MoveNext(out object result) {
            if (coroutine != null) {
                if (!coroutine.IsDone) {
                    result = null;
                    return true;
                } else {
                    result = coroutine.Result;
                    return false;
                }
            }
            result = null;
            return false;
        }
    }
}
