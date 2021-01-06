using System;
using System.Reflection;
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
            if (type == typeof(ScriptValue)) { return value; }
            switch (value.valueType) {
                case ScriptValue.doubleValueType: {
                    unchecked {
                        if (type == typeof(double) || type == typeof(object)) { return value.doubleValue; }
                        if (type == typeof(long)) { return (long)value.doubleValue; }
                        if (type == typeof(ulong)) { return (ulong)value.doubleValue; }
                        if (type == typeof(int)) { return (int)value.doubleValue; }
                        if (type == typeof(float)) { return (float)value.doubleValue; }
                        if (type == typeof(string)) { return value.doubleValue.ToString(); }
                        if (type.IsEnum) { return Enum.ToObject(type, (int)value.doubleValue); }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:DoubleNumber  target:{type.ToString()}");
                }
                case ScriptValue.longValueType: {
                    unchecked {
                        if (type == typeof(long) || type == typeof(object)) { return value.longValue; }
                        if (type == typeof(double)) { return (double)value.longValue; }
                        if (type == typeof(ulong)) { return (ulong)value.longValue; }
                        if (type == typeof(int)) { return (int)value.longValue; }
                        if (type == typeof(float)) { return (float)value.longValue; }
                        if (type == typeof(string)) { return value.longValue.ToString(); }
                        if (type.IsEnum) { return Enum.ToObject(type, (int)value.longValue); }
                    }
                    throw new System.Exception($"其他数字类型请先转换再传入 source:LongNumber  target:{type.ToString()}");
                }
                case ScriptValue.nullValueType: return null;
                case ScriptValue.trueValueType: return true;
                case ScriptValue.falseValueType: return false;
                case ScriptValue.stringValueType: return value.stringValue;
                case ScriptValue.objectValueType: return value.objectValue;
                case ScriptValue.scriptValueType: {
                    if (value.scriptValue is ScriptFunction && typeof(Delegate).IsAssignableFrom (type)) {
                        return ScorpioDelegateFactory.CreateDelegate (type, value.scriptValue);
                    }
                    return value.scriptValue.Value;
                }
                default:
                    return value.Value;
            }
        }
        public static bool CanChangeTypeRefOut (ScriptValue value, Type type) {
            if (type == typeof(object) || type == typeof(ScriptValue)) return true;
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    return true;
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == typeof(bool);
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive && type != typeof(bool);
                case ScriptValue.stringValueType:
                    return type == typeof(string);
                case ScriptValue.objectValueType:
                    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return typeof(Delegate).IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
        public static bool CanChangeType (ScriptValue value, Type type) {
            if (type == typeof(object) || type == typeof(ScriptValue)) return true;
            switch (value.valueType) {
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == typeof(bool);
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive && type != typeof(bool);
                case ScriptValue.stringValueType:
                    return type == typeof(string);
                case ScriptValue.nullValueType:
                    return !type.IsValueType;
                case ScriptValue.objectValueType:
                    return type.IsAssignableFrom (value.objectValue.GetType ());
                default:
                    return typeof(Delegate).IsAssignableFrom (type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom (value.scriptValue.ValueType);
            }
        }
        public static string ParseJsonString (string value, bool ucode) {
            var builder = new StringBuilder ();
            builder.Append ('\"');
            if (ucode) {
                foreach (var c in value.ToCharArray()) {
                    switch (c) {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126)) {
                                builder.Append(c);
                            } else {
                                builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }
                            break;
                    }
                }
            } else {
                foreach (var c in value.ToCharArray()) {
                    switch (c) {
                        case '"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append("\\\\");
                            break;
                        case '\b':
                            builder.Append("\\b");
                            break;
                        case '\f':
                            builder.Append("\\f");
                            break;
                        case '\n':
                            builder.Append("\\n");
                            break;
                        case '\r':
                            builder.Append("\\r");
                            break;
                        case '\t':
                            builder.Append("\\t");
                            break;
                        default:
                            builder.Append(c);
                            break;
                    }
                }
            }
            builder.Append ('\"');
            return builder.ToString ();
        }
    }
}