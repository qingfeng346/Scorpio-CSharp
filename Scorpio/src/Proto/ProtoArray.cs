namespace Scorpio.Proto {
    using Scorpio.Userdata;
    public class ProtoArray {
        public static ScriptType Load(Script script, ScriptType ret) {
            var functions = new (string, ScorpioHandle)[] {
                ("length", new length()),
                ("count", new length()),
                ("insert", new insert()),
                ("add", new add()),
                ("push", new add()),
                ("addUnique", new addUnique()),
                ("remove", new remove()),
                ("removeAt", new removeAt()),
                ("clear", new clear()),
                ("contains", new contains()),
                ("sort", new sort()),
                ("indexOf", new indexOf()),
                ("lastIndexOf", new lastIndexOf()),
                ("find", new find()),
                ("findIndex", new findIndex()),
                ("findLast", new findLast()),
                ("findLastIndex", new findLastIndex()),
                ("findAll", new findAll()),
                ("findAllIndex", new findAllIndex()),
                ("findAllLast", new findAllLast()),
                ("findAllLastIndex", new findAllLastIndex()),
                ("forEach", new forEach()),
                ("forEachLast", new forEachLast()),
                ("convertAll", new convertAll()),
                ("map", new convertAll()),
                ("reverse", new reverse()),
                ("first", new first()),
                ("last", new last()),
                ("popFirst", new popFirst()),
                ("safePopFirst", new safePopFirst()),
                ("popLast", new popLast()),
                ("safePopLast", new safePopLast()),
                ("join", new join()),
                ("toArray", new toArray()),
                ("+", new plus()),
                ("-", new minus()),
            };
            ret.SetFunctionCapacity(functions.Length);
            foreach (var (name, func) in functions) {
                ret.SetValue(name, script.CreateFunction(func));
            }
            return ret;
        }
        private class length : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue((double)thisObject.Get<ScriptArray>().m_Length);
            }
        }
        private class insert : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var index = args[0].ToInt32();
                for (int i = 1; i < length; ++i) {
                    array.Insert(index, args[i]);
                }
                return thisObject.Reference();
            }
        }
        private class add : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.Add(args[i]);
                }
                return thisObject.Reference();
            }
        }
        private class addUnique : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.AddUnique(args[i]);
                }
                return thisObject.Reference();
            }
        }
        private class remove : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.Remove(args[i]);
                }
                return thisObject.Reference();
            }
        }
        private class removeAt : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                array.RemoveAt(args[0].ToInt32());
                return thisObject.Reference();
            }
        }
        private class clear : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                thisObject.Get<ScriptArray>().Clear();
                return thisObject.Reference();
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
                return thisObject.Reference();
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
        private class find : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i]).IsTrue) {
                        return array[i].Reference();
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class findIndex : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i]).IsTrue) {
                        return new ScriptValue((double)i);
                    }
                }
                return ScriptValue.InvalidIndex;
            }
        }
        private class findLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (int i = array.Length() - 1; i >= 0; --i) {
                    if (func.Call(array[i]).IsTrue) {
                        return array[i];
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class findLastIndex : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (int i = array.Length() - 1; i >= 0; --i) {
                    if (func.Call(array[i]).IsTrue) {
                        return new ScriptValue((double)i);
                    }
                }
                return ScriptValue.InvalidIndex;
            }
        }
        private class findAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i]).IsTrue) {
                        ret.Add(array[i]);
                    }
                }
                return new ScriptValue(ret);
            }
        }
        private class findAllIndex : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i]).IsTrue) {
                        ret.Add(new ScriptValue((double)i));
                    }
                }
                return new ScriptValue(ret);
            }
        }
        private class findAllLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                var func = args[0].Get<ScriptFunction>();
                for (var i = array.Length() - 1; i >= 0; --i) {
                    if (func.Call(array[i]).IsTrue) {
                        ret.Add(array[i]);
                    }
                }
                return new ScriptValue(ret);
            }
        }
        private class findAllLastIndex : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                var func = args[0].Get<ScriptFunction>();
                for (var i = array.Length() - 1; i >= 0; --i) {
                    if (func.Call(array[i]).IsTrue) {
                        ret.Add(new ScriptValue((double)i));
                    }
                }
                return new ScriptValue(ret);
            }
        }
        private class forEach : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var func = args[0].Get<ScriptFunction>();
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i], new ScriptValue((double)i)).valueType == ScriptValue.falseValueType) {
                        return new ScriptValue((double)i);
                    }
                }
                return ScriptValue.InvalidIndex;
            }
        }
        private class forEachLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (var i = array.Length() - 1; i >= 0; --i) {
                    if (func.Call(array[i], new ScriptValue((double)i)).valueType == ScriptValue.falseValueType) {
                        return new ScriptValue((double)i);
                    }
                }
                return ScriptValue.InvalidIndex;
            }
        }
        private class convertAll : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    using (var v = func.Call(array[i]))
                        ret.Add(v);
                }
                return new ScriptValue(ret);
            }
        }
        private class reverse : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = array.script.NewArray();
                for (var i = array.Length() - 1; i >= 0; --i) {
                    ret.Add(array[i]);
                }
                return new ScriptValue(ret);
            }
        }

        private class first : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().First().Reference();
            }
        }
        private class last : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().Last().Reference();
            }
        }
        private class popFirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().PopFirst().Reference();
            }
        }
        private class safePopFirst : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().SafePopFirst().Reference();
            }
        }
        private class popLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().PopLast().Reference();
            }
        }
        private class safePopLast : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return thisObject.Get<ScriptArray>().SafePopLast().Reference();
            }
        }
        private class join : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var separator = args[0].ToString();
                var value = thisObject.Get<ScriptArray>();
                var count = value.Length();
                var values = new string[count];
                for (int i = 0; i < count; ++i) { values[i] = value[i].ToString(); }
                return new ScriptValue(string.Join(separator, values));
            }
        }
        private class toArray : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var type = args[0].Get<ScriptUserdataType>();
                return type == null ? ScriptValue.Null : ScriptValue.CreateValue(array.script, array.ToArray(type.Type));
            }
        }
        private class plus : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisArray = thisObject.Get<ScriptArray>().NewCopy();
                var array = args[0].Get<ScriptArray>();
                if (array != null) {
                    foreach (var value in array) {
                        thisArray.Add(value);
                    }
                } else {
                    thisArray.Add(args[0]);
                }
                return new ScriptValue(thisArray);
            }
        }
        private class minus : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var thisArray = thisObject.Get<ScriptArray>().NewCopy();
                var array = args[0].Get<ScriptArray>();
                if (array != null) {
                    foreach (var value in array) {
                        thisArray.Remove(value);
                    }
                } else {
                    thisArray.Remove(args[0]);
                }
                return new ScriptValue(thisArray);
            }
        }
    }
}
