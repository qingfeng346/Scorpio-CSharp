namespace Scorpio.LibraryV1 {
    public class LibraryArray {
        public static void Load(Script script) {
            var functions = new (string, ScorpioHandle)[] {
                ("count", new count()),
                ("insert", new insert()),
                ("add", new add()),
                ("remove", new remove()),
                ("removeat", new removeat()),
                ("clear", new clear()),
                ("contains", new contains()),
                ("sort", new sort()),
                ("indexof", new indexof()),
                ("lastindexof", new lastindexof()),
                ("first", new first()),
                ("last", new last()),
                ("pop", new popfirst()),
                ("safepop", new safepopfirst()),
                ("popfirst", new popfirst()),
                ("safepopfirst", new safepopfirst()),
                ("poplast", new poplast()),
                ("safepoplast", new safepoplast()),
            };
            var map = new ScriptMapString(script, functions.Length);
            foreach (var (name, func) in functions) {
                map.SetValue(name, script.CreateFunction(func));
            }
            script.SetGlobal("array", new ScriptValue(map));
        }
        private class count : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].Get<ScriptArray>().Length());
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = args[0].Get<ScriptArray>();
                var index = args[1].ToInt32();
                for (int i = 2; i < length; ++i) {
                    array.Insert(index, args[i]);
                }
                return args[0];
            }
        }
        private class add : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = args[0].Get<ScriptArray>();
                for (int i = 1; i < length; ++i) {
                    array.Add(args[i]);
                }
                return args[0];
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = args[0].Get<ScriptArray>();
                for (int i = 1; i < length; ++i) {
                    array.Remove(args[i]);
                }
                return args[0];
            }
        }
        private class removeat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = args[0].Get<ScriptArray>();
                array.RemoveAt(args[1].ToInt32());
                return args[0];
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                args[0].Get<ScriptArray>().Clear();
                return args[0];
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().Contains(args[1]) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class sort : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                args[0].Get<ScriptArray>().Sort(args[1].Get<ScriptFunction>());
                return args[0];
            }
        }
        private class indexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].Get<ScriptArray>().IndexOf(args[1]));
            }
        }
        private class lastindexof : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)args[0].Get<ScriptArray>().LastIndexOf(args[1]));
            }
        }
        private class first : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().First();
            }
        }
        private class last : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().Last();
            }
        }
        private class popfirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().PopFirst();
            }
        }
        private class safepopfirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().SafePopFirst();
            }
        }
        private class poplast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().PopLast();
            }
        }
        private class safepoplast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].Get<ScriptArray>().SafePopLast();
            }
        }
    }
}
