namespace Scorpio.Exception {
    /// <summary> 脚本内主动抛出的异常 </summary>
    public class ScriptException : ExecutionException {
        public ScriptValue value;
        public ScriptException(ScriptValue value) : base(value.ToString()) {
            this.value = value;
        }
    }
}