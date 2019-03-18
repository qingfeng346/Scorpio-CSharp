using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Library {
    public class LibraryIO {
        public static void Load(Script script) {
            var Table = script.CreateTable();
            Table.SetValue("tostring", script.CreateFunction(new tostring()));
            Table.SetValue("tobytes", script.CreateFunction(new tobytes()));
            script.SetObject("io", Table);
        }
        private class tostring : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                var bytes = args[0].ObjectValue;
                var codepage = args.Length > 1 ? "utf8" : args[1].ToString();
                return Encoding.GetEncoding(codepage).GetString((byte[])bytes);
            }
        }
        private class tobytes : ScorpioHandle {
            public object Call(ScriptObject[] args) {
                var str = args[0].ToString();
                var codepage = args.Length > 1 ? "utf8" : args[1].ToString();
                return Encoding.GetEncoding(codepage).GetBytes(str);
            }
        }
    }
}
