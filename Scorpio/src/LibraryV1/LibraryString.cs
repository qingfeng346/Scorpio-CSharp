using System;

namespace Scorpio.LibraryV1 {
    public class LibraryString {
        public static void Load(Script script) {
            var protoString = script.TypeString;
            var functions = new (string, ScriptValue)[] {
                ("format", protoString.GetValue("format")),
                ("cs_format", protoString.GetValue("csFormat")),
                ("isnullorempty", protoString.GetValue("isNullOrEmpty")),
                ("join", protoString.GetValue("join")),

                ("length", script.CreateFunction(new length())),
                ("substring", script.CreateFunction(new substring())),
                ("tolower", script.CreateFunction(new toLower())),
                ("toupper", script.CreateFunction(new toUpper())),
                ("trim", script.CreateFunction(new trim())),
                ("replace", script.CreateFunction(new replace())),
                ("indexof", script.CreateFunction(new indexOf())),
                ("lastindexof", script.CreateFunction(new lastIndexOf())),
                ("startswith", script.CreateFunction(new startsWith())),
                ("endswith", script.CreateFunction(new endsWith())),
                ("contains", script.CreateFunction(new contains())),
                ("split", script.CreateFunction(new split(script))),
                ("at", script.CreateFunction(new at())),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, func);
            }
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
        private class toLower : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].stringValue.ToLower());
            }
        }
        private class toUpper : ScorpioHandle {
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
        private class indexOf : ScorpioHandle {
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
        private class lastIndexOf : ScorpioHandle {
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
        private class startsWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].stringValue.StartsWith(args[1].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class endsWith : ScorpioHandle {
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
