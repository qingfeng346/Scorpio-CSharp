using Scorpio.Tools;

namespace Scorpio.Library {
    public class LibraryType {
        public static void Load(Script script) {
            script.SetGlobal("StringMap", script.CreateFunction(new StringMap(script)));
            script.SetGlobal("PollingMap", script.CreateFunction(new PollingMap(script)));
        }
        private class StringMap : ScorpioHandle {
            private Script m_Script;
            internal StringMap(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(new ScriptMapString(m_Script, args.GetArgsThrow(0, length).ToInt32()));
            }
        }
        private class PollingMap : ScorpioHandle {
            private Script m_Script;
            internal PollingMap(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(new ScriptMapPolling(m_Script, args.GetArgsThrow(0, length).ToInt32()));
            }
        }
    }
}
