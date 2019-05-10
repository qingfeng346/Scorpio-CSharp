using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Library {
    public class LibraryIO {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public static void Load(Script script) {
            var map = script.CreateMap();
            map.SetValue("toString", script.CreateFunction(new toString()));
            map.SetValue("toBytes", script.CreateFunction(new toBytes(script)));
            script.SetGlobal("io", new ScriptValue(map));
        }
        private class toString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = args.Length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return new ScriptValue(encoding.GetString((byte[])args[0].Value));
            }
        }
        private class toBytes : ScorpioHandle {
            private Script m_Script;
            public toBytes(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var encoding = args.Length > 1 ? Encoding.GetEncoding(args[1].ToString()) : DefaultEncoding;
                return m_Script.CreateObject(encoding.GetBytes(args[0].ToString()));
            }
        }
    }
}
