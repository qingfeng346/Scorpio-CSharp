using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Scorpio.Exception;
using Scorpio.Function;
using Scorpio.Userdata;
using Scorpio.Tools;
using System.Reflection;
namespace Scorpio.Library {
    public partial class LibraryBasis {
        private class ArrayPairs : ScorpioHandle, IDisposable {
            private ScriptEnumerator m_ItorResult;
            private IEnumerator<ScriptValue> m_Enumerator;
            private double m_Index;
            public ArrayPairs(ScriptEnumerator itorResult, IEnumerator<ScriptValue> enumerator) {
                m_Index = 0;
                m_Enumerator = enumerator;
                m_ItorResult = itorResult;
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
                m_ItorResult = itorResult;
                m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var current = m_Enumerator.Current;
                    m_ItorResult.SetKey(ScriptValue.CreateValue(current.Key));
                    m_ItorResult.SetValue(current.Value);
                    return ScriptValue.True;
                }
                m_Enumerator.Dispose();
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
                m_ItorResult = itorResult;
                m_Enumerator = enumerator;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                if (m_Enumerator.MoveNext()) {
                    var current = m_Enumerator.Current;
                    m_ItorResult.SetKey(new ScriptValue(current.Key));
                    m_ItorResult.SetValue(current.Value);
                    return ScriptValue.True;
                }
                m_Enumerator.Dispose();
                return ScriptValue.False;
            }
            public void Dispose() {
                m_Enumerator.Dispose();
                m_Enumerator = null;
                m_ItorResult = null;
            }
        }
        private class UserdataPairs : ScorpioHandle {
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
                    m_ItorResult.SetValue(ScriptValue.CreateValue(m_Enumerator.Current));
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
            script.SetGlobal("print", script.CreateFunction(new print(script)));
            script.SetGlobal("printf", script.CreateFunction(new printf(script)));
            script.SetGlobal("pairs", script.CreateFunction(new pairs(script)));
            script.SetGlobal("gc", script.CreateFunction(new gc(script)));
            script.SetGlobal("clearVariables", script.CreateFunction(new clearVariables()));

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

            script.SetGlobal("toBool", script.CreateFunction(new toBoolean()));
            script.SetGlobal("toBoolean", script.CreateFunction(new toBoolean()));
            script.SetGlobal("toChar", script.CreateFunction(new toChar()));

            script.SetGlobal("toFloat", script.CreateFunction(new toFloat()));
            script.SetGlobal("toDecimal", script.CreateFunction(new toDecimal()));

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
                var itorResult = new ScriptEnumerator(m_script);
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
                itorResult.SetNext(handle);
                return new ScriptValue(itorResult);
            }
        }
        private class gc : ScorpioHandle {
            private readonly Script m_script;
            public gc(Script script) {
                m_script = script;
            }
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var collect = args.GetArgs(0, length).IsTrue;
                m_script.ClearStack();
                if (collect) GC.Collect();
                return ScriptValue.Null;
            }
        }
        private class clearVariables: ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                for (var i = 0; i < length; ++i) {
                    var instance = args[0].Get<ScriptInstanceBase>();
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
                return (args[0].valueType == ScriptValue.scriptValueType && args[0].scriptValue is ScriptInstance) ? ScriptValue.True : ScriptValue.False;
            }
        }


        private class toInt8 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((sbyte)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((sbyte)args[0].longValue);
                    default: return new ScriptValue(Convert.ToSByte(args[0].Value));
                }
            }
        }
        private class toUint8 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((byte)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((byte)args[0].longValue);
                    default: return new ScriptValue(Convert.ToByte(args[0].Value));
                }
            }
        }
        private class toInt16 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((short)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((short)args[0].longValue);
                    default: return new ScriptValue(Convert.ToInt16(args[0].Value));
                }
            }
        }
        private class toUint16 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((ushort)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((ushort)args[0].longValue);
                    default: return new ScriptValue(Convert.ToUInt16(args[0].Value));
                }
            }
        }
        private class toInt32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((int)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((int)args[0].longValue);
                    default: return new ScriptValue(Convert.ToInt32(args[0].Value));
                }
            }
        }
        private class toUint32 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((uint)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((uint)args[0].longValue);
                    default: return new ScriptValue(Convert.ToUInt32(args[0].Value));
                }
            }
        }
        private class toInt64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((long)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue(args[0].longValue);
                    default: return new ScriptValue(Convert.ToInt64(args[0].Value));
                }
            }
        }
        private class toUint64 : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((ulong)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((ulong)args[0].longValue);
                    default: return new ScriptValue(Convert.ToUInt64(args[0].Value));
                }
            }
        }
        private class toBoolean : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return new ScriptValue(Convert.ToBoolean(args[0].Value));
            }
        }
        private class toChar : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((char)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((char)args[0].longValue);
                    default: return new ScriptValue(Convert.ToChar(args[0].Value));
                }
            }
        }

        private class toFloat : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((float)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((float)args[0].longValue);
                    default: return new ScriptValue(Convert.ToSingle(args[0].Value));
                }
            }
        }
        private class toDecimal : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue((decimal)args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((decimal)args[0].longValue);
                    default: return new ScriptValue(Convert.ToDecimal(args[0].Value));
                }
            }
        }
        private class toDouble : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                switch (args[0].valueType) {
                    case ScriptValue.doubleValueType: return new ScriptValue(args[0].doubleValue);
                    case ScriptValue.longValueType: return new ScriptValue((double)args[0].longValue);
                    default: return new ScriptValue(Convert.ToDouble(args[0].Value));
                }
            }
        }
        private class toEnum : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdataEnumType>();
                if (args[1].valueType == ScriptValue.stringValueType) {
                    var ignoreCase = length > 2 ? args[2].valueType == ScriptValue.trueValueType : false;
                    return ScriptValue.CreateValue(Enum.Parse(type.Type, args[1].stringValue, ignoreCase));
                } else {
                    return ScriptValue.CreateValue(Enum.ToObject(type.Type, args[1].ToLong()));
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
        private class setPrototype : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var scriptObject = args[0].Get();
                if (scriptObject == null) {
                    throw new ExecutionException("setPrototype 第1个参数必须是 ScriptObject");
                }
                var scriptType = args[1].Get<ScriptType>();
                if (scriptType == null) {
                    throw new ExecutionException("setPrototype 第2个参数必须是 ScriptType");
                }
                if (scriptObject is ScriptType) {
                    ((ScriptType)scriptObject).Prototype = scriptType;
                } else if (scriptObject is ScriptInstanceBase) {
                    ((ScriptInstanceBase)scriptObject).Prototype = scriptType;
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
                        if (value.scriptValue is ScriptInstanceBase) {
                            return new ScriptValue(((ScriptInstanceBase)value.scriptValue).Prototype);
                        } else if (value.scriptValue is ScriptType) {
                            return new ScriptValue(((ScriptType)value.scriptValue).Prototype);
                        } else {
                            return ScorpioTypeManager.GetUserdataType(value.scriptValue.Type);
                        }
                    default: return m_Script.TypeObjectValue;
                }
            }
        }
        private class setPropertys : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var source = args[0].Get<ScriptInstanceBase>();
                var target = args[1].Get<ScriptMap>();
                if (target != null) {
                    foreach (var pair in target) {
                        if (pair.Key is string) {
                            source.SetValue((string)pair.Key, pair.Value);
                        }
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
                    if (value.scriptValue is ScriptInstanceBase) {
                        return new ScriptValue(((ScriptInstanceBase)value.scriptValue).Prototype.Prototype);
                    } else if (value.scriptValue is ScriptType) {
                        return new ScriptValue(((ScriptType)value.scriptValue).Prototype);
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
                ScorpioTypeManager.PushAssembly(assembly);
                return ScriptValue.Null;
            }
        }
        private class importType : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScorpioTypeManager.GetUserdataType(args[0].ToString());
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
                    ScorpioTypeManager.LoadExtension(args[0].stringValue);
                } else {
                    var userdata = args[0].Get<ScriptUserdata>();
                    if (userdata != null) {
                        ScorpioTypeManager.LoadExtension(userdata.Type);
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
                return ScorpioTypeManager.GetType(args[0].scriptValue.Type).MakeGenericType(types);
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
                    return new ScriptValue(new ScriptStaticMethodFunction(method));
                } else {
                    return new ScriptValue(new ScriptInstanceMethodFunction(method));
                }
            }
        }
        private class setFastReflectClass : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                var type = args[0].Get<ScriptUserdata>().Type;
                var fastClass = args[1].Value as IScorpioFastReflectClass;
                ScorpioTypeManager.SetFastReflectClass(type, fastClass);
                return ScriptValue.Null;
            }
        }
        private class isFastReflectClass : ScorpioHandle {
            public ScriptValue Call(ScriptValue thisObject, ScriptValue[] args, int length) {
                return ScorpioTypeManager.IsFastReflectClass(args[0].Get<ScriptUserdata>().Type) ? ScriptValue.True : ScriptValue.False;
            }
        }
    }
}
