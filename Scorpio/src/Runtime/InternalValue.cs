using Scorpio.Tools;

namespace Scorpio.Runtime {
    public class InternalValue : IPool {
        private Script script;
        private int referenceCount = 0;
        public ScriptValue value;
        public InternalValue(Script script) {
            this.script = script;
        }
        public void Alloc() {
            referenceCount = 1;
        }
        public void Free() {

        }
        public InternalValue Reference() {
            ++referenceCount;
            return this;
        }
        public void Release() {
            if (--referenceCount == 0) {
                value.Free();
                script.Free(this);
            }
        }
    }
}
