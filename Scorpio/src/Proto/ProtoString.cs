using System;
using System.Text;

namespace Scorpio.Proto {
    public class ProtoString {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("format", new format()),
                ("csFormat", new csFormat()),
                ("isNullOrEmpty", new isNullOrEmpty()),
                ("join", new join()),
                ("fromCharCode", new fromCharCode()),
                ("toCharCode", new toCharCode(script)),
                ("isInterned", new isInterned()),
                ("intern", new intern()),
                ("length", new length()),
                ("count", new length()),
                ("at", new at()),
                ("charCodeAt", new at()),
                ("insert", new insert()),
                ("remove", new remove()),
                ("toLower", new toLower()),
                ("toUpper", new toUpper()),
                ("toOneLower", new toOneLower()),
                ("toOneUpper", new toOneUpper()),
                ("trim", new trim()),
                ("trimStart", new trimStart()),
                ("trimEnd", new trimEnd()),
                ("replace", new replace()),
                ("indexOf", new indexOf()),
                ("lastIndexOf", new lastIndexOf()),
                ("indexOfChar", new indexOfChar()),
                ("lastIndexOfChar", new lastIndexOfChar()),
                ("startsWith", new startsWith()),
                ("endsWith", new endsWith()),
                ("contains", new contains()),
                ("padLeft", new padLeft()),
                ("padRight", new padRight()),
                ("compareTo", new compareTo()),
                ("sub", new sub()),
                ("split", new split(script)),
            };
            ret.SetFunctions(script, functions);
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.stringValue.Length);
            }
        }
        private class at : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var isCode = length > 1 ? args[1].IsTrue : true;
                if (isCode) {
                    return new ScriptValue(thisObject.stringValue[args[0].ToInt32()]);
                } else {
                    return new ScriptValue(thisObject.stringValue[args[0].ToInt32()].ToString());
                }
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 2) {
                    return new ScriptValue(thisObject.stringValue.Insert(args[0].ToInt32(), args[1].ToString()));
                }
                return thisObject;
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 1) {
                    return new ScriptValue(thisObject.stringValue.Remove(args[0].ToInt32()));
                } else if (length == 2) {
                    return new ScriptValue(thisObject.stringValue.Remove(args[0].ToInt32(), args[0].ToInt32()));
                }
                return thisObject;
            }
        }
        private class toLower : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.ToLower());
            }
        }
        private class toUpper : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.ToUpper());
            }
        }
        private class toOneLower : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var str = thisObject.stringValue;
                if (str.Length <= 1) return new ScriptValue(thisObject.stringValue.ToLower());
                return new ScriptValue(char.ToLower(str[0]) + str.Substring(1));
            }
        }
        private class toOneUpper : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var str = thisObject.stringValue;
                if (str.Length <= 1) return new ScriptValue(thisObject.stringValue.ToUpper());
                return new ScriptValue(char.ToUpper(str[0]) + str.Substring(1));
            }
        }
        private class trim : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.Trim());
            }
        }
        private class trimStart : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.TrimStart());
            }
        }
        private class trimEnd : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.TrimEnd());
            }
        }
        private class replace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.stringValue.Replace(args[0].ToString(), args[1].ToString()));
            }
        }
        private class indexOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 3) {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToString(), args[1].ToInt32(), args[2].ToInt32()));
                } else if (length == 2) {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToString(), args[1].ToInt32()));
                } else {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToString()));
                }
            }
        }
        private class lastIndexOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 3) {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToString(), args[1].ToInt32(), args[2].ToInt32()));
                } else if (length == 2) {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToString(), args[1].ToInt32()));
                } else {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToString()));
                }
            }
        }
        private class indexOfChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 3) {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToChar(), args[1].ToInt32(), args[2].ToInt32()));
                } else if (length == 2) {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToChar(), args[1].ToInt32()));
                } else {
                    return new ScriptValue((double)thisObject.stringValue.IndexOf(args[0].ToChar()));
                }
            }
        }
        private class lastIndexOfChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 3) {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToChar(), args[1].ToInt32(), args[2].ToInt32()));
                } else if (length == 2) {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToChar(), args[1].ToInt32()));
                } else {
                    return new ScriptValue((double)thisObject.stringValue.LastIndexOf(args[0].ToChar()));
                }
            }
        }
        private class startsWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.stringValue.StartsWith(args[0].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class endsWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.stringValue.EndsWith(args[0].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.stringValue.Contains(args[0].ToString()) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class padLeft : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 2) {
                    return new ScriptValue(thisObject.stringValue.PadLeft(args[0].ToInt32(), args[1].ToChar()));
                } else {
                    return new ScriptValue(thisObject.stringValue.PadLeft(args[0].ToInt32()));
                }
            }
        }
        private class padRight : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 2) {
                    return new ScriptValue(thisObject.stringValue.PadRight(args[0].ToInt32(), args[1].ToChar()));
                } else {
                    return new ScriptValue(thisObject.stringValue.PadRight(args[0].ToInt32()));
                }
            }
        }
        private class compareTo : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.stringValue.CompareTo(args[0].Value));
            }
        }
        private class sub : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0)
                    return thisObject;
                else if (length == 1)
                    return new ScriptValue(thisObject.ToString().Substring(args[0].ToInt32()));
                else
                    return new ScriptValue(thisObject.ToString().Substring(args[0].ToInt32(), args[1].ToInt32()));
            }
        }
        private class split : ScorpioHandle {
            private Script m_script;
            public split(Script script) {
                this.m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var splits = new string[length];
                for (var i = 0; i < length; ++i) { splits[i] = args[i].ToString(); }
                var strs = thisObject.stringValue.Split(splits, StringSplitOptions.RemoveEmptyEntries);
                var ret = new ScriptArray(m_script);
                foreach (string str in strs) {
                    ret.Add(new ScriptValue(str));
                }
                return new ScriptValue(ret);
            }
        }

        public class format : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var format = args[0].ToString();
                var index = 1;
                var strLength = format.Length;
                var strLength1 = strLength - 1;
                var builder = new StringBuilder();
                for (var i = 0; i < strLength;) {
                    var c = format[i];
                    if (c == '{' && i < strLength1 && format[i + 1] == '}') {
                        i += 2;
                        builder.Append(args[index++]);
                    } else {
                        builder.Append(c);
                        ++i;
                    }
                }
                return new ScriptValue(builder.ToString());
            }
        }
        public class csFormat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0) return ScriptValue.Null;
                var format = args[0].ToString();
                if (length == 1) return new ScriptValue(format);
                var objs = new object[length - 1];
                for (var i = 1; i < length; ++i) {
                    objs[i - 1] = args[i].Value;
                }
                return new ScriptValue(string.Format(format, objs));
            }
        }
        private class isNullOrEmpty : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.nullValueType: return ScriptValue.True;
                    case ScriptValue.stringValueType: return args[0].stringValue.Length == 0 ? ScriptValue.True : ScriptValue.False;
                    default: return ScriptValue.False;
                }
            }
        }
        private class join : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var separator = args[0].ToString();
                var value = args[1].Get<ScriptArray>();
                var count = value.Length();
                var values = new string[count];
                for (int i = 0; i < count; ++i) { values[i] = value[i].ToString(); }
                return new ScriptValue(string.Join(separator, values));
            }
        }
        private class fromCharCode : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var values = new char[length];
                for (int i = 0; i < length; ++i) { values[i] = args[i].ToChar(); }
                return new ScriptValue(new string(values));
            }
        }
        private class toCharCode : ScorpioHandle {
            private Script m_script;
            public toCharCode(Script script) {
                this.m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var str = args[0].ToString();
                var isArray = length > 1 ? args[1].IsTrue : false;
                if (isArray) {
                    var ret = new ScriptArray(m_script);
                    foreach (var c in str) {
                        ret.Add(new ScriptValue(c));
                    }
                    return new ScriptValue(ret);
                } else {
                    return new ScriptValue(str[0]);
                }
            }
        }
        private class isInterned : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(string.IsInterned(args[0].ToString()));
            }
        }
        private class intern : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(string.Intern(args[0].ToString()));
            }
        }
    }
}
