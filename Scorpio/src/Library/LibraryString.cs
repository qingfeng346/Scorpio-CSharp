using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library {
    public class LibraryString {
        public static void Load(Script script) {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("format", script.CreateFunction(new format()));
            Table.SetValue("cs_format", script.CreateFunction(new cs_format()));
            Table.SetValue("substring", script.CreateFunction(new substring()));
            Table.SetValue("length", script.CreateFunction(new length()));
            Table.SetValue("tolower", script.CreateFunction(new tolower()));
            Table.SetValue("toupper", script.CreateFunction(new toupper()));
            Table.SetValue("trim", script.CreateFunction(new trim()));
            Table.SetValue("replace", script.CreateFunction(new replace()));
            Table.SetValue("isnullorempty", script.CreateFunction(new isnullorempty()));
            Table.SetValue("indexof", script.CreateFunction(new indexof()));
            Table.SetValue("lastindexof", script.CreateFunction(new lastindexof()));
            Table.SetValue("startswith", script.CreateFunction(new startswith()));
            Table.SetValue("endswith", script.CreateFunction(new endswith()));
            Table.SetValue("contains", script.CreateFunction(new contains()));
            Table.SetValue("split", script.CreateFunction(new split(script)));
            Table.SetValue("join", script.CreateFunction(new join()));
            Table.SetValue("at", script.CreateFunction(new at()));
            Table.SetValue("char2ascii", script.CreateFunction(new char2ascii(script)));
            Table.SetValue("ascii2char", script.CreateFunction(new ascii2char()));
            script.SetObjectInternal("string", Table);
        }
        const string DELIM_STR = "{}";
        private class format : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                if (args == null || args.Length == 0) return null;
                string messagePattern = (args[0] as ScriptString).Value;
                if (args.Length == 1) return messagePattern;
                StringBuilder sbuf = new StringBuilder();
                int L;
                if (args[1] is ScriptArray) {
                    L = 0;
                    args = ((ScriptArray)args[1]).ToArray();
                } else {
                    L = 1;
                }
                int length = args.Length;
                int i = 0, j = 0;
                for (; L < length; L++) {
                    j = messagePattern.IndexOf(DELIM_STR, i);
                    if (j == -1) {
                        if (i == 0) {
                            return messagePattern;
                        } else {
                            sbuf.Append(messagePattern.Substring(i));
                            return sbuf.ToString();
                        }
                    } else {
                        sbuf.Append(messagePattern.Substring(i, j - i));
                        sbuf.Append(args[L].ToString());
                        i = j + 2;
                    }
                }
                sbuf.Append(messagePattern.Substring(i));
                return sbuf.ToString();
            }
        }
        private class cs_format : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                if (args == null || args.Length == 0) return null;
                if (args.Length == 1) { return args[0].ToString(); }
                int L;
                if (args[1] is ScriptArray) {
                    L = 0;
                    args = ((ScriptArray)args[1]).ToArray();
                } else {
                    L = 1;
                }
                var length = args.Length;
                var objs = new object[length - 1];
                for (var i = L; i < length; ++i) {
                    objs[i - L] = args[i].ObjectValue;
                }
                return string.Format(args[0].ToString(), objs);
            }
        }
        private class substring : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string messagePattern = (args[0] as ScriptString).Value;
                if (args.Length == 1) return messagePattern;
                if (args.Length == 3)
                    return messagePattern.Substring((args[1] as ScriptNumber).ToInt32(), (args[2] as ScriptNumber).ToInt32());
                else
                    return messagePattern.Substring((args[1] as ScriptNumber).ToInt32());
            }
        }
        private class length : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.Length;
            }
        }
        private class tolower : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.ToLower();
            }
        }
        private class toupper : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.ToUpper();
            }
        }
        private class trim : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.Trim();
            }
        }
        private class replace : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                string oldValue = (args[1] as ScriptString).Value;
                string newValue = (args[2] as ScriptString).Value;
                return str.Replace(oldValue, newValue);
            }
        }
        private class isnullorempty : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return Util.IsNullOrEmpty(args[0].ObjectValue as string);
            }
        }
        private class indexof : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                string value = (args[1] as ScriptString).Value;
                if (args.Length == 3)
                    return str.IndexOf(value, (args[2] as ScriptNumber).ToInt32());
                else
                    return str.IndexOf(value);
            }
        }
        private class lastindexof : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                string value = (args[1] as ScriptString).Value;
                if (args.Length == 3)
                    return str.LastIndexOf(value, (args[2] as ScriptNumber).ToInt32());
                else
                    return str.LastIndexOf(value);
            }
        }
        private class startswith : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.StartsWith((args[1] as ScriptString).Value);
            }
        }
        private class endswith : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.EndsWith((args[1] as ScriptString).Value);
            }
        }
        private class contains : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                return (args[0] as ScriptString).Value.Contains((args[1] as ScriptString).Value);
            }
        }
        private class split : ScorpioHandle {
            private Script m_script;
            public split(Script script) {
                this.m_script = script;
            }
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                string tko = (args[1] as ScriptString).Value;
                string[] strs = str.Split(tko.ToCharArray());
                ScriptArray ret = m_script.CreateArray();
                foreach (string s in strs) {
                    ret.Add(m_script.CreateString(s));
                }
                return ret;
            }
        }
        private class join : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string separator = (args[0] as ScriptString).Value;
                ScriptArray value = (args[1] as ScriptArray);
                var count = value.Count();
                string[] values = new string[count];
                for (int i = 0; i < count; ++i) { values[i] = value.GetValue(i).ToString(); }
                return Util.Join(separator, values);
            }
        }
        private class at : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                int index = (args[1] as ScriptNumber).ToInt32();
                return (int)str[index];
            }
        }
        private class char2ascii : ScorpioHandle {
            private Script m_script;
            public char2ascii(Script script) {
                this.m_script = script;
            }
            public object Call(ScriptObject[] args) {
                string str = (args[0] as ScriptString).Value;
                int length = str.Length;
                if (length == 0) {
                    return null;
                } else if (length == 1) {
                    return (int)str[0];
                } else {
                    ScriptArray array = m_script.CreateArray();
                    for (int i = 0; i < length; ++i) {
                        array.Add(m_script.CreateObject((int)str[i]));
                    }
                    return array;
                }
            }
        }
        private class ascii2char : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                ScriptNumber num = args[0] as ScriptNumber;
                if (num != null) {
                    return new string(new char[] { (char)num.ToInt32() });
                } else {
                    ScriptArray array = args[0] as ScriptArray;
                    char[] chars = new char[array.Count()];
                    for (int i = 0;i<array.Count();++i) {
                        chars[i] = (char)((array.GetValue(i) as ScriptNumber).ToInt32());
                    }
                    return new string(chars);
                }
            }
        }
    }
}
