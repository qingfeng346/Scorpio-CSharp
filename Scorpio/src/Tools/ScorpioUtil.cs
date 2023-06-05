using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Scorpio.Exception;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace Scorpio.Tools {
    public static class ScorpioUtil {
        public static readonly Type TYPE_VOID = typeof (void);
        public static readonly Type TYPE_OBJECT = typeof (object);
        public static readonly Type TYPE_VALUE = typeof (ScriptValue);
        public static readonly Type TYPE_TYPE = typeof (Type);
        public static readonly Type TYPE_DELEGATE = typeof (Delegate);
        public static readonly Type TYPE_BOOL = typeof (bool);
        public static readonly Type TYPE_STRING = typeof (string);
        public static readonly Type TYPE_SBYTE = typeof (sbyte);
        public static readonly Type TYPE_BYTE = typeof (byte);
        public static readonly Type TYPE_SHORT = typeof (short);
        public static readonly Type TYPE_USHORT = typeof (ushort);
        public static readonly Type TYPE_INT = typeof (int);
        public static readonly Type TYPE_UINT = typeof (uint);
        public static readonly Type TYPE_LONG = typeof (long);
        public static readonly Type TYPE_ULONG = typeof (ulong);
        public static readonly Type TYPE_FLOAT = typeof (float);
        public static readonly Type TYPE_DOUBLE = typeof (double);
        public static readonly Type TYPE_DECIMAL = typeof (decimal);
        public static readonly Type TYPE_PARAMATTRIBUTE = typeof (ParamArrayAttribute); //不定参属性
        public static readonly Type TYPE_EXTENSIONATTRIBUTE = typeof (ExtensionAttribute); //扩展函数属性
        [Conditional("SCORPIO_DEBUG")]
        public static void Assert(this bool condition, string message) {
            if (!condition) throw new ExecutionException(message);
        }
        public static int ReadBytes(this Stream stream, byte[] buffer) {
            int count = buffer.Length;
            int numRead = 0;
            do {
                int n = stream.Read(buffer, numRead, count);
                if (n == 0)
                    break;
                numRead += n;
                count -= n;
            } while (count > 0);
            return numRead;
        }
        //是否是不定参
        public static bool IsParams (this ParameterInfo info) { return info.IsDefined (TYPE_PARAMATTRIBUTE, false); }
        //是否是扩展函数
        public static bool IsExtensionMethod (this MemberInfo method) { return method.IsDefined (TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是包含扩展函数类
        public static bool IsExtensionType (this Type type) { return type.IsDefined (TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是还没有定义的模板函数
        public static bool IsGenericMethod (this MethodBase method) { return method.IsGenericMethod && method.ContainsGenericParameters; }
        //判断参数是否是 ref out 参数
        public static bool IsRetvalOrOut (this ParameterInfo parameterInfo) { return parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef; }
        //
        public static object ChangeType (this ScriptValue value, Type type) {
            if (ReferenceEquals(type, TYPE_VALUE)) { return value; }
            switch (value.valueType) {
                case ScriptValue.doubleValueType: {
                    unchecked {
                        if (ReferenceEquals(type, TYPE_INT)) { return (int)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_FLOAT)) { return (float)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_DOUBLE) || ReferenceEquals(type, TYPE_OBJECT)) { return value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_LONG)) { return (long)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_STRING)) { return value.doubleValue.ToString(); }
                        if (type.IsEnum) { return Enum.ToObject(type, (long)value.doubleValue); }
                        if (ReferenceEquals(type, TYPE_SBYTE)) { return (sbyte)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_BYTE)) { return (byte)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_SHORT)) { return (short)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_USHORT)) { return (ushort)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_UINT)) { return (uint)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_ULONG)) { return (ulong)value.doubleValue; }
                        if (ReferenceEquals(type, TYPE_BOOL)) { return value.doubleValue != 0; }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:DoubleNumber  target:{type}");
                }
                case ScriptValue.int64ValueType: {
                    unchecked {
                        if (ReferenceEquals(type, TYPE_INT)) { return (int)value.longValue; }
                        if (ReferenceEquals(type, TYPE_FLOAT)) { return (float)value.longValue; }
                        if (ReferenceEquals(type, TYPE_LONG) || ReferenceEquals(type, TYPE_OBJECT)) { return value.longValue; }
                        if (ReferenceEquals(type, TYPE_DOUBLE)) { return (double)value.longValue; }
                        if (ReferenceEquals(type, TYPE_STRING)) { return value.longValue.ToString(); }
                        if (type.IsEnum) { return Enum.ToObject(type, value.longValue); }
                        if (ReferenceEquals(type, TYPE_SBYTE)) { return (sbyte)value.longValue; }
                        if (ReferenceEquals(type, TYPE_BYTE)) { return (byte)value.longValue; }
                        if (ReferenceEquals(type, TYPE_SHORT)) { return (short)value.longValue; }
                        if (ReferenceEquals(type, TYPE_USHORT)) { return (ushort)value.longValue; }
                        if (ReferenceEquals(type, TYPE_UINT)) { return (uint)value.longValue; }
                        if (ReferenceEquals(type, TYPE_ULONG)) { return (ulong)value.longValue; }
                        if (ReferenceEquals(type, TYPE_BOOL)) { return value.longValue != 0; }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:LongNumber  target:{type}");
                }
                case ScriptValue.nullValueType: return null;
                case ScriptValue.trueValueType: return true;
                case ScriptValue.falseValueType: return false;
                case ScriptValue.stringValueType: return value.stringValue;
                //case ScriptValue.objectValueType: return value.objectValue;
                case ScriptValue.scriptValueType: {
                    if (value.scriptValue is ScriptFunction && TYPE_DELEGATE.IsAssignableFrom (type)) {
                        return ScorpioDelegateFactoryManager.CreateDelegate (type, value.scriptValue);
                    }
                    return value.scriptValue.Value;
                }
                default:
                    return value.Value;
            }
        }
        public static bool CanChangeTypeRefOut (this ScriptValue value, Type type) {
            if (ReferenceEquals(type, TYPE_OBJECT) || ReferenceEquals(type, TYPE_VALUE)) return true;
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    return true;
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return ReferenceEquals(type, TYPE_BOOL);
                case ScriptValue.doubleValueType:
                case ScriptValue.int64ValueType:
                    return type.IsPrimitive;
                case ScriptValue.stringValueType:
                    return ReferenceEquals(type, TYPE_STRING);
                //case ScriptValue.objectValueType:
                //    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return TYPE_DELEGATE.IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
        public static bool CanChangeType (this ScriptValue value, Type type) {
            if (ReferenceEquals(type, TYPE_OBJECT) || ReferenceEquals(type, TYPE_VALUE)) return true;
            switch (value.valueType) {
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return ReferenceEquals(type, TYPE_BOOL);
                case ScriptValue.doubleValueType:
                case ScriptValue.int64ValueType:
                    return type.IsPrimitive;
                case ScriptValue.stringValueType:
                    return ReferenceEquals(type, TYPE_STRING);
                case ScriptValue.nullValueType:
                    return !type.IsValueType;
                //case ScriptValue.objectValueType:
                //    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return TYPE_DELEGATE.IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
        public static string GetParametersString(this ScriptValue[] parameters, int length) {
            var builder = new StringBuilder();
            for (var i = 0; i < length; ++i) {
                try {
                    builder.Append($"[{parameters[i].valueType}-{parameters[i]}]");
                } catch (System.Exception) {
                    builder.Append($"[toString Error]");
                }
            }
            return builder.ToString();
        }
        public static ScriptValue[] CloneParameters(this ScriptValue[] parameters, int length) {
            var pars = new ScriptValue[length];
            Array.Copy(parameters, pars, length);
            return pars;
        }
        public static ScriptValue GetArgs(this ScriptValue[] parameters, int index, int length) {
            return GetArgs(parameters, index, length, ScriptValue.Null);
        }
        public static ScriptValue GetArgs(this ScriptValue[] parameters, int index, int length, ScriptValue def) {
            if (index < length) {
                return parameters[index];
            }
            return def;
        }
        public static ScriptValue GetArgsThrow(this ScriptValue[] parameters, int index, int length) {
            if (index < length) {
                return parameters[index];
            }
            throw new ExecutionException($"参数个数少于要获取的索引 index:{index} length:{length}");
        }
        public unsafe static void Free(ScriptValue[] values, int length) {
            fixed (ScriptValue* ptr = values) {
                for (var i = 0; i < length; i++) {
                    (ptr + i)->Free();
                }
            }
        }
        public static void Free<T>(this Dictionary<T, ScriptValue> values) {
            foreach (var pair in values) {
                pair.Value.Free();
            }
            values.Clear();
        }
    }
}