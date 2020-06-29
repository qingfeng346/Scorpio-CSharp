namespace Scorpio.Exception {
    /// <summary> 执行代码抛出异常 </summary>
    public class ExecutionException : System.Exception {
        public string message;
        public ExecutionException(string message) {
            this.message = message;
        }
        public ExecutionException(string message, System.Exception innerException) : base(message, innerException) {
            this.message = message;
        }
        public override string Message => message;
    }
}
