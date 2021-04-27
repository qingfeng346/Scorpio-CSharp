using System;

namespace Scorpio.LibraryV1 {
    public class LibraryString {
        public static void Load(Script script) {
            var protoString = script.TypeString;
            var map = new ScriptMapString(script);
            map.SetValue("format", protoString.GetValue("format"));
            map.SetValue("cs_format", protoString.GetValue("csFormat"));
            map.SetValue("isnullorempty", protoString.GetValue("isNullOrEmpty"));
            map.SetValue("join", protoString.GetValue("join"));

            map.SetValue("length", script.CreateFunction(new length()));
            map.SetValue("substring", script.CreateFunction(new substring()));
            map.SetValue("tolower", script.CreateFunction(new toLower()));
            map.SetValue("toupper", script.CreateFunction(new toUpper()));
            map.SetValue("trim", script.CreateFunction(new trim()));
            map.SetValue("replace", script.CreateFunction(new replace()));
            map.SetValue("indexof", script.CreateFunction(new indexOf()));
            map.SetValue("lastindexof", script.CreateFunction(new lastIndexOf()));
            map.SetValue("startswith", script.CreateFunction(new startsWith()));
            map.SetValue("endswith", script.CreateFunction(new endsWith()));
            map.SetValue("contains", script.CreateFunction(new contains()));
            map.SetValue("split", script.CreateFunction(new split(script)));
            map.SetValue("at", script.CreateFunction(new at()));
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
