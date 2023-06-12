using System;
using System.Diagnostics;
using System.Text;
namespace Scorpio.Tools {
    public interface IScorpioLogger {
        void error(string message);
    }
    public static class ScorpioLogger {
        private class DefaultLogger : IScorpioLogger {
            public void error(string message) {
                var stack = new StackTrace(1, true);
                var builder = new StringBuilder();
                builder.AppendLine(message);
                for (var i = 0; i < stack.FrameCount; i++) {
                    var frame = stack.GetFrame(i);
                    builder.Append($"    {frame.GetMethod().Name} {frame.GetFileName()}:{frame.GetFileLineNumber()}\n");
                }
                Console.WriteLine(builder.ToString());
            }
        }
        public static IScorpioLogger ilog { get; set; }
        static ScorpioLogger() {
            ilog = new DefaultLogger();
        }
        public static void error(string message) {
            if (ilog != null) ilog.error(message);

        }
    }
}
