using System.IO;
using System.Text;
using System;
namespace Scorpio.Library {
    public class LibraryIO {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public const long BaseTime = 621355968000000000;                        //1970, 1, 1, 0, 0, 0, DateTimeKind.Utc
        public static long UnixNow => (DateTime.UtcNow.Ticks - BaseTime) / 10000;
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("unixNow", new unixNow()),
                ("toString", new toString()),
                ("toBytes", new toBytes()),
                ("readAll", new readAll()),
                ("writeAll", new writeAll()),
                ("readAllString", new readAllString()),
                ("writeAllString", new writeAllString()),
                ("readAllLines", new readAllLines(script)),
                ("writeAllLines", new writeAllLines()),
                ("appendAllText", new appendAllText()),
                ("appendAllLines", new appendAllLines()),
                ("fileExist", new fileExist()),
                ("pathExist", new pathExist()),
                ("createPath", new createPath()),
                ("deleteFile", new deleteFile()),
                ("deletePath", new deletePath()),
                ("copyFile", new copyFile()),
                ("moveFile", new moveFile()),
                ("movePath", new movePath()),
                ("getFiles", new getFiles()),
                ("getPaths", new getPaths()),
                ("getCreationTime", new getCreationTime()),
                ("getLastAccessTime", new getLastAccessTime()),
                ("getLastWriteTime", new getLastWriteTime()),
                ("setCreationTime", new setCreationTime()),
                ("setLastAccessTime", new setLastAccessTime()),
                ("setLastWriteTime", new setLastWriteTime()),
                ("getExtension", new getExtension()),
                ("getFileName", new getFileName()),
                ("getDirectoryName", new getDirectoryName()),
                ("changeExtension", new changeExtension()),
                ("combine", new combine()),
                ("getTempFileName", new getTempFileName()),
                ("getTempPath", new getTempPath()),
                ("md5", new md5()),
                ("md5Bytes", new md5Bytes()),
                ("md5HashToString", new md5HashToString()),
                ("toBase64", new toBase64()),
                ("fromBase64", new fromBase64()),
                ("workPath", new workPath()),
                ("lineArgs", new lineArgs()),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            script.SetGlobal("io", new ScriptValue(map));
        }
        private class unixNow : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(UnixNow);
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
                return ScriptValue.CreateValue(File.ReadAllBytes(args[0].ToString()));
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
        private class readAllLines : ScorpioHandle {
            private readonly Script script;
            public readAllLines(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(script.CreateArray(File.ReadAllLines(args[0].ToString(), length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding)));
            }
        }
        private class writeAllLines : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                File.WriteAllLines(args[0].ToString(), args[1].Get<ScriptArray>().ToArray<string>(), length > 2 ? Encoding.GetEncoding(args[2].ToString()) : DefaultEncoding);
                return ScriptValue.Null;
            }
        }
        private class appendAllText : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                File.AppendAllText(args[0].ToString(), args[1].ToString(), length > 2 ? Encoding.GetEncoding(args[2].ToString()) : DefaultEncoding);
                return ScriptValue.Null;
            }
        }
        private class appendAllLines : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                File.AppendAllLines(args[0].ToString(), args[1].Get<ScriptArray>().ToArray<string>(), length > 2 ? Encoding.GetEncoding(args[2].ToString()) : DefaultEncoding);
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
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class deleteFile : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var file = args[0].ToString();
                if (File.Exists(file)) {
                    File.Delete(file);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class deletePath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                if (Directory.Exists(path)) {
                    Directory.Delete(path);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class copyFile : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var source = args[0].ToString();
                var target = args[1].ToString();
                File.Copy(source, target, length > 2 ? args[2].IsTrue : false);
                return ScriptValue.Null;
            }
        }
        private class moveFile : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var source = args[0].ToString();
                var target = args[1].ToString();
                File.Move(source, target);
                return ScriptValue.Null;
            }
        }
        private class movePath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var source = args[0].ToString();
                var target = args[1].ToString();
                Directory.Move(source, target);
                return ScriptValue.Null;
            }
        }
        private class getFiles : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var searchPattern = length > 1 ? args[1].ToString() : "*";
                var searchOption = length > 2 ? (args[2].IsTrue ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : SearchOption.TopDirectoryOnly;
                return ScriptValue.CreateValue(Directory.GetFiles(path, searchPattern, searchOption));
            }
        }
        private class getPaths : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var searchPattern = length > 1 ? args[1].ToString() : "*";
                var searchOption = length > 2 ? (args[2].IsTrue ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) : SearchOption.TopDirectoryOnly;
                return ScriptValue.CreateValue(Directory.GetDirectories(path, searchPattern, searchOption));
            }
        }
        private class getCreationTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var isDirectory = length > 1 ? args[1].IsTrue : false;
                if (length > 2 ? args[2].IsTrue : false) {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetCreationTimeUtc(path) : File.GetCreationTimeUtc(path));
                } else {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetCreationTime(path) : File.GetCreationTime(path));
                }
            }
        }
        private class getLastAccessTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var isDirectory = length > 1 ? args[1].IsTrue : false;
                if (length > 2 ? args[2].IsTrue : false) {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetLastAccessTimeUtc(path) : File.GetLastAccessTimeUtc(path));
                } else {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetLastAccessTime(path) : File.GetLastAccessTime(path));
                }
            }
        }
        private class getLastWriteTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var isDirectory = length > 1 ? args[1].IsTrue : false;
                if (length > 2 ? args[2].IsTrue : false) {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetLastWriteTimeUtc(path) : File.GetLastWriteTimeUtc(path));
                } else {
                    return ScriptValue.CreateValue(isDirectory ? Directory.GetLastWriteTime(path) : File.GetLastWriteTime(path));
                }
            }
        }
        private class setCreationTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var time = (DateTime)args[1].Value;
                var isDirectory = length > 2 ? args[2].IsTrue : false;
                if (length > 3 ? args[3].IsTrue : false) {
                    if (isDirectory) {
                        Directory.SetCreationTimeUtc(path, time);
                    } else {
                        File.SetCreationTimeUtc(path, time);
                    }
                } else {
                    if (isDirectory) {
                        Directory.SetCreationTime(path, time);
                    } else {
                        File.SetCreationTime(path, time);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class setLastAccessTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var time = (DateTime)args[1].Value;
                var isDirectory = length > 2 ? args[2].IsTrue : false;
                if (length > 3 ? args[3].IsTrue : false) {
                    if (isDirectory) {
                        Directory.SetLastAccessTimeUtc(path, time);
                    } else {
                        File.SetLastAccessTimeUtc(path, time);
                    }
                } else {
                    if (isDirectory) {
                        Directory.SetLastAccessTime(path, time);
                    } else {
                        File.SetLastAccessTime(path, time);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class setLastWriteTime : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var path = args[0].ToString();
                var time = (DateTime)args[1].Value;
                var isDirectory = length > 2 ? args[2].IsTrue : false;
                if (length > 3 ? args[3].IsTrue : false) {
                    if (isDirectory) {
                        Directory.SetLastWriteTimeUtc(path, time);
                    } else {
                        File.SetLastWriteTimeUtc(path, time);
                    }
                } else {
                    if (isDirectory) {
                        Directory.SetLastWriteTime(path, time);
                    } else {
                        File.SetLastWriteTime(path, time);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class getExtension : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.GetExtension(args[0].ToString()));
            }
        }
        private class getFileName : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((length > 1 ? args[1].IsTrue : true) ? Path.GetFileName(args[0].ToString()) : Path.GetFileNameWithoutExtension(args[0].ToString()));
            }
        }
        private class getDirectoryName : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.GetDirectoryName(args[0].ToString()));
            }
        }
        private class getFullPath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.GetFullPath(args[0].ToString()));
            }
        }
        private class changeExtension : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.ChangeExtension(args[0].ToString(), args[1].ToString()));
            }
        }
        private class combine : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var paths = new string[length];
                for (var i = 0; i < length; i++) {
                    paths[i] = args[i].ToString();
                }
                return new ScriptValue(Path.Combine(paths));
            }
        }
        private class getTempFileName : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.GetTempFileName());
            }
        }
        private class getTempPath : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Path.GetTempPath());
            }
        }
        private class md5 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    if (length > 1 && args[1].IsTrue) {
                        using (var fileStream = File.OpenRead(args[0].stringValue)) {
                            return new ScriptValue(ScorpioMD5.GetMd5String(fileStream));
                        }
                    } else {
                        return new ScriptValue(ScorpioMD5.GetMd5String(args[0].stringValue));
                    }
                } else {
                    var value = args[0].scriptValue.Value;
                    if (value is Stream) {
                        return new ScriptValue(ScorpioMD5.GetMd5String((Stream)value));
                    } else if (value is byte[]) {
                        var bytes = (byte[])value;
                        return new ScriptValue(ScorpioMD5.GetMd5String(bytes, length > 1 ? args[1].ToInt32() : 0, length > 2 ? args[2].ToInt32() : bytes.Length));
                    } else {
                        return ScriptValue.Null;
                    }
                }
            }
        }
        private class md5Bytes : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    if (length > 1 && args[1].IsTrue) {
                        using (var fileStream = File.OpenRead(args[0].stringValue)) {
                            return ScriptValue.CreateValue(ScorpioMD5.GetMd5Bytes(fileStream));
                        }
                    } else {
                        return ScriptValue.CreateValue(ScorpioMD5.GetMd5Bytes(args[0].stringValue));
                    }
                } else {
                    var value = args[0].scriptValue.Value;
                    if (value is Stream) {
                        return ScriptValue.CreateValue(ScorpioMD5.GetMd5Bytes((Stream)value));
                    } else if (value is byte[]) {
                        var bytes = (byte[])value;
                        return ScriptValue.CreateValue(ScorpioMD5.GetMd5Bytes(bytes, length > 1 ? args[1].ToInt32() : 0, length > 2 ? args[2].ToInt32() : bytes.Length));
                    } else {
                        return ScriptValue.Null;
                    }
                }
            }
        }
        private class md5HashToString: ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(ScorpioMD5.HashToString((byte[])args[0].Value));
            }
        }
        private class toBase64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    return new ScriptValue(Convert.ToBase64String(DefaultEncoding.GetBytes(args[0].stringValue)));
                } else {
                    var bytes = args[0].scriptValue.Value as byte[];
                    return new ScriptValue(Convert.ToBase64String(bytes, length > 1 ? args[1].ToInt32() : 0, length > 2 ? args[2].ToInt32() : bytes.Length));
                }
            }
        }
        private class fromBase64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(Convert.FromBase64String(args[0].ToString()));
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
