namespace Scorpio.Proto {
    public class ProtoMap {
        public static ScriptType Load(Script script, ScriptType parentType) {
            var ret = script.CreateType("Map", parentType);
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("count", script.CreateFunction(new length()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("containsKey", script.CreateFunction(new containsKey()));
            ret.SetValue("containsValue", script.CreateFunction(new containsValue()));
            ret.SetValue("keys", script.CreateFunction(new keys()));
            ret.SetValue("values", script.CreateFunction(new values()));
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (double)thisObject.Get<ScriptMap>().Count();
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptMap>().Clear();
                return ScriptValue.Null;
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptMap>().Remove(args[0].Value);
                return ScriptValue.Null;
            }
        }
        private class containsKey : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptMap>().ContainsKey(args[0].Value) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class containsValue : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptMap>().ContainsValue(args[0]) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class keys : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.Get<ScriptMap>().GetKeys());
            }
        }
        private class values : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(thisObject.Get<ScriptMap>().GetValues());
            }
        }
    }
}
