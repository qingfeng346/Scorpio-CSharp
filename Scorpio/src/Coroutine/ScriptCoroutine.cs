using System.Collections;
using System.Collections.Generic;
namespace Scorpio.Coroutine {
    public class ScriptCoroutine {
        static Stack<IEnumerator> IEnumeratorProcessingStack = new Stack<IEnumerator>(32);
        private IEnumerator m_routine;
        private Script m_script;
        private ICoroutineProcessor m_processor;
        public bool IsDone { get; private set; }
        public ScriptCoroutine(Script script, IEnumerator routine) {
            m_script = script;
            m_routine = routine;
            m_processor = script.CoroutineProcessor;
            IsDone = false;
        }
        public void UpdateCoroutine() {
            if (!ProcessIEnumeratorRecursive(m_routine)) {
                Stop();
            }
        }
        public void Stop() {
            IsDone = true;
            m_script.Remove(this);
        }
        private bool ProcessIEnumeratorRecursive(IEnumerator enumerator) {
            var root = enumerator;
            while (enumerator.Current as IEnumerator != null) {
                IEnumeratorProcessingStack.Push(enumerator);
                enumerator = enumerator.Current as IEnumerator;
            }
            bool result;
            if (enumerator.Current == null) {
                result = enumerator.MoveNext();
            } else if (m_processor == null) {
                result = enumerator.MoveNext();
            } else {
                m_processor.SetCurrent(enumerator.Current);
                result = m_processor.MoveNext(enumerator);
            }
            while (IEnumeratorProcessingStack.Count > 1) {
                if (result) {
                    IEnumeratorProcessingStack.Clear();
                } else {
                    result = IEnumeratorProcessingStack.Pop().MoveNext();
                }
            }
            if (IEnumeratorProcessingStack.Count > 0 && !result && root == IEnumeratorProcessingStack.Pop()) {
                result = root.MoveNext();
            }
            return result;
        }
    }
}
