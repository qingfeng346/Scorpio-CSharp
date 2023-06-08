//using System;
//using System.Diagnostics;
//using System.Text;
//namespace Scorpio.Tools {
//    internal static class logger {
//        public static void debug(string message) {
//            var stack = new StackTrace(1, true);
//            var builder = new StringBuilder();
//            builder.AppendLine(message);
//            for (var i = 0; i < stack.FrameCount; i++) {
//                var frame = stack.GetFrame(i);
//                builder.Append($"    {frame.GetMethod().Name} {frame.GetFileName()}:{frame.GetFileLineNumber()}\n");
//            }
//            Console.WriteLine(builder.ToString());
//        }
//    }
//}
