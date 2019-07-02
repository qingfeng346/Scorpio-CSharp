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
        public static bool IsDelegate(Type type) { return TYPE_DELEGATE.IsAssignableFrom(type); }
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
                    return type.IsEnum ? Enum.ToObject(type, value.ToInt32()) : Convert.ChangeType(value.Value, type);
                case ScriptValue.scriptValueType: {
                    if (value.scriptValue is ScriptFunction && TYPE_DELEGATE.IsAssignableFrom(type)) {
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
                    return type.IsAssignableFrom(value.objectValue.GetType());
                default: {
                    return TYPE_DELEGATE.IsAssignableFrom(type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom(value.scriptValue.ValueType);
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
                    return type.IsAssignableFrom(value.objectValue.GetType());
                default: {
                    return TYPE_DELEGATE.IsAssignableFrom(type) ? value.scriptValue is ScriptFunction : type.IsAssignableFrom(value.scriptValue.ValueType);
                }
            }
        }
    }
}
