using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Userdata;
using System.Reflection;
namespace Scorpio.Library {
    public class LibraryBasis {
        private class ArrayPairs : ScorpioHandle {
            Script m_Script;
            IEnumerator<ScriptValue> m_Enumerator;
            int m_Index = 0;
            ScriptMap m_Map;
            public ArrayPairs(Script script, ScriptArray obj, ScriptMap map) {
                m_Script = script;
                m_Index = 0;
                m_Enumerator = obj.GetEnumerator();
                m_Map = map;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_Map.SetValue("key", m_Script.CreateObject(m_Index++));
                    m_Map.SetValue("value", m_Enumerator.Current);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class MapPairs : ScorpioHandle {
            Script m_Script;
            IEnumerator<KeyValuePair<object, ScriptValue>> m_Enumerator;
            ScriptMap m_Map;
            public MapPairs(Script script, ScriptMap obj, ScriptMap map) {
                m_Script = script;
                m_Enumerator = obj.GetEnumerator();
                m_Map = map;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_Map.SetValue("key", m_Script.CreateObject(value.Key));
                    m_Map.SetValue("value", value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class UserdataPairs : ScorpioHandle {
            Script m_Script;
            System.Collections.IEnumerator m_Enumerator;
            ScriptMap m_Map;
            public UserdataPairs(Script script, ScriptUserdata obj, ScriptMap map) {
                m_Script = script;
                var ienumerable = obj.Value as System.Collections.IEnumerable;
                if (ienumerable == null) throw new ExecutionException("pairs 只支持继承 IEnumerable 的类");
                m_Enumerator = ienumerable.GetEnumerator();
                m_Map = map;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_Map.SetValue("value", m_Script.CreateObject(m_Enumerator.Current));
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        public static void Load(Script script) {
            script.SetGlobal("print", script.CreateFunction(new print(script)));
            script.SetGlobal("pairs", script.CreateFunction(new pairs(script)));

            script.SetGlobal("isBoolean", script.CreateFunction(new isBoolean()));
            script.SetGlobal("isNumber", script.CreateFunction(new isNumber()));
            script.SetGlobal("isDouble", script.CreateFunction(new isDouble()));
            script.SetGlobal("isLong", script.CreateFunction(new isLong()));
            script.SetGlobal("isString", script.CreateFunction(new isString()));
            script.SetGlobal("isFunction", script.CreateFunction(new isFunction()));
            script.SetGlobal("isArray", script.CreateFunction(new isArray()));
            script.SetGlobal("isMap", script.CreateFunction(new isMap()));
            script.SetGlobal("isUserdata", script.CreateFunction(new isUserdata()));
            script.SetGlobal("isType", script.CreateFunction(new isType()));
            script.SetGlobal("isInstance", script.CreateFunction(new isInstance()));

            

            script.SetGlobal("toInt8", script.CreateFunction(new toInt8()));
            script.SetGlobal("toUint8", script.CreateFunction(new toUint8()));
            script.SetGlobal("toInt16", script.CreateFunction(new toInt16()));
            script.SetGlobal("toUint16", script.CreateFunction(new toUint16()));
            script.SetGlobal("toInt32", script.CreateFunction(new toInt32()));
            script.SetGlobal("toUint32", script.CreateFunction(new toUint32()));
            script.SetGlobal("toInt64", script.CreateFunction(new toInt64()));
            script.SetGlobal("toUint64", script.CreateFunction(new toUint64()));

            script.SetGlobal("toSbyte", script.CreateFunction(new toInt8()));
            script.SetGlobal("toByte", script.CreateFunction(new toUint8()));
            script.SetGlobal("toShort", script.CreateFunction(new toInt16()));
            script.SetGlobal("toUshort", script.CreateFunction(new toUint16()));
            script.SetGlobal("toInt", script.CreateFunction(new toInt32()));
            script.SetGlobal("toUint", script.CreateFunction(new toUint32()));
            script.SetGlobal("toLong", script.CreateFunction(new toInt64()));
            script.SetGlobal("toUlong", script.CreateFunction(new toUint64()));

            script.SetGlobal("toFloat", script.CreateFunction(new toFloat()));
            script.SetGlobal("toNumber", script.CreateFunction(new toDouble()));
            script.SetGlobal("toDouble", script.CreateFunction(new toDouble()));

            script.SetGlobal("toEnum", script.CreateFunction(new toEnum(script)));
            script.SetGlobal("toString", script.CreateFunction(new toString()));


            script.SetGlobal("typeOf", script.CreateFunction(new typeOf(script)));
            script.SetGlobal("clone", script.CreateFunction(new clone()));
            script.SetGlobal("require", script.CreateFunction(new require(script)));


            script.SetGlobal("push_search", script.CreateFunction(new push_search(script)));
            script.SetGlobal("push_assembly", script.CreateFunction(new push_assembly(script)));
            script.SetGlobal("import_type", script.CreateFunction(new import_type(script)));
            script.SetGlobal("import_extension", script.CreateFunction(new import_extension(script)));
            script.SetGlobal("generic_type", script.CreateFunction(new generic_type(script)));
            script.SetGlobal("generic_method", script.CreateFunction(new generic_method(script)));
        }
        private class print : ScorpioHandle {
            private Script m_script;
            public print(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var str = new StringBuilder();
                for (var i = 0; i < length; ++i) {
                    if (i != 0) {
                        str.Append("    ");
                    }
                    str.Append(args[i]);
                }
                System.Console.WriteLine(str);
                System.Diagnostics.Debug.WriteLine(str);
                return ScriptValue.Null;
            }
        }
        private class pairs : ScorpioHandle {
            private Script m_script;
            public pairs(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var obj = args[0].valueType == ScriptValue.scriptValueType ? args[0].scriptValue : null;
                if (obj is ScriptArray) {
                    var map = new ScriptMap(m_script);
                    map.SetValue("next", m_script.CreateFunction(new ArrayPairs(m_script, (ScriptArray)obj, map)));
                    return new ScriptValue(map);
                } else if (obj is ScriptMap) {
                    var map = new ScriptMap(m_script);
                    map.SetValue("next", m_script.CreateFunction(new MapPairs(m_script, (ScriptMap)obj, map)));
                    return new ScriptValue(map);
                } else if (obj is ScriptUserdata) {
                    var map = new ScriptMap(m_script);
                    map.SetValue("next", m_script.CreateFunction(new UserdataPairs(m_script, (ScriptUserdata)obj, map)));
                    return new ScriptValue(map);
                }
                throw new ExecutionException("pairs必须用于arrar, map, 继承IEnumerable的userdata 类型");
            }
        }
        private class isBoolean : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var valueType = args[0].valueType;
                return valueType == ScriptValue.trueValueType || valueType == ScriptValue.falseValueType;
            }
        }
        private class isNumber : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var valueType = args[0].valueType;
                return valueType == ScriptValue.doubleValueType || valueType == ScriptValue.longValueType;
            }
        }
        private class isDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.doubleValueType;
            }
        }
        private class isLong : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.longValueType;
            }
        }
        private class isString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.stringValueType;
            }
        }
        private class isFunction : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptFunction;
            }
        }
        private class isArray : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptArray;
            }
        }
        private class isMap : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptMap;
            }
        }
        private class isUserdata : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptUserdata;
            }
        }
        private class isType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptType;
            }
        }
        private class isInstance : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue.GetType() == typeof(ScriptInstance);
            }
        }


        private class toInt8 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToSByte(args[0].Value));
            }
        }
        private class toUint8 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToByte(args[0].Value));
            }
        }
        private class toInt16 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToInt16(args[0].Value));
            }
        }
        private class toUint16 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToUInt16(args[0].Value));
            }
        }
        private class toInt32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToInt32(args[0].Value));
            }
        }
        private class toUint32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToUInt32(args[0].Value));
            }
        }
        private class toInt64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToInt64(args[0].Value));
            }
        }
        private class toUint64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToUInt64(args[0].Value));
            }
        }
        private class toFloat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToSingle(args[0].Value));
            }
        }
        private class toDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToDouble(args[0].Value));
            }
        }
        private class toEnum : ScorpioHandle {
            private readonly Script m_script;
            public toEnum(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdataEnumType>();
                if (args[1].valueType == ScriptValue.stringValueType) {
                    var ignoreCase = length > 2 ? args[2].valueType == ScriptValue.trueValueType : false;
                    return new ScriptValue(Enum.Parse(type.Type, args[1].stringValue, ignoreCase));
                } else {
                    return new ScriptValue(Enum.ToObject(type.Type, args[1].ToInt32()));
                }
            }
        }
        private class toString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToString());
            }
        }

        private class typeOf : ScorpioHandle {
            private Script m_Script;
            public typeOf(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.scriptValueType ? m_Script.GetUserdataType(args[0].scriptValue.Type) : ScriptValue.Null;
            }
        }
        private class clone : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.scriptValueType) {
                    return new ScriptValue(args[0].scriptValue.Clone());
                } else {
                    return args[0];
                }
            }
        }
        private class require : ScorpioHandle {
            private Script m_script;
            public require(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return m_script.LoadSearchPathFile(args[0].ToString());
            }
        }
        private class push_search : ScorpioHandle {
            private Script m_script;
            public push_search(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                m_script.PushSearchPath(args[0].ToString());
                return ScriptValue.Null;
            }
        }
        private class push_assembly : ScorpioHandle {
            private Script m_script;
            public push_assembly(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    m_script.PushAssembly(Assembly.Load(new AssemblyName(args[0].ToString())));
                } else {
                    m_script.PushAssembly(args[0].Value as Assembly);
                }
                return ScriptValue.Null;
            }
        }
        private class import_type : ScorpioHandle {
            private Script m_script;
            public import_type(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return m_script.GetUserdataType(args[0].ToString());
            }
        }
        private class import_extension : ScorpioHandle {
            private Script m_script;
            public import_extension(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.scriptValueType) {
                    m_script.LoadExtension(args[0].scriptValue.Type);
                } else {
                    m_script.LoadExtension(args[0].ToString());
                }
                return ScriptValue.Null;
            }
        }
        private class generic_type : ScorpioHandle {
            private Script m_script;
            public generic_type(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.scriptValueType)
                    throw new ExecutionException("generic_type 第1个参数必须是 Type");
                var types = new Type[length - 1];
                for (int i = 1; i < length; ++i) {
                    if (args[i].valueType != ScriptValue.scriptValueType)
                        throw new ExecutionException($"generic_type 第{i+1}个参数必须是 Type");
                    types[i - 1] = args[i].scriptValue.Type;
                }
                return m_script.GetType(args[0].scriptValue.Type).MakeGenericType(types);
            }
        }
        private class generic_method : ScorpioHandle {
            private readonly Script m_script;
            public generic_method(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.scriptValueType || !(args[0].scriptValue is ScriptMethodFunction)) {
                    throw new ExecutionException("generic_method 第1个参数必须是函数");
                }
                var types = new Type[length - 1];
                for (int i = 1; i < args.Length; ++i) {
                    if (args[i].valueType != ScriptValue.scriptValueType)
                        throw new ExecutionException($"generic_method 第{i + 1}个参数必须是 Type");
                    types[i - 1] = args[i].scriptValue.Type;
                }
                var method = (args[0].scriptValue as ScriptMethodFunction).Method.MakeGenericMethod(types);
                if (method.IsStatic) {
                    return new ScriptValue(new ScriptStaticMethodFunction(m_script, method));
                } else {
                    return new ScriptValue(new ScriptGenericMethodFunction(m_script, method));
                }
            }
        }
    }
}
