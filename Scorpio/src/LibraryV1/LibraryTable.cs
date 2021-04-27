
namespace Scorpio.LibraryV1 {
    public class LibraryTable {
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("count", script.CreateFunction(new count()));
            map.SetValue("clear", script.CreateFunction(new clear()));
            map.SetValue("remove", script.CreateFunction(new remove()));
            map.SetValue("containskey", script.CreateFunction(new containskey()));
            map.SetValue("keys", script.CreateFunction(new keys()));
            map.SetValue("values", script.CreateFunction(new values()));
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
