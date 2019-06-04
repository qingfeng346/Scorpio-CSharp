namespace Scorpio.Proto {
    public class ProtoArray {
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("Array", parentType);
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("count", script.CreateFunction(new length()));
            ret.SetValue("insert", script.CreateFunction(new insert()));
            ret.SetValue("add", script.CreateFunction(new add()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("removeAt", script.CreateFunction(new removeAt()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("contains", script.CreateFunction(new contains()));
            ret.SetValue("sort", script.CreateFunction(new sort()));
            ret.SetValue("indexOf", script.CreateFunction(new indexOf()));
            ret.SetValue("lastIndexOf", script.CreateFunction(new lastIndexOf()));
            ret.SetValue("first", script.CreateFunction(new first()));
            ret.SetValue("last", script.CreateFunction(new last()));
            ret.SetValue("popFirst", script.CreateFunction(new popFirst()));
            ret.SetValue("safePopFirst", script.CreateFunction(new safePopFirst()));
            ret.SetValue("popLast", script.CreateFunction(new popLast()));
            ret.SetValue("safePopLast", script.CreateFunction(new safePopLast()));
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (double)thisObject.Get<ScriptArray>().Length();
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var index = args[0].ToInt32();
                for (int i = 1; i < length; ++i) {
                    array.Insert(index, args[i]);
                }
                return thisObject;
            }
        }
        private class add : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.Add(args[i]);
                }
                return thisObject;
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.Remove(args[i]);
                }
                return thisObject;
            }
        }
        private class removeAt : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                array.RemoveAt(args[0].ToInt32());
                return thisObject;
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptArray>().Clear();
                return ScriptValue.Null;
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().Contains(args[0]) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class sort : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptArray>().Sort(args[0].Get<ScriptFunction>());
                return thisObject;
            }
        }
        private class indexOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptArray>().IndexOf(args[0]));
            }
        }
        private class lastIndexOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptArray>().LastIndexOf(args[0]));
            }
        }
        private class first : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().First();
            }
        }
        private class last : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().Last();
            }
        }
        private class popFirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().PopFirst();
            }
        }
        private class safePopFirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().SafePopFirst();
            }
        }
        private class popLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().PopLast();
            }
        }
        private class safePopLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().SafePopLast();
            }
        }
    }
}
