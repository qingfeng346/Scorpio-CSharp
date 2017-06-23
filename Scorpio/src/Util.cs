using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
using System.Runtime.CompilerServices;
namespace Scorpio {
    public static class Util {
        private static readonly Type TYPE_VOID = typeof(void);
        private static readonly Type TYPE_OBJECT = typeof(object);
        private static readonly Type TYPE_TYPE = typeof(Type);
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private static readonly Type TYPE_BOOL = typeof(bool);
        private static readonly Type TYPE_STRING = typeof(string);
        private static readonly Type TYPE_SBYTE = typeof(sbyte);
        private static readonly Type TYPE_BYTE = typeof(byte);
        private static readonly Type TYPE_SHORT = typeof(short);
        private static readonly Type TYPE_USHORT = typeof(ushort);
        private static readonly Type TYPE_INT = typeof(int);
        private static readonly Type TYPE_UINT = typeof(uint);
        private static readonly Type TYPE_LONG = typeof(long);
        private static readonly Type TYPE_FLOAT = typeof(float);
        private static readonly Type TYPE_DOUBLE = typeof(double);
        private static readonly Type TYPE_DECIMAL = typeof(decimal);
        private static readonly Type TYPE_PARAMATTRIBUTE = typeof(ParamArrayAttribute);
        private static readonly Type TYPE_EXTENSIONATTRIBUTE = typeof(ExtensionAttribute);

        public static bool IsDelegateType(Type type) {
            return TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type);
        }
        public static bool IsVoid(Type type) {
            return type == TYPE_VOID;
        }
        public static bool IsParamArray(ParameterInfo info) {
            return info.IsDefined(TYPE_PARAMATTRIBUTE, false);
        }
        public static bool IsExtensionType(Type type) {
            return type.IsDefined(TYPE_EXTENSIONATTRIBUTE, false);
        }
        public static bool IsExtensionMethod(MemberInfo method) {
            return method.IsDefined(TYPE_EXTENSIONATTRIBUTE, false);
        }
        public static object ChangeType(Script script, ScriptObject par, Type type) {
            if (type == TYPE_OBJECT) {
                return par.ObjectValue;
            } else {
                if (par is ScriptUserdata && type == TYPE_TYPE)
                    return ((ScriptUserdata)par).ValueType;
                else if (par is ScriptNumber)
                    return ChangeType_impl(par.ObjectValue, type);
                else if (TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type)) {
                    if (par is ScriptFunction)
                        return script.GetDelegate(type).Call(new ScriptObject[] { par });
                    else
                        return par.ObjectValue;
                } else
                    return par.ObjectValue;
            }
        }
        public static bool CanChangeType(ScriptObject par, Type type) {
            if (type == TYPE_OBJECT)
                return true;
            else if (type == TYPE_SBYTE || type == TYPE_BYTE || type == TYPE_SHORT || type == TYPE_USHORT || type == TYPE_INT || type == TYPE_UINT ||
                    type == TYPE_FLOAT || type == TYPE_DOUBLE || type == TYPE_DECIMAL || type == TYPE_LONG)
                return par is ScriptNumber;
            else if (type == TYPE_BOOL)
                return par is ScriptBoolean;
            else if (type.GetTypeInfo().IsEnum)
                return par is ScriptEnum && ((ScriptEnum)par).EnumType == type;
            else if (par is ScriptNull)
                return true;
            else if (type == TYPE_STRING)
                return par is ScriptString;
            else if (type == TYPE_TYPE)
                return par is ScriptUserdata;
            else if (TYPE_DELEGATE.GetTypeInfo().IsAssignableFrom(type))
                return (par is ScriptFunction) || (par is ScriptUserdata && (par as ScriptUserdata).ValueType == type);
            else if (par is ScriptUserdata)
                return type.GetTypeInfo().IsAssignableFrom(((ScriptUserdata)par).ValueType);
            else
                return type.GetTypeInfo().IsAssignableFrom(par.GetType());
        }
        public static void Assert(bool b, Script script, string message) {
            if (!b) throw new ExecutionException(script, message);
        }
        public static void WriteString(BinaryWriter writer, string str) {
            if (string.IsNullOrEmpty(str)) {
                writer.Write((byte)0);
            } else {
                writer.Write(Encoding.UTF8.GetBytes(str));
                writer.Write((byte)0);
            }
        }
        public static string ReadString(BinaryReader reader) {
            List<byte> sb = new List<byte>();
            byte ch;
            while ((ch = reader.ReadByte()) != 0)
                sb.Add(ch);
            byte[] buffer = sb.ToArray();
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
        public static bool IsNullOrEmpty(String value) {
            return value == null || value.Length == 0;
        }
        public static string Join(String separator, String[] stringarray) {
            int startindex = 0;
            int count = stringarray.Length;
            String result = "";
            for (int index = startindex; index < count; index++) {
                if (index > startindex)
                    result += separator;
                result += stringarray[index];
            }
            return result;
        }
        public static object ChangeType_impl(object value, Type conversionType) {
            return Convert.ChangeType(value, conversionType);
        }
        public static object ToEnum(Type type, int value) {
            return Enum.ToObject(type, value);
        }
        public static sbyte ToSByte(object value) {
            return Convert.ToSByte(value);
        }
        public static byte ToByte(object value) {
            return Convert.ToByte(value);
        }
        public static Int16 ToInt16(object value) {
            return Convert.ToInt16(value);
        }
        public static UInt16 ToUInt16(object value) {
            return Convert.ToUInt16(value);
        }
        public static Int32 ToInt32(object value) {
            return Convert.ToInt32(value);
        }
        public static UInt32 ToUInt32(object value) {
            return Convert.ToUInt32(value);
        }
        public static Int64 ToInt64(object value) {
            return Convert.ToInt64(value);
        }
        public static UInt64 ToUInt64(object value) {
            return Convert.ToUInt64(value);
        }
        public static float ToSingle(object value) {
            return Convert.ToSingle(value);
        }
        public static double ToDouble(object value) {
            return Convert.ToDouble(value);
        }

    }
}
