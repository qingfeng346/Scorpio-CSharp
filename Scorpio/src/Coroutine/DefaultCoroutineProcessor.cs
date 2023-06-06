using System.Collections;
namespace Scorpio.Coroutine {
    public class DefaultCoroutineProcessor : ICoroutineProcessor {
        ICoroutine coroutine;
        public void SetCurrent(object current) {
            if (coroutine == current) { return; }
            coroutine = current as ICoroutine;
        }
        public bool MoveNext(out ScriptValue result) {
            if (coroutine != null) {
                if (!coroutine.IsDone) {
                    result = ScriptValue.Null;
                    return true;
                } else {
                    result = coroutine.Result;
                    return false;
                }
            }
            result = ScriptValue.Null;
            return false;
        }
    }
}
