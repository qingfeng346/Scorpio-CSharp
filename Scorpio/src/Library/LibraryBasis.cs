using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Userdata;
using Scorpio.Tools;
using System.Reflection;

namespace Scorpio.Library
{
    public partial class LibraryBasis {
        private class ArrayPairs : ScorpioHandle, IDisposable {
            private ScriptEnumerator m_ItorResult;
            private IEnumerator<ScriptValue> m_Enumerator;
            private double m_Index = 0;
            public ArrayPairs(ScriptEnumerator itorResult, IEnumerator<ScriptValue> enumerator) {
                m_ItorResult = itorResult;
                m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetKey(new ScriptValue(m_Index++));
                    m_ItorResult.SetValue(m_Enumerator.Current);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
            public void Dispose() {
                m_Enumerator.Dispose();
                m_Enumerator = null;
                m_ItorResult = null;
            }
        }
        private class MapPairs : ScorpioHandle, IDisposable {
            private ScriptEnumerator m_ItorResult;
            private IEnumerator<KeyValuePair<object, ScriptValue>> m_Enumerator;
            public MapPairs(ScriptEnumerator itorResult, IEnumerator<KeyValuePair<object, ScriptValue>> enumerator) {
                this.m_ItorResult = itorResult;
                this.m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_ItorResult.SetKey(ScriptValue.CreateValue(m_ItorResult.script, value.Key));
                    m_ItorResult.SetValue(value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
            public void Dispose() {
                m_Enumerator.Dispose();
                m_Enumerator = null;
                m_ItorResult = null;
            }
        }
        private class StringMapPairs : ScorpioHandle, IDisposable {
            private ScriptEnumerator m_ItorResult;
            private IEnumerator<KeyValuePair<string, ScriptValue>> m_Enumerator;
            public StringMapPairs(ScriptEnumerator itorResult, IEnumerator<KeyValuePair<string, ScriptValue>> enumerator) {
                this.m_ItorResult = itorResult;
                this.m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var value = m_Enumerator.Current;
                    m_ItorResult.SetKey(ScriptValue.CreateValue(m_ItorResult.script, value.Key));
                    m_ItorResult.SetValue(value.Value);
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
            public void Dispose() {
                m_Enumerator.Dispose();
                m_Enumerator = null;
                m_ItorResult = null;
            }
        }
        private class UserdataPairs : ScorpioHandle, IDisposable {
            private ScriptEnumerator m_ItorResult;
            private IEnumerator m_Enumerator;
            public UserdataPairs(ScriptEnumerator itorResult, ScriptUserdata userdata) {
                m_ItorResult = itorResult;
                if (userdata.Value is IEnumerable)  {
                    m_Enumerator = ((IEnumerable)userdata.Value).GetEnumerator();
                } else {
                    throw new ExecutionException("pairs 只支持继承 IEnumerable 的类");
                }
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    m_ItorResult.SetValueUserdata(ScriptValue.CreateValue(m_ItorResult.script, m_Enumerator.Current));
                    return ScriptValue.True;
                }
                return ScriptValue.False;
            }
            public void Dispose() {
                if (m_Enumerator is IDisposable)
                    ((IDisposable)m_Enumerator).Dispose();
                m_Enumerator = null;
                m_ItorResult = null;
            }
        }
        public static void Load(Script script) {
            script.SetGlobalNoReference("print", script.CreateFunction(new print(script)));
            script.SetGlobalNoReference("printf", script.CreateFunction(new printf(script)));
            script.SetGlobalNoReference("pairs", script.CreateFunction(new pairs(script)));
            script.SetGlobalNoReference("alloc", script.CreateFunction(new alloc()));
            script.SetGlobalNoReference("free", script.CreateFunction(new free()));
            script.SetGlobalNoReference("gc", script.CreateFunction(new gc()));
            script.SetGlobalNoReference("clearVariables", script.CreateFunction(new clearVariables()));

            script.SetGlobalNoReference("isNull", script.CreateFunction(new isNull()));
            script.SetGlobalNoReference("isBoolean", script.CreateFunction(new isBoolean()));
            script.SetGlobalNoReference("isBool", script.CreateFunction(new isBoolean()));
            script.SetGlobalNoReference("isNumber", script.CreateFunction(new isNumber()));
            script.SetGlobalNoReference("isDouble", script.CreateFunction(new isDouble()));
            script.SetGlobalNoReference("isLong", script.CreateFunction(new isLong()));
            script.SetGlobalNoReference("isString", script.CreateFunction(new isString()));
            script.SetGlobalNoReference("isFunction", script.CreateFunction(new isFunction()));
            script.SetGlobalNoReference("isArray", script.CreateFunction(new isArray()));
            script.SetGlobalNoReference("isMap", script.CreateFunction(new isMap()));
            script.SetGlobalNoReference("isUserdata", script.CreateFunction(new isUserdata()));
            script.SetGlobalNoReference("isType", script.CreateFunction(new isType()));
            script.SetGlobalNoReference("isInstance", script.CreateFunction(new isInstance()));

            script.SetGlobalNoReference("toInt8", script.CreateFunction(new toNumber<sbyte>()));
            script.SetGlobalNoReference("toUint8", script.CreateFunction(new toNumber<byte>()));
            script.SetGlobalNoReference("toInt16", script.CreateFunction(new toNumber<short>()));
            script.SetGlobalNoReference("toUint16", script.CreateFunction(new toNumber<ushort>()));
            script.SetGlobalNoReference("toInt32", script.CreateFunction(new toInt32()));
            script.SetGlobalNoReference("toUint32", script.CreateFunction(new toNumber<uint>()));
            script.SetGlobalNoReference("toInt64", script.CreateFunction(new toInt64()));
            script.SetGlobalNoReference("toUint64", script.CreateFunction(new toNumber<ulong>()));
            script.SetGlobalNoReference("toFloat32", script.CreateFunction(new toNumber<float>()));
            script.SetGlobalNoReference("toFloat64", script.CreateFunction(new toDouble()));

            script.SetGlobalNoReference("toSbyte", script.CreateFunction(new toNumber<sbyte>()));
            script.SetGlobalNoReference("toByte", script.CreateFunction(new toNumber<byte>()));
            script.SetGlobalNoReference("toShort", script.CreateFunction(new toNumber<short>()));
            script.SetGlobalNoReference("toUshort", script.CreateFunction(new toNumber<ushort>()));
            script.SetGlobalNoReference("toInt", script.CreateFunction(new toInt32()));
            script.SetGlobalNoReference("toUint", script.CreateFunction(new toNumber<uint>()));
            script.SetGlobalNoReference("toLong", script.CreateFunction(new toInt64()));
            script.SetGlobalNoReference("toUlong", script.CreateFunction(new toNumber<ulong>()));
            script.SetGlobalNoReference("toFloat", script.CreateFunction(new toNumber<float>()));
            script.SetGlobalNoReference("toDouble", script.CreateFunction(new toDouble()));

            script.SetGlobalNoReference("toBool", script.CreateFunction(new toBoolean()));
            script.SetGlobalNoReference("toBoolean", script.CreateFunction(new toBoolean()));
            script.SetGlobalNoReference("toChar", script.CreateFunction(new toChar()));
            script.SetGlobalNoReference("toNumber", script.CreateFunction(new toDouble()));

            script.SetGlobalNoReference("toEnum", script.CreateFunction(new toEnum(script)));
            script.SetGlobalNoReference("toEnumString", script.CreateFunction(new toEnumString(script)));
            script.SetGlobalNoReference("toString", script.CreateFunction(new toString(script)));

            script.SetGlobalNoReference("toIndex", script.CreateFunction(new toIndex(script)));

            script.SetGlobalNoReference("typeOf", script.CreateFunction(new getPrototype(script)));
            //script.SetGlobal("setPrototype", script.CreateFunction(new setPrototype()));
            script.SetGlobalNoReference("getPrototype", script.CreateFunction(new getPrototype(script)));
            script.SetGlobalNoReference("setPropertys", script.CreateFunction(new setPropertys()));
            script.SetGlobalNoReference("createArray", script.CreateFunction(new createArray()));
            script.SetGlobalNoReference("getBase", script.CreateFunction(new getBase()));
            script.SetGlobalNoReference("clone", script.CreateFunction(new clone()));

            script.SetGlobalNoReference("require", script.CreateFunction(new require(script)));
            script.SetGlobalNoReference("setFastReflectClass", script.CreateFunction(new setFastReflectClass(script)));
            script.SetGlobalNoReference("isFastReflectClass", script.CreateFunction(new isFastReflectClass(script)));

            script.SetGlobalNoReference("pushSearch", script.CreateFunction(new pushSearch(script)));
            script.SetGlobalNoReference("pushAssembly", script.CreateFunction(new pushAssembly(script)));
            script.SetGlobalNoReference("importType", script.CreateFunction(new importType(script)));
            script.SetGlobalNoReference("importNamespace", script.CreateFunction(new importNamespace(script)));
            script.SetGlobalNoReference("importExtension", script.CreateFunction(new importExtension(script)));
            script.SetGlobalNoReference("genericType", script.CreateFunction(new genericType(script)));
            script.SetGlobalNoReference("genericMethod", script.CreateFunction(new genericMethod(script)));
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
            private Script script;
            public pairs(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var obj = args[0].valueType == ScriptValue.scriptValueType ? args[0].GetScriptValue(script) : null;
                var itorResult = script.NewEnumerator();
                try {
                    ScorpioHandle handle;
                    if (obj is ScriptArray) {
                        handle = new ArrayPairs(itorResult, ((ScriptArray)obj).GetEnumerator());
                    } else if (obj is ScriptMap) {
                        handle = new MapPairs(itorResult, ((ScriptMap)obj).GetEnumerator());
                    } else if (obj is ScriptHashSet) {
                        handle = new ArrayPairs(itorResult, ((ScriptHashSet)obj).GetEnumerator());
                    } else if (obj is ScriptUserdata) {
                        handle = new UserdataPairs(itorResult, ((ScriptUserdata)obj));
                    } else if (obj is ScriptInstance) {
                        handle = new StringMapPairs(itorResult, ((ScriptInstance)obj).GetEnumerator());
                    } else if (obj is ScriptType) {
                        handle = new StringMapPairs(itorResult, ((ScriptType)obj).GetEnumerator());
                    } else if (obj is ScriptGlobal) {
                        handle = new StringMapPairs(itorResult, ((ScriptGlobal)obj).GetEnumerator());
                    } else {
                        throw new ExecutionException("pairs 必须用于 array, map, type, global 或者 继承 IEnumerable 的 userdata 类型");
                    }
                    itorResult.Set(handle);
                } catch (System.Exception) {
                    itorResult.Free();
                    throw;
                }
                return new ScriptValue(itorResult, script);
            }
        }
        private class alloc : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    args[i].Reference();
                }
                return ScriptValue.Null;
            }
        }
        private class free : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    args[i].Release();
                }
                return ScriptValue.Null;
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
        private class clearVariables : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    var instance = args[0].Get<ScriptInstance>();
                    if (instance != null) instance.ClearVariables();
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
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue is ScriptFunction) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isArray : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue is ScriptArray) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isMap : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue is ScriptMap) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isUserdata : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue is ScriptUserdata) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue is ScriptType) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class isInstance : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].GetScriptValue.GetType() == typeof(ScriptInstance)) ? ScriptValue.True : ScriptValue.False;
            }
        }
        public class toNumber<T> : ScorpioHandle where T : struct, IConvertible {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateNumber(args[0].ToNumber<T>());
            }
        }
        private class toInt32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateNumber(args[0].ToInt32());
            }
        }
        private class toBoolean : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return Convert.ToBoolean(args[0].GetValue) ? ScriptValue.True : ScriptValue.False;
            }
        }
        private class toChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToChar());
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
                    return ScriptValue.CreateValue(script, Enum.Parse(type.Type, args[1].GetStringValue, ignoreCase));
                } else {
                    return ScriptValue.CreateValue(script, Enum.ToObject(type.Type, args[1].ToLong()));
                }
            }
        }
        private class toEnumString : ScorpioHandle {
            readonly Script script;
            public toEnumString(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScriptValue.CreateValue(script, Enum.ToObject(args[1].Get<ScriptUserdataEnumType>().Type, args[0].ToLong()).ToString());
            }
        }
        private class toString : ScorpioHandle {
            readonly Script script;
            public toString(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(args[0].ToString());
            }
        }
        private class toIndex : ScorpioHandle {
            readonly Script script;
            public toIndex(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var value = args[0];
                if (value.valueType == ScriptValue.scriptValueType) {
                    return new ScriptValue($"{value.GetScriptValue}  index:{value.index}  id:{value.GetScriptValue.Index}  count:{ScriptObjectReference.GetReferenceCount(value.index)}");
                } else if (value.valueType == ScriptValue.stringValueType) {
                    return new ScriptValue($"{value.GetStringValue}  index:{value.index}  count:{StringReference.GetReferenceCount(value.index)}");
                } else {
                    return new ScriptValue(value.ToString());
                }
            }
        }
        private class clone : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var deep = length > 1 ? args[1].IsTrue : true;
                if (args[0].valueType == ScriptValue.scriptValueType) {
                    return new ScriptValue(args[0].GetScriptValue.Clone(deep));
                } else {
                    return args[0].Reference();
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
                    case ScriptValue.nullValueType:
                        return ScriptValue.Null;
                    case ScriptValue.trueValueType:
                    case ScriptValue.falseValueType:
                        return script.TypeBooleanValue.Reference();
                    case ScriptValue.doubleValueType:
                    case ScriptValue.int64ValueType:
                        return script.TypeNumberValue.Reference();
                    case ScriptValue.stringValueType:
                        return script.TypeStringValue.Reference();
                    case ScriptValue.scriptValueType:
                        if (value.GetScriptValue is ScriptInstance) {
                            return new ScriptValue((value.GetScriptValue as ScriptInstance).Prototype);
                        } else if (value.GetScriptValue is ScriptType) {
                            return new ScriptValue((value.GetScriptValue as ScriptType).Prototype);
                        } else {
                            return script.GetUserdataTypeValue(value.GetScriptValue.Type).Reference();
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
                    if (value.GetScriptValue is ScriptInstance) {
                        return new ScriptValue((value.GetScriptValue as ScriptInstance).Prototype.Prototype);
                    } else if (value.GetScriptValue is ScriptType) {
                        return new ScriptValue((value.GetScriptValue as ScriptType).Prototype);
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
                    assembly = args[0].GetValue as Assembly;
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
                return script.GetUserdataTypeValue(args[0].ToString()).Reference();
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
                    script.LoadExtension(args[0].GetStringValue);
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
                    types[i - 1] = args[i].GetScriptValue.Type;
                }
                return script.GetUserdataType(args[0].GetScriptValue.Type).MakeGenericType(script, types).Reference();
            }
        }
        private class genericMethod : ScorpioHandle {
            private Script script;
            public genericMethod(Script script) {
                this.script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (args[0].valueType != ScriptValue.scriptValueType || !(args[0].GetScriptValue is ScriptMethodFunction)) {
                    throw new ExecutionException("generic_method 第1个参数必须是函数");
                }
                var types = new Type[length - 1];
                for (int i = 1; i < length; ++i) {
                    if (args[i].valueType != ScriptValue.scriptValueType)
                        throw new ExecutionException($"generic_method 第{i + 1}个参数必须是 Type");
                    types[i - 1] = args[i].GetScriptValue.Type;
                }
                var method = (args[0].GetScriptValue as ScriptMethodFunction).Method.MakeGenericMethod(types);
                if (method.IsStatic) {
                    return new ScriptValue(script.NewStaticMethod().Set(method.MethodName, method));
                } else {
                    return new ScriptValue(script.NewInstanceMethod().Set(method.MethodName, method));
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
                var fastClass = args[1].GetValue as IScorpioFastReflectClass;
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
