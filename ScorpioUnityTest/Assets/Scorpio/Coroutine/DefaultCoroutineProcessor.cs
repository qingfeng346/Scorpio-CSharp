using System.Collections;
namespace Scorpio.Coroutine {
    public class DefaultCoroutineProcessor : ICoroutineProcessor {
        ICoroutine coroutine;
        //AsyncOperation asyncOperation;
        public void SetCurrent(object current) {
            if (coroutine == current) { return; }
            coroutine = current as ICoroutine;
            //asyncOperation = current as AsyncOperation;
        }
        public bool MoveNext(IEnumerator enumerator) {
            if (coroutine != null && !coroutine.IsDone) {
                return true;
            }
            //if (asyncOperation != null && !asyncOperation.isDone) {
            //    return true;
            //}
            return enumerator.MoveNext();
        }
    }
}
