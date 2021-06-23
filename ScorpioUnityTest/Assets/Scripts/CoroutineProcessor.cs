using System.Collections;
using Scorpio.Coroutine;
using UnityEngine;
public class CoroutineProcessor : ICoroutineProcessor {
    object current;
    ICoroutine coroutine;
    AsyncOperation asyncOperation;
    public void SetCurrent(object current) {
        if (this.current == current) { return; }
        this.current = current;
        if (current is ICoroutine) {
            coroutine = current as ICoroutine;
        } else if (current is AsyncOperation) {
            asyncOperation = current as AsyncOperation;
        }
    }
    public bool MoveNext(IEnumerator enumerator) {
        if (coroutine != null) {
            if (!coroutine.IsDone) {
                return true;
            }
        } else if (asyncOperation != null) {
            if (!asyncOperation.isDone) {
                return true;
            }
        }
        return enumerator.MoveNext();
    }
}