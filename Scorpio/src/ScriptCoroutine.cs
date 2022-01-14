using System.Collections;
using System.Collections.Generic;
using Scorpio.Coroutine;
namespace Scorpio {
    public partial class Script {
        private LinkedList<ScriptCoroutine> m_Coroutines = new LinkedList<ScriptCoroutine>();
        private List<ScriptCoroutine> m_AddCoroutines = new List<ScriptCoroutine>();
        private List<ScriptCoroutine> m_DelCoroutines = new List<ScriptCoroutine>();
        public ICoroutineProcessor CoroutineProcessor { get; set; } = new DefaultCoroutineProcessor();
        public ScriptCoroutine StartCoroutine(IEnumerator enumerator) {
            var scriptCoroutine = new ScriptCoroutine(this, enumerator);
            m_AddCoroutines.Add(scriptCoroutine);
            return scriptCoroutine;
        }
        public void StopCoroutine(ScriptCoroutine scriptCoroutine) {
            scriptCoroutine.Stop();
        }
        internal void Remove(ScriptCoroutine scriptCoroutine) {
            m_DelCoroutines.Add(scriptCoroutine);
        }
        public void StopAllCoroutine() {
            m_DelCoroutines.AddRange(m_Coroutines);
        }
        public bool UpdateCoroutine() {
            if (m_DelCoroutines.Count > 0) {
                var length = m_DelCoroutines.Count;
                for (var i = 0; i < length; ++i) {
                    m_Coroutines.Remove(m_DelCoroutines[i]);
                }
                m_DelCoroutines.Clear();
            }
            if (m_AddCoroutines.Count > 0) {
                var length = m_AddCoroutines.Count;
                for (var i = 0; i < length; ++i) {
                    m_Coroutines.AddLast(m_AddCoroutines[i]);
                }
                m_AddCoroutines.Clear();
            }
            foreach (var coroutine in m_Coroutines) {
                coroutine.UpdateCoroutine();
            }
            return m_Coroutines.Count > 0;
        }
    }
}
