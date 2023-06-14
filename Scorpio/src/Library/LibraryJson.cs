namespace Scorpio.Library {
    public partial class LibraryJson {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("encode", new encode(script)),
                ("decode", new decode(script)),
            };
            var map = script.NewMapString();
            map.SetCapacity(functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            script.SetGlobal("json", map);
        }
        private class encode : ScorpioHandle {
            private readonly Script m_Script;
            public encode(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(m_Script.ToJson(args[0]));
            }
        }
        private class decode : ScorpioHandle {
            private readonly Script m_Script;
            public decode(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return m_Script.ParseJson(args[0].ToString(), length > 1 ? args[1].IsTrue : true);
            }
        }
    }
}
