using System;

namespace Scorpio.LibraryV1 {
    public class LibraryString {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("length", new length(script)),
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
            var map = script.AddLibrary("string", functions, 4);
            var protoString = script.TypeString;
            map.SetValue("format", protoString.GetValue("format"));
            map.SetValue("cs_format", protoString.GetValue("csFormat"));
            map.SetValue("isnullorempty", protoString.GetValue("isNullOrEmpty"));
            map.SetValue("join", protoString.GetValue("join"));
        }
        private class length : ScorpioHandle {
            private Script script;
            public length(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].GetStringValue(script).Length);
            }
        }
        private class at : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].GetStringValue[args[1].ToInt32()]);
            }
        }
        private class tolower : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].GetStringValue.ToLower());
            }
        }
        private class toupper : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].GetStringValue.ToUpper());
            }
        }
        private class trim : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].GetStringValue.Trim());
            }
        }
        private class replace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].GetStringValue.Replace(args[1].ToString(), args[2].ToString()));
            }
        }
        private class indexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 4) {
                    return new ScriptValue((double)args[0].GetStringValue.IndexOf(args[1].ToString(), args[2].ToInt32(), args[3].ToInt32()));
                } else if (length == 3) {
                    return new ScriptValue((double)args[0].GetStringValue.IndexOf(args[1].ToString(), args[2].ToInt32()));
                } else {
                    return new ScriptValue((double)args[0].GetStringValue.IndexOf(args[1].ToString()));
                }
            }
        }
        private class lastindexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 4) {
                    return new ScriptValue((double)args[0].GetStringValue.LastIndexOf(args[1].ToString(), args[2].ToInt32(), args[3].ToInt32()));
                } else if (length == 3) {
                    return new ScriptValue((double)args[0].GetStringValue.LastIndexOf(args[1].ToString(), args[2].ToInt32()));
                } else {
                    return new ScriptValue((double)args[0].GetStringValue.LastIndexOf(args[1].ToString()));
                }
            }
        }
        private class startswith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].GetStringValue.StartsWith(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class endswith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].GetStringValue.EndsWith(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].GetStringValue.Contains(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class substring : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 1)
                    return args[0];
                else if (length == 2)
                    return new ScriptValue(args[0].GetStringValue.Substring(args[1].ToInt32()));
                else
                    return new ScriptValue(args[0].GetStringValue.Substring(args[1].ToInt32(), args[2].ToInt32()));
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
                var strs = args[0].GetStringValue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                var ret = new ScriptArray(m_script);
                foreach (string str in strs) {
                    ret.Add(new ScriptValue(str));
                }
                return new ScriptValue(ret);
            }
        }
    }
}
