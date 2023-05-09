namespace Scorpio.Library {
    public partial class LibraryJson {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("encode", new encode()),
                ("decode", new decode(script)),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            script.SetGlobal("json", new ScriptValue(map));
        }
        private class encode : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                using (var serializer = new ScorpioJsonSerializer()) {
                    return new ScriptValue(serializer.ToJson(args[0]));
                }
            }
        }
        private class decode : ScorpioHandle {
            private readonly Script m_Script;
            public decode(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                using (var deserializer = new ScorpioJsonDeserializer(m_Script, args[0].ToString(), length > 1 ? args[1].IsTrue : true)) {
                    return deserializer.Parse();
                }
            }
        }
    }
}
