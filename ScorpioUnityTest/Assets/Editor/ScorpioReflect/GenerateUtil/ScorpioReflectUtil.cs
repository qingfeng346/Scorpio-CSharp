using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
namespace Scorpio.ScorpioReflect {
    public struct ComparerType : IComparer<Type> {
        public int Compare(Type x, Type y) {
            if (x == null || y == null) { return 0; }
            return ScorpioReflectUtil.GetFullName(x).CompareTo(ScorpioReflectUtil.GetFullName(y));
        }
    }
    public struct ComparerFieldInfo : IComparer<FieldInfo> {
        public int Compare(FieldInfo x, FieldInfo y) {
            return x.Name.CompareTo(y.Name);
        }
    }
    public struct ComparerEventInfo : IComparer<EventInfo> {
        public int Compare(EventInfo x, EventInfo y) {
            return x.Name.CompareTo(y.Name);
        }
    }
    public struct ComparerPropertyInfo : IComparer<PropertyInfo> {
        public int Compare(PropertyInfo x, PropertyInfo y) {
            return x.Name.CompareTo(y.Name);
        }
    }
    public struct ComparerMethodInfo : IComparer<MethodInfo> {
        public int Compare(MethodInfo x, MethodInfo y) {
            return x.Name.CompareTo(y.Name);
        }
    }
    public static class ScorpioReflectUtil {
        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        public static void SortType(this List<Type> types) {
            types.Sort(new ComparerType());
        }
        public static void SortField(this List<FieldInfo> fieldInfos) {
            fieldInfos.Sort(new ComparerFieldInfo());
        }
        public static void SortEvent(this List<EventInfo> eventInfos) {
            eventInfos.Sort(new ComparerEventInfo());
        }
        public static void SortProperty(this List<PropertyInfo> propertyInfos) {
            propertyInfos.Sort(new ComparerPropertyInfo());
        }
        public static void SortMethod(this List<MethodInfo> methodInfos) {
            methodInfos.Sort(new ComparerMethodInfo());
        }
        //获得一个类的完整名字
        public static string GetFullName(this Type type) {
            if (type.IsGenericParameter) {
                return GetFullName(type.BaseType);
            } else if (type == typeof(void)) {
                return "void";
            } else {
                var fullName = type.FullName;
                if (string.IsNullOrEmpty(fullName))
                    return "";
                fullName = fullName.Replace("+", ".");
                var builder = new StringBuilder();
                if (type.IsGenericType) {
                    if (type.IsGenericTypeDefinition) {
                        throw new System.Exception("未声明的模板类 : " + fullName);
                    }
                    var index = fullName.IndexOf("`");
                    builder.Append(fullName.Substring(0, index));
                    builder.Append("<");
                    var types = type.GetGenericArguments();
                    bool first = true;
                    foreach (var t in types) {
                        if (first == false) { builder.Append(","); } else { first = false; }
                        builder.Append(GetFullName(t));
                    }
                    builder.Append(">");
                } else {
                    builder.Append(fullName);
                }
                return builder.ToString();
            }
        }
        //获得最后生成的类的名字 把+和.都换成_
        public static string GetGenerateClassName(Type type) {
            var fullName = type.FullName;
            if (type.IsGenericType) {
                var index = fullName.IndexOf("`");
                fullName = fullName.Substring(0, index);
                fullName += "_";
                var types = type.GetGenericArguments();
                bool first = true;
                foreach (var t in types) {
                    if (first == false) { fullName += "_"; } else { first = false; }
                    fullName += GetGenerateClassName(t);
                }
            }
            fullName = fullName.Replace("+", "_");
            fullName = fullName.Replace(".", "_");
            return fullName;
        }

        //判断函数是否有未使用的模板定义
        public static bool CheckGenericMethod(MethodBase method) {
            if (!method.IsGenericMethod) { return true; }
            var genericArgs = new List<Type>(method.GetGenericArguments());
            var parameters = Array.ConvertAll(method.GetParameters(), _ => _.ParameterType);
            Array.ForEach(parameters, _ => genericArgs.Remove(_));
            return genericArgs.Count == 0;
        }
        public static string ReturnString(string invoke, Type returnType) {
            var returnFullName = returnType.GetFullName();
            if (returnType == typeof(void)) {
                return $"{invoke}";
            } else if (returnType == typeof(ScriptValue)) {
                return $"return {invoke}";
            } else if (returnType == typeof(bool)) {
                return $"return {invoke}.IsTrue";
            } else if (returnType == typeof(string)) {
                return $"return {invoke}.ToString()";
            } else if (returnType == typeof(sbyte) || returnType == typeof(byte) ||
                        returnType == typeof(short) || returnType == typeof(ushort) ||
                        returnType == typeof(int) || returnType == typeof(uint) ||
                        returnType == typeof(long) || returnType == typeof(ulong) ||
                        returnType == typeof(float) || returnType == typeof(double) || returnType == typeof(decimal)) {
                return $"return ({returnFullName})Convert.ChangeType({invoke}.Value, typeof({returnFullName}))";
            } else {
                return $"return ({returnFullName}){invoke}.Value";
            }        
        }
    }
}
