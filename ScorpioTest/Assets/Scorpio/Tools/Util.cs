using System;
using System.Reflection;
using System.Text;
using System.IO;
using Scorpio.Exception;
using System.Runtime.CompilerServices;
namespace Scorpio.Tools {
    public static class Util {
        public static readonly Type TYPE_VOID = typeof(void);
        public static readonly Type TYPE_OBJECT = typeof(object);
        public static readonly Type TYPE_VALUE = typeof(ScriptValue);
        public static readonly Type TYPE_TYPE = typeof(Type);
        public static readonly Type TYPE_DELEGATE = typeof(Delegate);
        public static readonly Type TYPE_BOOL = typeof(bool);
        public static readonly Type TYPE_STRING = typeof(string);
        public static readonly Type TYPE_SBYTE = typeof(sbyte);
        public static readonly Type TYPE_BYTE = typeof(byte);
        public static readonly Type TYPE_SHORT = typeof(short);
        public static readonly Type TYPE_USHORT = typeof(ushort);
        public static readonly Type TYPE_INT = typeof(int);
        public static readonly Type TYPE_UINT = typeof(uint);
        public static readonly Type TYPE_LONG = typeof(long);
        public static readonly Type TYPE_FLOAT = typeof(float);
        public static readonly Type TYPE_DOUBLE = typeof(double);
        public static readonly Type TYPE_DECIMAL = typeof(decimal);
        public static readonly Type TYPE_PARAMATTRIBUTE = typeof(ParamArrayAttribute);         //不定参属性
        public static readonly Type TYPE_EXTENSIONATTRIBUTE = typeof(ExtensionAttribute);      //扩展函数属性

        //是否是委托
        public static bool IsDelegate(Type type) { return TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type); }
        //是否是void
        public static bool IsVoid(Type type) { return type == TYPE_VOID; }
        //是否是不定参
        public static bool IsParams(ParameterInfo info) { return info.IsDefined(TYPE_PARAMATTRIBUTE, false); }
        //是否是扩展函数
        public static bool IsExtensionMethod(MemberInfo method) { return method.IsDefined(TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是包含扩展函数类
        public static bool IsExtensionType(Type type) { return type.IsDefined(TYPE_EXTENSIONATTRIBUTE, false); }
        //是否是还没有定义的模板函数
        public static bool IsGenericMethod(MethodBase method) { return method.IsGenericMethod && method.ContainsGenericParameters; }
        //判断参数是否是 ref out 参数
        public static bool IsRetvalOrOut(ParameterInfo parameterInfo) { return parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef; }
        public static object ChangeType(ScriptValue value, Type type) {
            if (type == TYPE_VALUE) { return value; }
            switch (value.valueType) {
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return Convert.ChangeType(value.Value, type);
                case ScriptValue.scriptValueType: {
                    if (value.scriptValue is ScriptFunction && TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type)) {
                        return ScorpioDelegateFactory.CreateDelegate(type, value.scriptValue);
                    }
                    return value.scriptValue.Value;
                }
                default: return value.Value;
            }
        }
        public static bool CanChangeTypeRefOut(ScriptValue value, Type type) {
            if (type == TYPE_OBJECT || type == TYPE_VALUE) return true;
            switch (value.valueType) {
                case ScriptValue.nullValueType:
                    return true;
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == TYPE_BOOL;
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive;
                case ScriptValue.stringValueType:
                    return type == TYPE_STRING;
                case ScriptValue.objectValueType:
                    return type.GetTypeInfo().IsAssignableFrom(value.objectValue.GetType());
                default: {
                    return TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type) ? value.scriptValue is ScriptFunction : type.GetTypeInfo().IsAssignableFrom(value.scriptValue.ValueType);
                }
            }
        }
        public static bool CanChangeType(ScriptValue value, Type type) {
            if (type == TYPE_OBJECT || type == TYPE_VALUE) return true;
            switch (value.valueType) {
                case ScriptValue.trueValueType:
                case ScriptValue.falseValueType:
                    return type == TYPE_BOOL;
                case ScriptValue.doubleValueType:
                case ScriptValue.longValueType:
                    return type.IsPrimitive;
                case ScriptValue.stringValueType:
                    return type == TYPE_STRING;
                case ScriptValue.nullValueType:
                    return !type.IsValueType;
                case ScriptValue.objectValueType:
                    return type.GetTypeInfo().IsAssignableFrom(value.objectValue.GetType());
                default: {
                    return TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type) ? value.scriptValue is ScriptFunction : type.GetTypeInfo().IsAssignableFrom(value.scriptValue.ValueType);
                }
            }
        }
        //public static void Assert(bool b, Script script, string message) {
        //    if (!b) throw new ExecutionException(script, message);
        //}
        //public static void WriteString(BinaryWriter writer, string str) {
        //    if (string.IsNullOrEmpty(str)) {
        //        writer.Write(0);
        //    } else {
        //        var bytes = Encoding.UTF8.GetBytes(str);
        //        writer.Write(bytes.Length);
        //        writer.Write(bytes);
        //    }
        //}
        //public static string ReadString(BinaryReader reader) {
        //    var length = reader.ReadInt32();
        //    if (length <= 0) { return ""; }
        //    return Encoding.UTF8.GetString(reader.ReadBytes(length));
        //}
        public static bool IsNullOrEmpty(String value) {
            return value == null || value.Length == 0;
        }
        //public static string Join(String separator, String[] stringarray) {
        //    int startindex = 0;
        //    int count = stringarray.Length;
        //    String result = "";
        //    for (int index = startindex; index < count; index++) {
        //        if (index > startindex)
        //            result += separator;
        //        result += stringarray[index];
        //    }
        //    return result;
        //}
        //public static object ChangeType_impl(object value, Type conversionType) {
        //    return Convert.ChangeType(value, conversionType);
        //}
        //public static object ToEnum(Type type, int value) {
        //    return Enum.ToObject(type, value);
        //}
        public static sbyte ToSByte(object value) { return Convert.ToSByte(value); }
        public static byte ToByte(object value) { return Convert.ToByte(value); }
        public static Int16 ToInt16(object value) { return Convert.ToInt16(value); }
        public static UInt16 ToUInt16(object value) { return Convert.ToUInt16(value); }
        public static Int32 ToInt32(object value) { return Convert.ToInt32(value); }
        public static UInt32 ToUInt32(object value) { return Convert.ToUInt32(value); }
        public static Int64 ToInt64(object value) { return Convert.ToInt64(value); }
        public static UInt64 ToUInt64(object value) { return Convert.ToUInt64(value); }
        public static float ToSingle(object value) { return Convert.ToSingle(value); }
        public static double ToDouble(object value) { return Convert.ToDouble(value); }
    }
}
