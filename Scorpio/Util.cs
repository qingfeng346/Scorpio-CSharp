using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using Scorpio;
using Scorpio.Variable;
using Scorpio.Exception;
namespace Scorpio
{
    public static class Util
    {
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
        public static readonly Assembly MSCORLIB_ASSEMBLY = TYPE_OBJECT.Assembly;

        public static void SetObject(Dictionary<object, ScriptObject> variables, object key, ScriptObject obj)
        {
            variables[key] = obj.Assign();
        }
        public static void SetObject(Dictionary<string, ScriptObject> variables, string key, ScriptObject obj)
        {
            variables[key] = obj.Assign();
        }
        public static bool IsBool(Type type)
        {
            return type == TYPE_BOOL;
        }
        public static bool IsString(Type type)
        {
            return type == TYPE_STRING;
        }
        public static bool IsDouble(Type type)
        {
            return type == TYPE_DOUBLE;
        }
        public static bool IsLong(Type type)
        {
            return type == TYPE_LONG;
        }
        public static bool IsNumber(Type type)
        {
            return (type == TYPE_SBYTE || type == TYPE_BYTE ||
                    type == TYPE_SHORT || type == TYPE_USHORT ||
                    type == TYPE_INT || type == TYPE_UINT ||
                    type == TYPE_FLOAT || type == TYPE_DOUBLE ||
                    type == TYPE_DECIMAL || type == TYPE_LONG);
        }
        public static bool IsEnum(Type type)
        {
            return type.IsEnum;
        }
        public static bool IsDelegateType(Type type)
        {
            return TYPE_DELEGATE.IsAssignableFrom(type);
        }
        public static bool IsVoid(Type type)
        {
            return type == TYPE_VOID;
        }
        public static bool IsType(Type type)
        {
            return type == TYPE_TYPE;
        }
        public static bool IsBoolObject(object obj)
        {
            return obj is bool;
        }
        public static bool IsStringObject(object obj)
        {
            return obj is string;
        }
        public static bool IsDoubleObject(object obj)
        {
            return obj is double;
        }
        public static bool IsLongObject(object obj)
        {
            return obj is long;
        }
        public static bool IsNumberObject(object obj)
        {
            return (obj is sbyte || obj is byte ||
                    obj is short || obj is ushort ||
                    obj is int || obj is uint ||
                    obj is float || obj is double ||
                    obj is decimal || obj is long);
        }
        public static bool IsEnumObject(object obj)
        {
            return IsEnum(obj.GetType());
        }
        public static bool IsParamArray(ParameterInfo info)
        {
            return info.IsDefined(TYPE_PARAMATTRIBUTE, false);
        }

        public static object ChangeType(ScriptObject par, Type type)
        {
            if (type == TYPE_OBJECT) {
                return par.ObjectValue;
            } else {
                if (par is ScriptUserdata && Util.IsType(type))
                    return ((ScriptUserdata)par).ValueType;
                else if (par is ScriptNumber)
                    return ChangeType_impl(par.ObjectValue, type);
                else
                    return par.ObjectValue;
            }
        }
        public static bool CanChangeType(ScriptObject[] pars, Type[] types)
        {
            if (pars.Length != types.Length) 
                return false;
            for (int i = 0; i < pars.Length;++i ) {
                if (!CanChangeType(pars[i], types[i]))
                    return false;
            }
            return true;
        }
        public static bool CanChangeType(ScriptObject par, Type type)
        {
            if (type == TYPE_OBJECT || par.IsNull) {
                return true;
            } else {
                if (par is ScriptString && Util.IsString(type)) {
                    return true;
                } else if (par is ScriptNumber && IsNumber(type)) {
                    return true;
                } else if (par is ScriptBoolean && IsBool(type)) {
                    return true;
                } else if (par is ScriptEnum && (par as ScriptEnum).EnumType == type) {
                    return true;
                } else if (par is ScriptFunction && IsDelegateType(type)) {
                    return true;
                } else if (par is ScriptUserdata) {
                    if (Util.IsType(type) || type.IsAssignableFrom(((ScriptUserdata)par).ValueType))
                        return true;
                } else if (type.IsAssignableFrom(par.GetType())) {
                    return true;
                }
            }
            return false;
        }
        public static String GetFileString(String fileName, Encoding encoding)
        {
            FileStream stream = File.OpenRead(fileName);
            long length = stream.Length;
            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();
            return encoding.GetString(buffer, 0, buffer.Length);
        }
        public static bool IsNullOrEmpty(String str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static object ChangeType_impl(object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
        public static void Assert(bool b)
        {
            Assert(b, "");
        }
        public static void Assert(bool b, string message)
        {
            if (!b) throw new ExecutionException(message);
        }
        public static int ToInt32(object value)
        {
            return Convert.ToInt32(value);
        }
        public static double ToDouble(object value)
        {
            return Convert.ToDouble(value);
        }
        public static long ToInt64(object value)
        {
            return Convert.ToInt64(value);
        }
    }
}
