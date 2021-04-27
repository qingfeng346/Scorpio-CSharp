using System.IO;
using System.Text;
using System;
namespace Scorpio.Library {
    public class LibraryIO {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public const long BaseTime = 621355968000000000;                        //1970, 1, 1, 0, 0, 0, DateTimeKind.Utc
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("unixNow", script.CreateFunction(new unixNow()));
            map.SetValue("toString", script.CreateFunction(new toString()));
            map.SetValue("toBytes", script.CreateFunction(new toBytes()));
            map.SetValue("readAll", script.CreateFunction(new readAll()));
            map.SetValue("writeAll", script.CreateFunction(new writeAll()));
            map.SetValue("readAllString", script.CreateFunction(new readAllString()));
            map.SetValue("writeAllString", script.CreateFunction(new writeAllString()));
            map.SetValue("fileExist", script.CreateFunction(new fileExist()));
            map.SetValue("pathExist", script.CreateFunction(new pathExist()));
            map.SetValue("createPath", script.CreateFunction(new createPath()));
            map.SetValue("deleteFile", script.CreateFunction(new deleteFile()));
            map.SetValue("deletePath", script.CreateFunction(new deletePath()));
            map.SetValue("getFiles", script.CreateFunction(new getFiles()));
            map.SetValue("getPaths", script.CreateFunction(new getPaths()));
            map.SetValue("workPath", script.CreateFunction(new workPath()));
            map.SetValue("lineArgs", script.CreateFunction(new lineArgs()));
            script.SetGlobal("io", new ScriptValue(map));
        }
        private class unixNow : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((DateTime.UtcNow.Ticks - BaseTime) / 10000);
            }
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
                return ScriptValue.CreateValue(encoding.GetBytes(args[0].ToString()));
            }
        }
        private class readAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(File.ReadAllBytes(args[0].ToString()));
            }
        }
        private class writeAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                File.WriteAllBytes(args[0].ToString(), args[1].Value as byte[]);
                return ScriptValue.Null;
            }
        }
        private class readAllString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return new ScriptValue(encoding.GetString(File.ReadAllBytes(args[0].ToString())));
            }
        }
        private class writeAllString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = length > 2 ? Encoding.GetEncoding(args[2].ToString()) : DefaultEncoding;
                File.WriteAllBytes(args[0].ToString(), encoding.GetBytes(args[1].ToString()));
                return ScriptValue.Null;
            }
        }
        private class fileExist : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return File.Exists(args[0].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class pathExist : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return Directory.Exists(args[0].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class createPath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return ScriptValue.Null;
            }
        }
        private class deleteFile : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var file = args[0].ToString();
                if (File.Exists(file)) File.Delete(file);
                return ScriptValue.Null;
            }
        }
        private class deletePath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                if (Directory.Exists(path)) Directory.Delete(path);
                return ScriptValue.Null;
            }
        }
        private class getFiles : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var searchPattern = length > 1 ? args[1].ToString() : "*";
                var searchOption = length > 2 ? (args[1].IsTrue ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : SearchOption.TopDirectoryOnly;
                return new ScriptValue(Directory.GetFiles(path, searchPattern, searchOption));
            }
        }
        private class getPaths : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var searchPattern = length > 1 ? args[1].ToString() : "*";
                var searchOption = length > 2 ? (args[1].IsTrue ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : SearchOption.TopDirectoryOnly;
                return new ScriptValue(Directory.GetDirectories(path, searchPattern, searchOption));
            }
        }
        private class workPath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Environment.CurrentDirectory);
            }
        }
        private class lineArgs : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(Environment.GetCommandLineArgs());
            }
        }
    }
}
