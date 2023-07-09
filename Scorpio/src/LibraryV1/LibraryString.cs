using System;

namespace Scorpio.LibraryV1 {
    public class LibraryString {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("length", new length()),
                ("substring", new substring()),
                ("tolower", new tolower()),
                ("toupper", new toupper()),
                ("trim", new trim()),
                ("replace", new replace()),
                ("indexof", new indexof()),
                ("lastindexof", new lastindexof()),
                ("startswith", new startswith()),
                ("endswith", new endswith()),
                ("contains", new contains()),
                ("split", new split(script)),
                ("at", new at()),
            };
            var map = new ScriptMapStringPolling(script, functions.Length + 4);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            var protoString = script.TypeString;
            map.SetValue("format", protoString.GetValue("format"));
            map.SetValue("cs_format", protoString.GetValue("csFormat"));
            map.SetValue("isnullorempty", protoString.GetValue("isNullOrEmpty"));
            map.SetValue("join", protoString.GetValue("join"));
            script.SetGlobal("string", new ScriptValue(map));
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].stringValue.Length);
            }
        }
        private class at : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].stringValue[args[1].ToInt32()]);
            }
        }
        private class tolower : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].stringValue.ToLower());
            }
        }
        private class toupper : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].stringValue.ToUpper());
            }
        }
        private class trim : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].stringValue.Trim());
            }
        }
        private class replace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].stringValue.Replace(args[1].ToString(), args[2].ToString()));
            }
        }
        private class indexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 4) {
                    return new ScriptValue((double)args[0].stringValue.IndexOf(args[1].ToString(), args[2].ToInt32(), args[3].ToInt32()));
                } else if (length == 3) {
                    return new ScriptValue((double)args[0].stringValue.IndexOf(args[1].ToString(), args[2].ToInt32()));
                } else {
                    return new ScriptValue((double)args[0].stringValue.IndexOf(args[1].ToString()));
                }
            }
        }
        private class lastindexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 4) {
                    return new ScriptValue((double)args[0].stringValue.LastIndexOf(args[1].ToString(), args[2].ToInt32(), args[3].ToInt32()));
                } else if (length == 3) {
                    return new ScriptValue((double)args[0].stringValue.LastIndexOf(args[1].ToString(), args[2].ToInt32()));
                } else {
                    return new ScriptValue((double)args[0].stringValue.LastIndexOf(args[1].ToString()));
                }
            }
        }
        private class startswith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].stringValue.StartsWith(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class endswith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].stringValue.EndsWith(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].stringValue.Contains(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class substring : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 1)
                    return args[0];
                else if (length == 2)
                    return new ScriptValue(args[0].stringValue.Substring(args[1].ToInt32()));
                else
                    return new ScriptValue(args[0].stringValue.Substring(args[1].ToInt32(), args[2].ToInt32()));
            }
        }
        private class split : ScorpioHandle {
            private Script m_script;
            public split(Script script) {
                this.m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var splits = new string[length];
                for (var i = 1; i < length; ++i) { splits[i] = args[i].ToString(); }
                var strs = args[0].stringValue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                var ret = new ScriptArray(m_script);
                foreach (string str in strs) {
                    ret.Add(new ScriptValue(str));
                }
                return new ScriptValue(ret);
            }
        }
    }
}
