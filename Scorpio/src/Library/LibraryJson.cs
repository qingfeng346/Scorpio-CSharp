namespace Scorpio.Library {
    public partial class LibraryJson {
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("encode", script.CreateFunction(new encode()));
            map.SetValue("decode", script.CreateFunction(new decode(script)));
            script.SetGlobal("json", map);
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
