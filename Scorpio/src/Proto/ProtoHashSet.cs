using System.Collections.Generic;
namespace Scorpio.Proto {
    public class ProtoHashSet {
        public static ScriptType Load(Script script, ScriptType ret) {
            ret.SetValue("add", script.CreateFunction(new add()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("removeWhere", script.CreateFunction(new removeWhere()));
            ret.SetValue("contains", script.CreateFunction(new contains()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("unionWith", script.CreateFunction(new unionWith()));
            ret.SetValue("exceptWith", script.CreateFunction(new exceptWith()));
            ret.SetValue("intersectWith", script.CreateFunction(new intersectWith()));
            ret.SetValue("isProperSubsetOf", script.CreateFunction(new isProperSubsetOf()));
            ret.SetValue("isProperSupersetOf", script.CreateFunction(new isProperSupersetOf()));
            ret.SetValue("isSubsetOf", script.CreateFunction(new isSubsetOf()));
            ret.SetValue("isSupersetOf", script.CreateFunction(new isSupersetOf()));
            ret.SetValue("overlaps", script.CreateFunction(new overlaps()));
            ret.SetValue("setEquals", script.CreateFunction(new setEquals()));
            ret.SetValue("symmetricExceptWith", script.CreateFunction(new symmetricExceptWith()));
            ret.SetValue("trimExcess", script.CreateFunction(new trimExcess()));
            ret.SetValue("forEach", script.CreateFunction(new forEach()));
            ret.SetValue("find", script.CreateFunction(new find()));
            ret.SetValue("toArray", script.CreateFunction(new toArray(script)));
            ret.SetValue("convertAll", script.CreateFunction(new convertAll()));
            return ret;
        }
        private class add : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var hashSet = thisObject.Get<ScriptHashSet>();
                for (int i = 0; i < length; ++i) {
                    hashSet.Add(args[i]);
                }
                return thisObject.Reference();
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().Remove(args[0]) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class removeWhere : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                return new ScriptValue((double)thisObject.Get<ScriptHashSet>().RemoveWhere((value) => {
                    return func.Call(value).IsTrue;
                }));
            }
        }
        private class contains : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().Contains(args[0]) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().Clear();
                return thisObject.Reference();
            }
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptHashSet>().Count);
            }
        }
        private class unionWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().UnionWith(args[0].Value as IEnumerable<ScriptValue>);
                return thisObject.Reference();
            }
        }
        private class exceptWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().ExceptWith(args[0].Value as IEnumerable<ScriptValue>);
                return thisObject.Reference();
            }
        }
        private class intersectWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().IntersectWith(args[0].Value as IEnumerable<ScriptValue>);
                return thisObject.Reference();
            }
        }
        private class isProperSubsetOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().IsProperSubsetOf(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isProperSupersetOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().IsProperSupersetOf(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isSubsetOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().IsSubsetOf(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isSupersetOf : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().IsSupersetOf(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class overlaps : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().Overlaps(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class setEquals : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptHashSet>().SetEquals(args[0].Value as IEnumerable<ScriptValue>) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class symmetricExceptWith : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().SymmetricExceptWith(args[0].Value as IEnumerable<ScriptValue>);
                return thisObject.Reference();
            }
        }
        private class trimExcess : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptHashSet>().TrimExcess();
                return thisObject.Reference();
            }
        }
        private class forEach : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptHashSet>();
                var func = args[0].Get<ScriptFunction>();
                foreach (var value in array) {
                    if (func.Call(value).valueType == ScriptValue.falseValueType) {
                        return value;
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class find : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptHashSet>();
                var func = args[0].Get<ScriptFunction>();
                foreach (var value in array) {
                    if (func.Call(value).IsTrue) {
                        return value;
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class toArray : ScorpioHandle {
            private Script script;
            public toArray(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<Scorpio.Userdata.ScriptUserdataType>();
                return type == null ? ScriptValue.Null : ScriptValue.CreateValue(script, thisObject.Get<ScriptHashSet>().ToArray(type.Type));
            }
        }
        private class convertAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptHashSet>();
                var ret = new ScriptHashSet(array.getScript());
                var func = args[0].Get<ScriptFunction>();
                foreach (var value in array) {
                    ret.Add(func.Call(value));
                }
                return new ScriptValue(ret);
            }
        }
    }
}
