namespace Scorpio.Library {
    public class LibraryType {
        public static void Load(Script script) {
            script.SetValueNoReference("StringMap", script.CreateFunction(new StringMap(script)));
            //script.SetGlobal("PollingMap", script.CreateFunction(new PollingMap(script)));
        }
        private class StringMap : ScorpioHandle {
            private Script m_Script;
            internal StringMap(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(m_Script.NewMapString());
            }
        }
        //private class PollingMap : ScorpioHandle {
        //    private Script m_Script;
        //    internal PollingMap(Script script) {
        //        m_Script = script;
        //    }
        //    public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
        //        using (var value = new ScriptValue(new ScriptMapPolling(m_Script, length > 0 ? args[0].ToInt32() : 0, length > 1 ? args[1].IsTrue : false))) {
        //            return value;
        //        }
        //    }
        //}
    }
}
