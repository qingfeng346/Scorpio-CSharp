using System.IO;
using System.Text;

namespace Scorpio.Library {
    public class LibraryIO {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public static void Load(Script script) {
            var map = script.CreateMap();
            map.SetValue("toString", script.CreateFunction(new toString()));
            map.SetValue("toBytes", script.CreateFunction(new toBytes()));
            map.SetValue("readAll", script.CreateFunction(new readAll()));
            map.SetValue("writeAll", script.CreateFunction(new writeAll()));
            script.SetGlobal("io", new ScriptValue(map));
        }
        private class toString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return new ScriptValue(encoding.GetString((byte[])args[0].Value));
            }
        }
        private class toBytes : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return ScriptValue.CreateObject(encoding.GetBytes(args[0].ToString()));
            }
        }
        private class readAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return new ScriptValue(encoding.GetString(File.ReadAllBytes(args[0].ToString())));
            }
        }
        private class writeAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 2 ? Encoding.GetEncoding(args[2].ToString()) : DefaultEncoding;
                File.WriteAllBytes(args[0].ToString(), encoding.GetBytes(args[1].ToString()));
                return ScriptValue.Null;
            }
        }
    }
}
