
namespace Scorpio.LibraryV1 {
    public class LibraryTable {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("count", new count()),
                ("clear", new clear()),
                ("remove", new remove()),
                ("containskey", new containskey()),
                ("keys", new keys()),
                ("values", new values()),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            script.SetGlobal("table", new ScriptValue(map));
        }
        private class count : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].Get<ScriptMap>().Count());
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                args[0].Get<ScriptMap>().Clear();
                return args[0];
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 1; i < length; ++i) {
                    args[0].Get<ScriptMap>().Remove(args[i].Value);
                }
                return args[0];
            }
        }
        private class containskey : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptMap>().ContainsKey(args[0].Value) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class keys : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].Get<ScriptMap>().GetKeys());
            }
        }
        private class values : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].Get<ScriptMap>().GetValues());
            }
        }
    }
}
