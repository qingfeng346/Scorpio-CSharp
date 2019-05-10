using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Proto {
    public class ProtoString {
        const string DELIM_STR = "{}";
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("String", parentType);
            ret.SetValue("format", script.CreateFunction(new format()));
            ret.SetValue("csFormat", script.CreateFunction(new csFormat()));
            ret.SetValue("isNullOrEmpty", script.CreateFunction(new isnullorempty()));
            ret.SetValue("join", script.CreateFunction(new join()));

            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("count", script.CreateFunction(new length()));
            ret.SetValue("at", script.CreateFunction(new at()));
            ret.SetValue("toLower", script.CreateFunction(new toLower()));
            ret.SetValue("toUpper", script.CreateFunction(new toUpper()));
            ret.SetValue("trim", script.CreateFunction(new trim()));
            ret.SetValue("trimStart", script.CreateFunction(new trimStart()));
            ret.SetValue("trimEnd", script.CreateFunction(new trimEnd()));
            ret.SetValue("replace", script.CreateFunction(new replace()));
            ret.SetValue("indexOf", script.CreateFunction(new indexOf()));
            ret.SetValue("lastIndexOf", script.CreateFunction(new lastIndexOf()));
            ret.SetValue("startsWith", script.CreateFunction(new startsWith()));
            ret.SetValue("endsWith", script.CreateFunction(new endsWith()));
            ret.SetValue("contains", script.CreateFunction(new contains()));
            ret.SetValue("split", script.CreateFunction(new split(script)));
            ret.SetValue("splitArray", script.CreateFunction(new splitArray(script)));
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (double)thisObject.stringValue.Length;
            }
        }
        private class at : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (double)thisObject.stringValue[args[0].ToInt32()];
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
                    return (double)thisObject.stringValue.IndexOf(args[0].ToString(), args[1].ToInt32(), args[2].ToInt32());
                } else if (length == 2) {
                    return (double)thisObject.stringValue.IndexOf(args[0].ToString(), args[1].ToInt32());
                } else {
                    return (double)thisObject.stringValue.IndexOf(args[0].ToString());
                }
            }
        }
        private class lastIndexOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 3) {
                    return (double)thisObject.stringValue.LastIndexOf(args[0].ToString(), args[1].ToInt32(), args[2].ToInt32());
                } else if (length == 2) {
                    return (double)thisObject.stringValue.LastIndexOf(args[0].ToString(), args[1].ToInt32());
                } else {
                    return (double)thisObject.stringValue.LastIndexOf(args[0].ToString());
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
        private class split : ScorpioHandle {
            private Script m_script;
            public split(Script script) {
                this.m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var removeEmpty = length > 1 ? args[1].valueType == ScriptValue.trueValueType : true;
                var strs = thisObject.stringValue.Split(new string[] { args[0].ToString() }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
                var ret = m_script.CreateArray();
                foreach (string str in strs) {
                    ret.Add(new ScriptValue(str));
                }
                return new ScriptValue(ret);
            }
        }
        private class splitArray : ScorpioHandle {
            private Script m_script;
            public splitArray(Script script) {
                this.m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var removeEmpty = length > 1 ? args[1].valueType == ScriptValue.trueValueType : true;
                var strs = thisObject.stringValue.Split(args[0].Get<ScriptArray>().ToArray<string>(), removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
                var ret = m_script.CreateArray();
                foreach (string str in strs) {
                    ret.Add(new ScriptValue(str));
                }
                return new ScriptValue(ret);
            }
        }

        public class format : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (length == 0) return ScriptValue.Null;
                var format = args[0].ToString();
                if (length == 1) return new ScriptValue(format);
                var builder = new StringBuilder();
                var startIndex = 0;
                for (var i = 1; i < length; ++i) {
                    var index = format.IndexOf(DELIM_STR, startIndex);
                    if (index >= 0) {
                        builder.Append(format.Substring(startIndex, index - startIndex));
                        builder.Append(args[i].ToString());
                        startIndex = index + 2;
                    } else {
                        builder.Append(format.Substring(startIndex));
                        return new ScriptValue(builder.ToString());
                    }
                }
                builder.Append(format.Substring(startIndex));
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
        private class isnullorempty : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.stringValueType) { return true; }
                return string.IsNullOrEmpty(args[0].stringValue) ? ScriptValue.True : ScriptValue.False;
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
    }
}
