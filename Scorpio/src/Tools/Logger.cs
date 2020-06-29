using System.Diagnostics;
namespace Scorpio.Tools {
    public static class Logger {
        [Conditional("DEBUG")]
        public static void debug(bool condition, string format) {
            Debug.WriteLineIf(condition, format);
        }
        [Conditional("DEBUG")]
        public static void debug(bool condition, string format, params object[] args) {
            Debug.WriteLineIf(condition, string.Format(format, args));
        }
        [Conditional("DEBUG")]
        public static void debug(string format) {
            Debug.WriteLine(format);
        }
        [Conditional("DEBUG")]
        public static void debug(string format, params object[] args) {
            Debug.WriteLine(format, args);
        }
    }
}
