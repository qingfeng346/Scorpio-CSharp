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
                ("forEach", new forEach()),
                ("forEachValue", new forEachValue()),
                ("find", new find()),
                ("findKey", new find()),
                ("findValue", new findValue()),
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
                return thisObject;
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    thisObject.Get<ScriptMap>().Remove(args[i].Value);
                }
                return thisObject;
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
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    if (func.Call(ScriptValue.CreateValue(pair.Key), pair.Value).valueType == ScriptValue.falseValueType) {
                        return ScriptValue.CreateValue(pair.Key);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class forEachValue : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    if (func.Call(pair.Value, ScriptValue.CreateValue(pair.Key)).valueType == ScriptValue.falseValueType) {
                        return ScriptValue.CreateValue(pair.Key);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class find : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    if (func.Call(ScriptValue.CreateValue(pair.Key), pair.Value).IsTrue) {
                        return ScriptValue.CreateValue(pair.Key);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class findValue : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                foreach (var pair in thisObject.Get<ScriptMap>()) {
                    if (func.Call(ScriptValue.CreateValue(pair.Key), pair.Value).IsTrue) {
                        return pair.Value;
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
                        thisMap.Remove(value.Value);
                    }
                } else {
                    thisMap.Remove(args[0].Value);
                }
                return new ScriptValue(thisMap);
            }
        }
    }
}
