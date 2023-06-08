using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Userdata;
using Scorpio.Tools;
using System.Reflection;
using Scorpio.Instruction;

namespace Scorpio.Library
{
    public partial class LibraryBasis {
        private abstract class KeyValuePairs<T> : ScorpioHandle {
            readonly Script script;
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<KeyValuePair<T, ScriptValue>> m_Enumerator;
            public KeyValuePairs(Script script, ScriptMap itorResult, IEnumerator<KeyValuePair<T, ScriptValue>> enumerator) {
                this.script = script;
                m_ItorResult = itorResult;
                m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    using (var key = ScriptValue.CreateValue(script, value.Key))
                        m_ItorResult.SetValue("key", key);
                    m_ItorResult.SetValue("value", value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private abstract class ValuePairs : ScorpioHandle {
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator<ScriptValue> m_Enumerator;
            private double m_Index = 0;
            public ValuePairs(ScriptMap itorResult, IEnumerator<ScriptValue> enumerator) {
                m_ItorResult = itorResult;
                m_Enumerator = enumerator;
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
        private class ArrayPairs : ValuePairs {
            public ArrayPairs(ScriptMap itorResult, ScriptArray array) : base(itorResult, array.GetEnumerator()) { }
        }
        private class HashSetPairs : ValuePairs {
            public HashSetPairs(ScriptMap itorResult, ScriptHashSet hashSet) : base(itorResult, hashSet.GetEnumerator()) { }
        }
        private class MapObjectPairs : KeyValuePairs<object> {
            public MapObjectPairs(Script script, ScriptMap itorResult, ScriptMapObject map) : base(script, itorResult, map.GetEnumerator()) { }
        }
        private class MapStringPairs : KeyValuePairs<object> {
            public MapStringPairs(Script script, ScriptMap itorResult, ScriptMapString map) : base(script, itorResult, map.GetEnumerator()) { }
        }

        private class InstancePairs : KeyValuePairs<string> {
            public InstancePairs(Script script, ScriptMap itorResult, ScriptInstance map) : base(script, itorResult, map.GetEnumerator()) { }
        }
        private class TypePairs : KeyValuePairs<string> {
            public TypePairs(Script script, ScriptMap itorResult, ScriptType map) : base(script, itorResult, map.GetEnumerator()) { }
        }
        private class UserdataPairs : ScorpioHandle {
            readonly Script script;
            readonly ScriptMap m_ItorResult;
            readonly IEnumerator m_Enumerator;
            public UserdataPairs(Script script, ScriptMap itorResult, ScriptUserdata userdata) {
                this.script = script;
                m_ItorResult = itorResult;
                if (userdata.Value is IEnumerator) {
                    m_Enumerator = (IEnumerator)userdata.Value;
                } else if (userdata.Value is IEnumerable)  {
                    m_Enumerator = ((IEnumerable)userdata.Value).GetEnumerator();
                } else {
                    throw new ExecutionException("pairs 只支持继承 IEnumerable 的类 或 IEnumerator");
                }
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    using (var value = ScriptValue.CreateValue(script, m_Enumerator.Current)) {
                        m_ItorResult.SetValue("value", value);
                    }
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
        }
        private class GlobalPairs : KeyValuePairs<string> {
            public GlobalPairs(Script script, ScriptMap itorResult, ScriptGlobal global) : base(script, itorResult, global.GetEnumerator()) { }
        }
        public static void Load(Script script) {
            script.SetGlobal("print", script.CreateFunction(new print(script)));
            script.SetGlobal("printf", script.CreateFunction(new printf(script)));
            script.SetGlobal("pairs", script.CreateFunction(new pairs(script)));
            script.SetGlobal("gc", script.CreateFunction(new gc()));

            script.SetGlobal("isNull", script.CreateFunction(new isNull()));
            script.SetGlobal("isBoolean", script.CreateFunction(new isBoolean()));
            script.SetGlobal("isBool", script.CreateFunction(new isBoolean()));
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

            script.SetGlobal("toInt8", script.CreateFunction(new toNumber<sbyte>()));
            script.SetGlobal("toUint8", script.CreateFunction(new toNumber<byte>()));
            script.SetGlobal("toInt16", script.CreateFunction(new toNumber<short>()));
            script.SetGlobal("toUint16", script.CreateFunction(new toNumber<ushort>()));
            script.SetGlobal("toInt32", script.CreateFunction(new toInt32()));
            script.SetGlobal("toUint32", script.CreateFunction(new toNumber<uint>()));
            script.SetGlobal("toInt64", script.CreateFunction(new toInt64()));
            script.SetGlobal("toUint64", script.CreateFunction(new toNumber<ulong>()));
            script.SetGlobal("toFloat32", script.CreateFunction(new toNumber<float>()));
            script.SetGlobal("toFloat64", script.CreateFunction(new toDouble()));

            script.SetGlobal("toSbyte", script.CreateFunction(new toNumber<sbyte>()));
            script.SetGlobal("toByte", script.CreateFunction(new toNumber<byte>()));
            script.SetGlobal("toShort", script.CreateFunction(new toNumber<short>()));
            script.SetGlobal("toUshort", script.CreateFunction(new toNumber<ushort>()));
            script.SetGlobal("toInt", script.CreateFunction(new toInt32()));
            script.SetGlobal("toUint", script.CreateFunction(new toNumber<uint>()));
            script.SetGlobal("toLong", script.CreateFunction(new toInt64()));
            script.SetGlobal("toUlong", script.CreateFunction(new toNumber<ulong>()));
            script.SetGlobal("toFloat", script.CreateFunction(new toNumber<float>()));
            script.SetGlobal("toDouble", script.CreateFunction(new toDouble()));

            script.SetGlobal("toBool", script.CreateFunction(new toBoolean()));
            script.SetGlobal("toBoolean", script.CreateFunction(new toBoolean()));
            script.SetGlobal("toChar", script.CreateFunction(new toChar()));
            script.SetGlobal("toNumber", script.CreateFunction(new toDouble()));

            script.SetGlobal("toEnum", script.CreateFunction(new toEnum(script)));
            script.SetGlobal("toString", script.CreateFunction(new toString()));

            script.SetGlobal("typeOf", script.CreateFunction(new getPrototype(script)));
            //script.SetGlobal("setPrototype", script.CreateFunction(new setPrototype()));
            script.SetGlobal("getPrototype", script.CreateFunction(new getPrototype(script)));
            script.SetGlobal("setPropertys", script.CreateFunction(new setPropertys()));
            script.SetGlobal("createArray", script.CreateFunction(new createArray()));
            script.SetGlobal("getBase", script.CreateFunction(new getBase()));
            script.SetGlobal("clone", script.CreateFunction(new clone()));

            script.SetGlobal("require", script.CreateFunction(new require(script)));
            script.SetGlobal("setFastReflectClass", script.CreateFunction(new setFastReflectClass(script)));
            script.SetGlobal("isFastReflectClass", script.CreateFunction(new isFastReflectClass(script)));

            script.SetGlobal("pushSearch", script.CreateFunction(new pushSearch(script)));
            script.SetGlobal("pushAssembly", script.CreateFunction(new pushAssembly(script)));
            script.SetGlobal("importType", script.CreateFunction(new importType(script)));
            script.SetGlobal("importNamespace", script.CreateFunction(new importNamespace(script)));
            script.SetGlobal("importExtension", script.CreateFunction(new importExtension(script)));
            script.SetGlobal("genericType", script.CreateFunction(new genericType(script)));
            script.SetGlobal("genericMethod", script.CreateFunction(new genericMethod(script)));
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
                var itorResult = m_script.NewMapObject();  
                if (obj is ScriptArray) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new ArrayPairs(itorResult, (ScriptArray)obj)));
                } else if (obj is ScriptMapObject) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new MapObjectPairs(m_script, itorResult, (ScriptMapObject)obj)));
                } else if (obj is ScriptMapString) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new MapStringPairs(m_script, itorResult, (ScriptMapString)obj)));
                } else if (obj is ScriptHashSet) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new HashSetPairs(itorResult, (ScriptHashSet)obj)));
                } else if (obj is ScriptUserdata) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new UserdataPairs(m_script, itorResult, (ScriptUserdata)obj)));
                } else if (obj is ScriptInstance) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new InstancePairs(m_script, itorResult, (ScriptInstance)obj)));
                } else if (obj is ScriptType) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new TypePairs(m_script, itorResult, (ScriptType)obj)));
                } else if (obj is ScriptGlobal) {
                    itorResult.SetValue(ScriptConstValue.IteratorNext, m_script.CreateFunction(new GlobalPairs(m_script, itorResult, (ScriptGlobal)obj)));
                } else {
                    throw new ExecutionException("pairs 必须用于 array, map, type, global 或者 继承 IEnumerable 的 userdata 类型");
                }
                return new ScriptValue(itorResult);
            }
        }
        private class gc : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                //for (var i = ScriptContext.VariableValueIndex; i < ScriptContext.ValueCacheLength; ++i) {
                //    Array.Clear(ScriptContext.VariableValues[i], 0, ScriptContext.VariableValues[i].Length);
                //    Array.Clear(ScriptContext.StackValues[i], 0, ScriptContext.StackValues[i].Length);
                //}
                //ScriptContext.AsyncValueQueue.Clear();
                GC.Collect();
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
                return (valueType == ScriptValue.doubleValueType || valueType == ScriptValue.int64ValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.doubleValueType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isLong : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.int64ValueType) ? ScriptValue.True : ScriptValue.False;
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
        public class toNumber<T> : ScorpioHandle where T : struct, IConvertible {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateNumber(args[0].ToNumber<T>());
            }
        }
        private class toBoolean : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return Convert.ToBoolean(args[0].Value) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class toChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToChar());
            }
        }
        private class toInt32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToInt32());
            }
        }
        private class toInt64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToLong());
            }
        }
        private class toDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToDouble());
            }
        }
        private class toEnum : ScorpioHandle {
            readonly Script script;
            public toEnum(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdataEnumType>();
                if (args[1].valueType == ScriptValue.stringValueType) {
                    var ignoreCase = length > 2 ? args[2].valueType == ScriptValue.trueValueType : true;
                    return ScriptValue.CreateValue(script, Enum.Parse(type.Type, args[1].stringValue, ignoreCase));
                } else {
                    return ScriptValue.CreateValue(script, Enum.ToObject(type.Type, args[1].ToLong()));
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
                return m_script.LoadFile(args[0].ToString());
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
        //private class setPrototype : ScorpioHandle {
        //    public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
        //        var scriptObject = args[0].Get();
        //        if (scriptObject == null) {
        //            throw new ExecutionException("setPrototype 第1个参数必须是 ScriptObject");
        //        }
        //        var scriptType = args[1].Get<ScriptType>();
        //        if (scriptType == null) {
        //            throw new ExecutionException("setPrototype 第2个参数必须是 ScriptType");
        //        }
        //        if (scriptObject is ScriptType) {
        //            (scriptObject as ScriptType).Prototype = scriptType;
        //        } else if (scriptObject is ScriptInstance) {
        //            (scriptObject as ScriptInstance).Prototype = scriptType;
        //        }
        //        return ScriptValue.Null;
        //    }
        //}
        private class getPrototype : ScorpioHandle {
            private Script script;
            public getPrototype(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                switch (value.valueType) {
                    case ScriptValue.trueValueType:
                    case ScriptValue.falseValueType:
                        return script.TypeBooleanValue.Reference();
                    case ScriptValue.doubleValueType:
                    case ScriptValue.int64ValueType:
                        return script.TypeNumberValue.Reference();
                    case ScriptValue.stringValueType:
                        return script.TypeStringValue.Reference();
                    case ScriptValue.scriptValueType:
                        if (value.scriptValue is ScriptArray) {
                            return script.TypeArrayValue.Reference();
                        } else if (value.scriptValue is ScriptMap) {
                            return script.TypeMapValue.Reference();
                        } else if (value.scriptValue is ScriptFunction) {
                            return script.TypeFunctionValue.Reference();
                        } else if (value.scriptValue is ScriptInstance) {
                            return (value.scriptValue as ScriptInstance).PrototypeValue.Reference();
                        } else if (value.scriptValue is ScriptType) {
                            return (value.scriptValue as ScriptType).PrototypeValue.Reference();
                        } else {
                            return script.GetUserdataTypeValue(value.scriptValue.Type).Reference();
                        }
                    default: return script.TypeObjectValue.Reference();
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
                var userData = args[0].Get<ScriptUserdata>();
                return ScriptValue.CreateValue(userData.script, Array.CreateInstance(userData.Type, args[1].ToInt32()));
            }
        }
        private class getBase : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                if (value.valueType == ScriptValue.scriptValueType) {
                    if (value.scriptValue is ScriptInstance) {
                        return (value.scriptValue as ScriptInstance).Prototype.PrototypeValue.Reference();
                    } else if (value.scriptValue is ScriptType) {
                        return (value.scriptValue as ScriptType).PrototypeValue.Reference();
                    }
                }
                return ScriptValue.Null;
            }
        }

        private class pushAssembly : ScorpioHandle {
            private Script script;
            public pushAssembly(Script script) {
                this.script = script;
            }
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
                script.PushAssembly(assembly);
                return ScriptValue.Null;
            }
        }
        private class importType : ScorpioHandle {
            private Script script;
            public importType(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(script.GetUserdataTypeValue(args[0].ToString()));
            }
        }
        private class importNamespace : ScorpioHandle {
            private Script script;
            public importNamespace(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(new ScriptNamespace(script, args[0].ToString()));
            }
        }
        private class importExtension : ScorpioHandle {
            private Script script;
            public importExtension(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType == ScriptValue.stringValueType) {
                    script.LoadExtension(args[0].stringValue);
                } else {
                    var userdata = args[0].Get<ScriptUserdata>();
                    if (userdata != null) {
                        script.LoadExtension(userdata.Type);
                    }
                }
                return ScriptValue.Null;
            }
        }
        private class genericType : ScorpioHandle {
            private Script script;
            public genericType(Script script) {
                this.script = script;
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
                return script.GetUserdataType(args[0].scriptValue.Type).MakeGenericType(script, types).Reference();
            }
        }
        private class genericMethod : ScorpioHandle {
            private Script script;
            public genericMethod(Script script) {
                this.script = script;
            }
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
                    return new ScriptValue(script.NewStaticMethod().Set(method.MethodName, method));
                } else {
                    return new ScriptValue(script.NewGenericMethod().Set(method.MethodName, method));
                }
            }
        }
        private class setFastReflectClass : ScorpioHandle {
            private Script script;
            public setFastReflectClass(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdata>().Type;
                var fastClass = args[1].Value as IScorpioFastReflectClass;
                script.SetFastReflectClass(type, fastClass);
                return ScriptValue.Null;
            }
        }
        private class isFastReflectClass : ScorpioHandle {
            private Script script;
            public isFastReflectClass(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return script.IsFastReflectClass(args[0].Get<ScriptUserdata>().Type) ? ScriptValue.True : ScriptValue.False;
            }
        }
    }
}
