namespace Scorpio.Exception
{
    //执行代码异常
    public class ExecutionException : System.Exception {
        public ExecutionException(string strMessage) : base(strMessage) { }
    }
    public class ExecutionStackException : System.Exception {
        public ExecutionStackException(string strMessage) : base(strMessage) { }
    }
}
