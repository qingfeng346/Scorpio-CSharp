using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
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
        public static bool IsEnum(Type type) {
            return ScriptExtensions.IsEnum(type);
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
        public static bool IsIntObject(object obj)
        {
            return obj is int;
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
        public static object ChangeType(Script script, ScriptObject par, Type type)
        {
            if (type == TYPE_OBJECT) {
                return par.ObjectValue;
            } else {
                if (par is ScriptUserdata && Util.IsType(type))
                    return ((ScriptUserdata)par).ValueType;
                else if (par is ScriptNumber)
                    return ChangeType_impl(par.ObjectValue, type);
                else if (Util.IsDelegateType(type)) {
                    if (par is ScriptFunction)
                        return script.GetUserdataFactory().GetDelegate(type).Call(new ScriptObject[] { par });
                    else
                        return par.ObjectValue;
                } else
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
            if (type == TYPE_OBJECT)
                return true;
            else if (IsNumber(type))
                return par is ScriptNumber;
            else if (IsBool(type))
                return par is ScriptBoolean;
            else if (IsEnum(type))
                return par is ScriptEnum && ((ScriptEnum)par).EnumType == type;
            else if (par is ScriptNull)
                return true;
            else if (IsString(type))
                return par is ScriptString;
            else if (IsDelegateType(type))
                return (par is ScriptFunction) || (par is ScriptUserdata && (par as ScriptUserdata).ValueType == type);
            else if (IsType(type))
                return par is ScriptUserdata;
            else if (par is ScriptUserdata)
                return type.IsAssignableFrom(((ScriptUserdata)par).ValueType);
            else
                return type.IsAssignableFrom(par.GetType());
        }
        public static void WriteString(BinaryWriter writer, string str)
        {
            if (string.IsNullOrEmpty(str)) {
                writer.Write((byte)0);
            } else {
                writer.Write(Encoding.UTF8.GetBytes(str));
                writer.Write((byte)0);
            }
        }
        public static string ReadString(BinaryReader reader)
        {
            List<byte> sb = new List<byte>();
            byte ch;
            while ((ch = reader.ReadByte()) != 0)
                sb.Add(ch);
            byte[] buffer = sb.ToArray();
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length);
        }
        public static bool IsNullOrEmpty(String str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static object ChangeType_impl(object value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
        public static void Assert(bool b, Script script, string message)
        {
            if (!b) throw new ExecutionException(script, message);
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
