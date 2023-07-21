namespace Scorpio.Coroutine {
    public class DefaultCoroutineProcessor : ICoroutineProcessor {
        ICoroutine coroutine;
        public void SetCurrent(object current) {
            if (coroutine == current) { return; }
            coroutine = current as ICoroutine;
        }
        public bool MoveNext(out ScriptValue result) {
            result = default;
            if (coroutine != null) {
                if (!coroutine.IsDone) {
                    return true;
                } else {
                    result = coroutine.Result;
                    coroutine.Dispose();
                    return false;
                }
            }
            return false;
        }
    }
}
