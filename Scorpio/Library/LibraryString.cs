using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryString
    {
        public static ScriptTable Table = new ScriptTable();
        public static void Load(Script script)
        {
            script.SetObject("string", Table);
        }
        static LibraryString()
        {
            Table.SetValue("format", new ScriptFunction(new format()));
            Table.SetValue("substring", new ScriptFunction(new substring()));
        }
        const string DELIM_STR = "{}";
        private class format : ScorpioHandle
        {
            public object run(object[] args) {
                if (args == null || args.Length == 0) {
                    return null;
                }
                string messagePattern = (args[0] as ScriptString).Value;
                if (args.Length == 1)
                    return messagePattern;
                int i = 0;
                int j;
                StringBuilder sbuf = new StringBuilder();
                int length = args.Length;
                int L;
                for (L = 1; L < length; L++) {
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
        private class substring : ScorpioHandle
        {
            public object run(object[] args) {
                if (args == null || args.Length == 0) {
                    return null;
                }
                string messagePattern = (args[0] as ScriptString).Value;
                if (args.Length == 1)
                    return messagePattern;
                int index = (args[1] as ScriptNumber).ToInt32();
                int length = (args[2] as ScriptNumber).ToInt32();
                return messagePattern.Substring(index, length);
            }
        }
    }
}
