using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Userdata;
using Scorpio.Tools;
using System.Reflection;
using Scorpio.Instruction;
using Scorpio.Runtime;
namespace Scorpio.Library {
    public partial class LibraryBasis {
        private class ArrayPairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<ScriptValue> m_Enumerator;
            double m_Index = 0;
            public ArrayPairs(ScriptArray array, ScriptMap itorResult) {
                m_Index = 0;
                m_Enumerator = array.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetValue("key", new ScriptValue(m_Index++));
                    m_ItorResult.SetValue("value", m_Enumerator.Current);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class MapObjectPairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<KeyValuePair<object, ScriptValue>> m_Enumerator;
            public MapObjectPairs(ScriptMapObject map, ScriptMap itorResult) {
                m_Enumerator = map.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetValue("key", ScriptValue.CreateValue(m_Enumerator.Current.Key));
                    m_ItorResult.SetValue("value", m_Enumerator.Current.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class MapStringPairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<KeyValuePair<object, ScriptValue>> m_Enumerator;
            public MapStringPairs(ScriptMapString map, ScriptMap itorResult) {
                m_Enumerator = map.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetValue("key", ScriptValue.CreateValue(m_Enumerator.Current.Key));
                    m_ItorResult.SetValue("value", m_Enumerator.Current.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class InstancePairs : ScorpioHandle {
            readonly ScriptInstance m_ItorResult;
            readonly IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            public InstancePairs(ScriptInstance map, ScriptMap itorResult) {
                m_Enumerator = map.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_ItorResult.SetValue("key", ScriptValue.CreateValue(value.Key));
                    m_ItorResult.SetValue("value", value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class TypePairs : ScorpioHandle {
            readonly ScriptInstance m_ItorResult;
            readonly IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            public TypePairs(ScriptType map, ScriptMap itorResult) {
                m_Enumerator = map.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_ItorResult.SetValue("key", ScriptValue.CreateValue(value.Key));
                    m_ItorResult.SetValue("value", value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class UserdataPairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly System.Collections.IEnumerator m_Enumerator;
            public UserdataPairs(ScriptUserdata userdata, ScriptMap itorResult) {
                var ienumerable = userdata.Value as System.Collections.IEnumerable;
                if (ienumerable == null) throw new ExecutionException("pairs 只支持继承 IEnumerable 的类");
                m_Enumerator = ienumerable.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetValue("value", ScriptValue.CreateValue(m_Enumerator.Current));
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class GlobalPairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<ScorpioValue<string, ScriptValue>> m_Enumerator;
            public GlobalPairs(ScriptGlobal global, ScriptMap itorResult) {
                m_Enumerator = global.GetEnumerator();
                m_ItorResult = itorResult;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_ItorResult.SetValue("key", new ScriptValue(value.Key));
                    m_ItorResult.SetValue("value", value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        public static void Load(Script script) {
            script.SetGlobal("print", script.CreateFunction(new print(script)));
            script.SetGlobal("printf", script.CreateFunction(new printf(script)));
            script.SetGlobal("pairs", script.CreateFunction(new pairs(script)));
            script.SetGlobal("gc", script.CreateFunction(new gc()));

            script.SetGlobal("isNull", script.CreateFunction(new isNull()));
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

            script.SetGlobal("toChar", script.CreateFunction(new toChar()));

            script.SetGlobal("toFloat", script.CreateFunction(new toFloat()));
            script.SetGlobal("toNumber", script.CreateFunction(new toDouble()));
            script.SetGlobal("toDouble", script.CreateFunction(new toDouble()));

            script.SetGlobal("toEnum", script.CreateFunction(new toEnum()));
            script.SetGlobal("toString", script.CreateFunction(new toString()));

            script.SetGlobal("typeOf", script.CreateFunction(new getPrototype(script)));
            script.SetGlobal("setPrototype", script.CreateFunction(new setPrototype()));
            script.SetGlobal("getPrototype", script.CreateFunction(new getPrototype(script)));
            script.SetGlobal("setPropertys", script.CreateFunction(new setPropertys()));
            script.SetGlobal("createArray", script.CreateFunction(new createArray()));
            script.SetGlobal("getBase", script.CreateFunction(new getBase()));
            script.SetGlobal("clone", script.CreateFunction(new clone()));

            script.SetGlobal("require", script.CreateFunction(new require(script)));
            script.SetGlobal("setFastReflectClass", script.CreateFunction(new setFastReflectClass()));
            script.SetGlobal("isFastReflectClass", script.CreateFunction(new isFastReflectClass()));

            script.SetGlobal("pushSearch", script.CreateFunction(new pushSearch(script)));
            script.SetGlobal("pushAssembly", script.CreateFunction(new pushAssembly()));
            script.SetGlobal("importType", script.CreateFunction(new importType()));
            script.SetGlobal("importNamespace", script.CreateFunction(new importNamespace()));
            script.SetGlobal("importExtension", script.CreateFunction(new importExtension()));
            script.SetGlobal("genericType", script.CreateFunction(new genericType()));
            script.SetGlobal("genericMethod", script.CreateFunction(new genericMethod()));
        }
        private class print : ScorpioHandle {
            private Script script;
            internal print(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var builder = new StringBuilder();
                var stack = script.GetStackInfo();
                builder.Append($"{stack.Breviary}:{stack.Line} ");
                for (var i = 0; i < length; ++i) {
                    if (i != 0) { builder.Append("    "); }
                    builder.Append(args[i]);
                }
                System.Console.WriteLine(builder);
                return ScriptValue.Null;
            }
        }
        private class printf : ScorpioHandle {
            private Script script;
            internal printf(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var format = args[0].ToString();
                var index = 1;
                var strLength = format.Length;
                var strLength1 = strLength - 1;
                var builder = new StringBuilder();
                var stack = script.GetStackInfo();
                builder.Append($"{stack.Breviary}:{stack.Line} ");
                for (var i = 0; i < strLength;) {
                    var c = format[i];
                    if (c == '{' && i < strLength1 && format[i + 1] == '}') {
                        i += 2;
                        builder.Append(args[index++]);
                    } else {
                        builder.Append(c);
                        ++i;
                    }
                }
                System.Console.WriteLine(builder);
                return ScriptValue.Null;
            }
        }
        private class pairs : ScorpioHandle {
            private readonly Script m_script;
            public pairs(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var obj = args[0].valueType == ScriptValue.scriptValueType ? args[0].scriptValue : null;
                var map = new ScriptMapString(m_script);
                if (obj is ScriptArray) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new ArrayPairs((ScriptArray)obj, map)));
                } else if (obj is ScriptMapObject) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new MapObjectPairs((ScriptMapObject)obj, map)));
                } else if (obj is ScriptMapString) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new MapStringPairs((ScriptMapString)obj, map)));
                } else if (obj is ScriptUserdata) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new UserdataPairs((ScriptUserdata)obj, map)));
                } else if (obj is ScriptInstance) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new InstancePairs((ScriptInstance)obj, map)));
                } else if (obj is ScriptType) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new TypePairs((ScriptType)obj, map)));
                } else if (obj is ScriptGlobal) {
                    map.SetValue(ScriptConst.IteratorNext, m_script.CreateFunction(new GlobalPairs((ScriptGlobal)obj, map)));
                } else {
                    throw new ExecutionException("pairs 必须用于 array, map, type, global 或者 继承 IEnumerable 的 userdata 类型");
                }
                return new ScriptValue(map);
            }
        }
        private class gc : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                Array.Clear(ScriptValue.Parameters, 0, ScriptValue.Parameters.Length);
                for (var i = ScriptContext.VariableValueIndex; i < ScriptContext.ValueCacheLength; ++i) {
                    Array.Clear(ScriptContext.VariableValues[i], 0, ScriptContext.VariableValues[i].Length);
                    Array.Clear(ScriptContext.StackValues[i], 0, ScriptContext.StackValues[i].Length);
                }
                return ScriptValue.Null;
            }
        }
        private class isNull : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.nullValueType ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isBoolean : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var valueType = args[0].valueType;
                return (valueType == ScriptValue.trueValueType || valueType == ScriptValue.falseValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isNumber : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var valueType = args[0].valueType;
                return (valueType == ScriptValue.doubleValueType || valueType == ScriptValue.longValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.doubleValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isLong : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.longValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isString : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return args[0].valueType == ScriptValue.stringValueType ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isFunction : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptFunction) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isArray : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptArray) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isMap : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptMap) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isUserdata : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptUserdata) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isInstance : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue.GetType() == typeof(ScriptInstance)) ? ScriptValue.True : ScriptValue.False;
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
        private class toChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(System.Convert.ToChar(args[0].Value));
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
        private class clone : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var deep = length > 1 ? args[1].IsTrue : true;
                if (args[0].valueType == ScriptValue.scriptValueType) {
                    return new ScriptValue(args[0].scriptValue.Clone(deep));
                } else {
                    return args[0];
                }
            }
        }
        private class require : ScorpioHandle {
            readonly Script m_script;
            public require(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return m_script.LoadSearchPathFile(args[0].ToString());
            }
        }
        private class pushSearch : ScorpioHandle {
            readonly Script m_script;
            public pushSearch(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                m_script.PushSearchPath(args[0].ToString());
                return ScriptValue.Null;
            }
        }
        private class setPrototype : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var scriptValue = args[0].Get();
                if (scriptValue is ScriptType) {
                    (scriptValue as ScriptType).Prototype = args[1];
                } else if (scriptValue is ScriptInstance) {
                    (scriptValue as ScriptInstance).Prototype = args[1];
                }
                return ScriptValue.Null;
            }
        }
        private class getPrototype : ScorpioHandle {
            readonly Script m_Script;
            public getPrototype(Script script) {
                m_Script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.trueValueType:
                    case ScriptValue.falseValueType:
                        return m_Script.TypeBooleanValue;
                    case ScriptValue.doubleValueType:
                    case ScriptValue.longValueType:
                        return m_Script.TypeNumberValue;
                    case ScriptValue.stringValueType:
                        return m_Script.TypeStringValue;
                    case ScriptValue.scriptValueType:
                        if (value.scriptValue is ScriptArray) {
                            return m_Script.TypeArrayValue;
                        } else if (value.scriptValue is ScriptMap) {
                            return m_Script.TypeMapValue;
                        } else if (value.scriptValue is ScriptFunction) {
                            return m_Script.TypeFunctionValue;
                        } else if (value.scriptValue is ScriptInstance) {
                            return (value.scriptValue as ScriptInstance).Prototype;
                        } else if (value.scriptValue is ScriptType) {
                            return (value.scriptValue as ScriptType).Prototype;
                        } else {
                            return TypeManager.GetUserdataType(value.scriptValue.Type);
                        }
                    default: return m_Script.TypeObjectValue;
                }
            }
        }
        private class setPropertys : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var source = args[0].Get<ScriptInstance>();
                var target = args[1].Get<ScriptInstance>();
                if (target is ScriptMap) {
                    foreach (var pair in (target as ScriptMap)) {
                        if (pair.Key is string) {
                            source.SetValue((string)pair.Key, pair.Value);
                        }
                    }
                } else {
                    foreach (var pair in target) {
                        source.SetValue(pair.Key, pair.Value);
                    }
                }
                return args[0];
            }
        }
        private class createArray : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(Array.CreateInstance(args[0].Get<ScriptUserdata>().Type, args[1].ToInt32()));
            }
        }
        private class getBase : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                if (value.valueType == ScriptValue.scriptValueType) {
                    if (value.scriptValue is ScriptInstance) {
                        return ((value.scriptValue as ScriptInstance).Prototype.scriptValue as ScriptType).Prototype;
                    } else if (value.scriptValue is ScriptType) {
                        return (value.scriptValue as ScriptType).Prototype;
                    }
                }
                return ScriptValue.Null;
            }
        }

        private class pushAssembly : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var throwException = length > 1 ? args[1].IsTrue : true;
                Assembly assembly = null;
                if (args[0].valueType == ScriptValue.stringValueType) {
                    if (throwException) {
                        assembly = Assembly.Load(new AssemblyName(args[0].ToString()));
                    } else {
                        try {
                            assembly = Assembly.Load(new AssemblyName(args[0].ToString()));
                        } catch (System.Exception) { }
                    }
                } else {
                    assembly = args[0].Value as Assembly;
                }
                TypeManager.PushAssembly(assembly);
                return ScriptValue.Null;
            }
        }
        private class importType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return TypeManager.GetUserdataType(args[0].ToString());
            }
        }
        private class importNamespace : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(new ScriptNamespace(args[0].ToString()));
            }
        }
        private class importExtension : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    TypeManager.LoadExtension(args[0].stringValue);
                } else {
                    var userdata = args[0].Get<ScriptUserdata>();
                    if (userdata != null) {
                        TypeManager.LoadExtension(userdata.Type);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class genericType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.scriptValueType)
                    throw new ExecutionException("generic_type 第1个参数必须是 Type");
                var types = new Type[length - 1];
                for (int i = 1; i < length; ++i) {
                    if (args[i].valueType != ScriptValue.scriptValueType)
                        throw new ExecutionException($"generic_type 第{i+1}个参数必须是 Type");
                    types[i - 1] = args[i].scriptValue.Type;
                }
                return TypeManager.GetType(args[0].scriptValue.Type).MakeGenericType(types);
            }
        }
        private class genericMethod : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.scriptValueType || !(args[0].scriptValue is ScriptMethodFunction)) {
                    throw new ExecutionException("generic_method 第1个参数必须是函数");
                }
                var types = new Type[length - 1];
                for (int i = 1; i < length; ++i) {
                    if (args[i].valueType != ScriptValue.scriptValueType)
                        throw new ExecutionException($"generic_method 第{i + 1}个参数必须是 Type");
                    types[i - 1] = args[i].scriptValue.Type;
                }
                var method = (args[0].scriptValue as ScriptMethodFunction).Method.MakeGenericMethod(types);
                if (method.IsStatic) {
                    return new ScriptValue(new ScriptStaticMethodFunction(method, method.MethodName));
                } else {
                    return new ScriptValue(new ScriptGenericMethodFunction(method, method.MethodName));
                }
            }
        }
        private class setFastReflectClass : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdata>().Type;
                var fastClass = args[1].Value as ScorpioFastReflectClass;
                TypeManager.SetFastReflectClass(type, fastClass);
                return ScriptValue.Null;
            }
        }
        private class isFastReflectClass : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return TypeManager.IsFastReflectClass(args[0].Get<ScriptUserdata>().Type) ? ScriptValue.True : ScriptValue.False;
            }
        }
    }
}
