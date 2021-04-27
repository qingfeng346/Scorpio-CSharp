namespace Scorpio.LibraryV1 {
    public class LibraryArray {
        public static void Load(Script script) {
            var map = new ScriptMapString(script);
            map.SetValue("count", script.CreateFunction(new count()));
            map.SetValue("insert", script.CreateFunction(new insert()));
            map.SetValue("add", script.CreateFunction(new add()));
            map.SetValue("remove", script.CreateFunction(new remove()));
            map.SetValue("removeat", script.CreateFunction(new removeat()));
            map.SetValue("clear", script.CreateFunction(new clear()));
            map.SetValue("contains", script.CreateFunction(new contains()));
            map.SetValue("sort", script.CreateFunction(new sort()));
            map.SetValue("indexof", script.CreateFunction(new indexof()));
            map.SetValue("lastindexof", script.CreateFunction(new lastindexof()));
            map.SetValue("first", script.CreateFunction(new first()));
            map.SetValue("last", script.CreateFunction(new last()));
            map.SetValue("pop", script.CreateFunction(new popfirst()));
            map.SetValue("safepop", script.CreateFunction(new safepopfirst()));
            map.SetValue("popfirst", script.CreateFunction(new popfirst()));
            map.SetValue("safepopfirst", script.CreateFunction(new safepopfirst()));
            map.SetValue("poplast", script.CreateFunction(new poplast()));
            map.SetValue("safepoplast", script.CreateFunction(new safepoplast()));
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
