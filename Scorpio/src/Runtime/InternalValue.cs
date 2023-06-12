namespace Scorpio.Runtime {
    public class InternalValue {
        private Script script;
        private int referenceCount = 0;
        public ScriptValue value;
        public InternalValue(Script script) {
            this.script = script;
        }
        public InternalValue Reference() {
            ++referenceCount;
            return this;
        }
        public void Free() {
            if (--referenceCount == 0) {
                value.Free();
                script.Free(this);
            }
        }
    }
}
