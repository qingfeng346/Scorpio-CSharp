namespace Scorpio.Proto {
    public class ProtoArray {
        public static ScriptType Load(Script script, ScriptType ret) {
            ret.SetValue("length", script.CreateFunction(new length()));
            ret.SetValue("count", script.CreateFunction(new length()));
            ret.SetValue("insert", script.CreateFunction(new insert()));
            ret.SetValue("add", script.CreateFunction(new add()));
            ret.SetValue("push", script.CreateFunction(new add()));
            ret.SetValue("addUnique", script.CreateFunction(new addUnique()));
            ret.SetValue("remove", script.CreateFunction(new remove()));
            ret.SetValue("removeAt", script.CreateFunction(new removeAt()));
            ret.SetValue("clear", script.CreateFunction(new clear()));
            ret.SetValue("contains", script.CreateFunction(new contains()));
            ret.SetValue("sort", script.CreateFunction(new sort()));
            ret.SetValue("indexOf", script.CreateFunction(new indexOf()));
            ret.SetValue("lastIndexOf", script.CreateFunction(new lastIndexOf()));
            ret.SetValue("find", script.CreateFunction(new find()));
            ret.SetValue("findIndex", script.CreateFunction(new findIndex()));
            ret.SetValue("findLast", script.CreateFunction(new findLast()));
            ret.SetValue("findLastIndex", script.CreateFunction(new findLastIndex()));
            ret.SetValue("findAll", script.CreateFunction(new findAll()));
            ret.SetValue("findAllIndex", script.CreateFunction(new findAllIndex()));
            ret.SetValue("findAllLast", script.CreateFunction(new findAllLast()));
            ret.SetValue("findAllLastIndex", script.CreateFunction(new findAllLastIndex()));
            ret.SetValue("forEach", script.CreateFunction(new forEach()));
            ret.SetValue("forEachLast", script.CreateFunction(new forEachLast()));
            ret.SetValue("convertAll", script.CreateFunction(new map()));
            ret.SetValue("map", script.CreateFunction(new map()));
            ret.SetValue("reverse", script.CreateFunction(new reverse()));
            ret.SetValue("first", script.CreateFunction(new first()));
            ret.SetValue("last", script.CreateFunction(new last()));
            ret.SetValue("popFirst", script.CreateFunction(new popFirst()));
            ret.SetValue("safePopFirst", script.CreateFunction(new safePopFirst()));
            ret.SetValue("popLast", script.CreateFunction(new popLast()));
            ret.SetValue("safePopLast", script.CreateFunction(new safePopLast()));
            ret.SetValue("join", script.CreateFunction(new join()));
            ret.SetValue("toArray", script.CreateFunction(new toArray()));
            ret.SetValue("+", script.CreateFunction(new plus()));
            ret.SetValue("-", script.CreateFunction(new minus()));
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
        private class addUnique : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                for (int i = 0; i < length; ++i) {
                    array.AddUnique(args[i]);
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
                return thisObject;
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
        private class find : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    if (func.Call(array[i]).IsTrue) {
                        return array[i];
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
                var ret = new ScriptArray(array.getScript());
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
                var ret = new ScriptArray(array.getScript());
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
                var ret = new ScriptArray(array.getScript());
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
                var ret = new ScriptArray(array.getScript());
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
        private class map : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = new ScriptArray(array.getScript());
                var func = args[0].Get<ScriptFunction>();
                for (int i = 0, count = array.Length(); i < count; ++i) {
                    ret.Add(func.Call(array[i]));
                }
                return new ScriptValue(ret);
            }
        }
        private class reverse : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var array = thisObject.Get<ScriptArray>();
                var ret = new ScriptArray(array.getScript());
                for (var i = array.Length() - 1; i >= 0; --i) {
                    ret.Add(array[i]);
                }
                return new ScriptValue(ret);
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
                var type = args[0].Get<Scorpio.Userdata.ScriptUserdataType>();
                return type == null ? ScriptValue.Null : ScriptValue.CreateValue(thisObject.Get<ScriptArray>().ToArray(type.Type));
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
