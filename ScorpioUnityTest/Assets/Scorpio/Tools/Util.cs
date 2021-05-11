using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Scorpio.Exception;
using System.Diagnostics;
namespace Scorpio.Tools {
    public static class Util {
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
        public static void Assert(bool condition, string message) {
            if (!condition) throw new ExecutionException(message);
        }
        [Conditional("SCORPIO_DEBUG")]
        public static void Assert(bool condition, string message, params object[] args) {
            if (!condition) throw new ExecutionException(string.Format(message, args));
        }
        //是否是不定参
        public static bool IsParams (ParameterInfo info) { return info.IsDefined (TYPE_PARAMATTRIBUTE, false); }
        //是否是扩展函数
        public static bool IsExtensionMethod (MemberInfo method) { return method.IsDefined (TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是包含扩展函数类
        public static bool IsExtensionType (Type type) { return type.IsDefined (TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是还没有定义的模板函数
        public static bool IsGenericMethod (MethodBase method) { return method.IsGenericMethod && method.ContainsGenericParameters; }
        //判断参数是否是 ref out 参数
        public static bool IsRetvalOrOut (ParameterInfo parameterInfo) { return parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef; }
        //
        public static object ChangeType (ScriptValue value, Type type) {
            if (type == TYPE_VALUE) { return value; }
            switch (value.valueType) {
                case ScriptValue.doubleValueType: {
                    unchecked {
                        if (type == TYPE_INT) { return (int)value.doubleValue; }
                        if (type == TYPE_FLOAT) { return (float)value.doubleValue; }
                        if (type == TYPE_DOUBLE || type == TYPE_OBJECT) { return value.doubleValue; }
                        if (type == TYPE_LONG) { return (long)value.doubleValue; }
                        if (type == TYPE_ULONG) { return (ulong)value.doubleValue; }
                        if (type.IsEnum) { return Enum.ToObject(type, (int)value.doubleValue); }
                        if (type == TYPE_STRING) { return value.doubleValue.ToString(); }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:DoubleNumber  target:{type}");
                }
                case ScriptValue.longValueType: {
                    unchecked {
                        if (type == TYPE_INT) { return (int)value.longValue; }
                        if (type == TYPE_FLOAT) { return (float)value.longValue; }
                        if (type == TYPE_LONG || type == TYPE_OBJECT) { return value.longValue; }
                        if (type == TYPE_DOUBLE) { return (double)value.longValue; }
                        if (type == TYPE_ULONG) { return (ulong)value.longValue; }
                        if (type.IsEnum) { return Enum.ToObject(type, (int)value.longValue); }
                        if (type == TYPE_STRING) { return value.longValue.ToString(); }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:LongNumber  target:{type}");
                }
                case ScriptValue.nullValueType: return null;
                case ScriptValue.trueValueType: return true;
                case ScriptValue.falseValueType: return false;
                case ScriptValue.stringValueType: return value.stringValue;
                case ScriptValue.objectValueType: return value.objectValue;
                case ScriptValue.scriptValueType: {
                    if (value.scriptValue is ScriptFunction && TYPE_DELEGATE.IsAssignableFrom (type)) {
                        return ScorpioDelegateFactory.CreateDelegate (type, value.scriptValue);
                    }
                    return value.scriptValue.Value;
                }
                default:
                    return value.Value;
            }
        }
        public static bool CanChangeTypeRefOut (ScriptValue value, Type type) {
            if (type == TYPE_OBJECT || type == TYPE_VALUE) return true;
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    return true;
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == TYPE_BOOL;
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive && type != TYPE_BOOL;
                case ScriptValue.stringValueType:
                    return type == TYPE_STRING;
                case ScriptValue.objectValueType:
                    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return TYPE_DELEGATE.IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
        public static bool CanChangeType (ScriptValue value, Type type) {
            if (type == TYPE_OBJECT || type == TYPE_VALUE) return true;
            switch (value.valueType) {
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == TYPE_BOOL;
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive && type != TYPE_BOOL;
                case ScriptValue.stringValueType:
                    return type == TYPE_STRING;
                case ScriptValue.nullValueType:
                    return !type.IsValueType;
                case ScriptValue.objectValueType:
                    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return TYPE_DELEGATE.IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
    }
}