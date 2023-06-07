namespace Scorpio.Proto {
    public class ProtoMap {
        public static ScriptType Load(Script script, ScriptType ret) {
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("count", script.CreateFunction(new length()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("containsKey", script.CreateFunction(new containsKey()));
            ret.SetValue("containsValue", script.CreateFunction(new containsValue()));
            ret.SetValue("keys", script.CreateFunction(new keys()));
            ret.SetValue("values", script.CreateFunction(new values()));
            ret.SetValue("forEach", script.CreateFunction(new forEach(script)));
            ret.SetValue("forEachValue", script.CreateFunction(new forEachValue(script)));
            ret.SetValue("find", script.CreateFunction(new find(script)));
            ret.SetValue("findValue", script.CreateFunction(new findValue(script)));
            ret.SetValue("+", script.CreateFunction(new plus()));
            ret.SetValue("-", script.CreateFunction(new minus()));
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptMap>().Count());
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptMap>().Clear();
                return thisObject.Reference();
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    thisObject.Get<ScriptMap>().Remove(args[i].Value);
                }
                return thisObject.Reference();
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
        private class forEach : ScorpioHandle {
            private Script script;
            public forEach(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    using (var key = ScriptValue.CreateValue(script, pair.Key)) {
                        if (func.Call(key, pair.Value).valueType == ScriptValue.falseValueType) {
                            return key.Reference();
                        }
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class forEachValue : ScorpioHandle {
            private Script script;
            public forEachValue(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    using (var key = ScriptValue.CreateValue(script, pair.Key)) {
                        if (func.Call(pair.Value, key).valueType == ScriptValue.falseValueType) {
                            return key.Reference();
                        }
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class find : ScorpioHandle {
            private Script script;
            public find(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    using (var key = ScriptValue.CreateValue(script, pair.Key)) {
                        if (func.Call(key, pair.Value).IsTrue) {
                            return key.Reference();
                        }
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class findValue : ScorpioHandle {
            private Script script;
            public findValue(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    using (var key = ScriptValue.CreateValue(script, pair.Key)) {
                        if (func.Call(key, pair.Value).IsTrue) {
                            return new ScriptValue(pair.Value);
                        }
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class plus : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisMap = thisObject.Get<ScriptMap>().NewCopy();
                var map = args[0].Get<ScriptMap>();
                foreach (var pair in map) {
                    thisMap.SetValue(pair.Key, pair.Value);
                }
                using var ret = new ScriptValue(thisMap);
                return ret;
            }
        }
        private class minus : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisMap = thisObject.Get<ScriptMap>().NewCopy();
                var array = args[0].Get<ScriptArray>();
                if (array != null) {
                    foreach (var value in array) {
                        thisMap.Remove(value.Value);
                    }
                } else {
                    thisMap.Remove(args[0].Value);
                }
                using var ret = new ScriptValue(thisMap);
                return ret;
            }
        }
    }
}
