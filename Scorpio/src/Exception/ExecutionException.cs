namespace Scorpio.Exception {
    //执行代码异常
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
