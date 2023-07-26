namespace Scorpio.Proto {
    public class ProtoMap {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("length", new length()),
                ("count", new length()),
                ("clear", new clear()),
                ("remove", new remove()),
                ("containsKey", new containsKey()),
                ("containsValue", new containsValue()),
                ("keys", new keys()),
                ("values", new values()),
                ("forEach", new forEach(script)),
                ("forEachValue", new forEachValue(script)),
                ("find", new find(script)),
                ("findKey", new find(script)),
                ("findValue", new findValue(script)),
                ("+", new plus()),
                ("-", new minus()),
            };
            ret.SetFunctions(script, functions);
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
                    thisObject.Get<ScriptMap>().Remove(args[i].GetValue);
                }
                return thisObject.Reference();
            }
        }
        private class containsKey : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptMap>().ContainsKey(args[0].GetValue) ? ScriptValue.True : ScriptValue.False;
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
                    var key = ScriptValue.CreateValueNoReference(script, pair.Key);
                    if (func.Call(key, pair.Value).valueType == ScriptValue.falseValueType) {
                        return key.Reference();
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
                    var key = ScriptValue.CreateValueNoReference(script, pair.Key);
                    if (func.Call(pair.Value, key).valueType == ScriptValue.falseValueType) {
                        return key.Reference();
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
                    var key = ScriptValue.CreateValueNoReference(script, pair.Key);
                    if (func.Call(key, pair.Value).IsTrue) {
                        return key.Reference();
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
                    if (func.Call(ScriptValue.CreateValueNoReference(script, pair.Key), pair.Value).IsTrue) {
                        return pair.Value.Reference();
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
                return new ScriptValue(thisMap);
            }
        }
        private class minus : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisMap = thisObject.Get<ScriptMap>().NewCopy();
                var array = args[0].Get<ScriptArray>();
                if (array != null) {
                    foreach (var value in array) {
                        thisMap.Remove(value.GetValue);
                    }
                } else {
                    thisMap.Remove(args[0].GetValue);
                }
                return new ScriptValue(thisMap);
            }
        }
    }
}
