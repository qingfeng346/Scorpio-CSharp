using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Library
{
    public class LibraryString
    {
        public static void Load(Script script)
        {
            ScriptTable Table = script.CreateTable();
            Table.SetValue("format", script.CreateFunction(new format()));
            Table.SetValue("substring", script.CreateFunction(new substring()));
            Table.SetValue("length", script.CreateFunction(new length()));
            Table.SetValue("tolower", script.CreateFunction(new tolower()));
            Table.SetValue("toupper", script.CreateFunction(new toupper()));
            Table.SetValue("trim", script.CreateFunction(new trim()));
            Table.SetValue("replace", script.CreateFunction(new replace()));
            script.SetObjectInternal("string", Table);
        }
        const string DELIM_STR = "{}";
        private class format : ScorpioHandle
        {
            public object Call(ScriptObject[] args) {
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
            public object Call(ScriptObject[] args)
            {
                string messagePattern = (args[0] as ScriptString).Value;
                if (args.Length == 1) return messagePattern;
                int index = (args[1] as ScriptNumber).ToInt32();
                int length = (args[2] as ScriptNumber).ToInt32();
                return messagePattern.Substring(index, length);
            }
        }
        private class length : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return (args[0] as ScriptString).Value.Length;
            }
        }
        private class tolower : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return (args[0] as ScriptString).Value.ToLowerInvariant();
            }
        }
        private class toupper : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return (args[0] as ScriptString).Value.ToUpperInvariant();
            }
        }
        private class trim : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                return (args[0] as ScriptString).Value.Trim();
            }
        }
        private class replace : ScorpioHandle
        {
            public object Call(ScriptObject[] args)
            {
                string str = (args[0] as ScriptString).Value;
                string oldValue = (args[1] as ScriptString).Value;
                string newValue = (args[2] as ScriptString).Value;
                return str.Replace(oldValue, newValue);
            }
        }
    }
}
