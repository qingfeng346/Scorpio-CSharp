using System;
using System.Collections.Generic;
public class ScorpioReflectUtil {
    struct ComparerType : IComparer<Type> {
        public int Compare(Type x, Type y) {
            return x.FullName.CompareTo(y.FullName);
        }
    }
    public static string GetFullName(Type type) {
        return GetFullName(type, null);
    }
    //获得一个类的完整名字 模板类名字要计算一下
    public static string GetFullName(Type type, Type[] args) {
        var fullName = type.FullName;
        if (string.IsNullOrEmpty(fullName))
            return "";
        fullName = fullName.Replace("+", ".");
        if (type.IsGenericType) {
            if (!type.IsNested) {
                var index = fullName.IndexOf("`");
                fullName = fullName.Substring(0, index);
                fullName += "<";
                var types = args == null ? type.GetGenericArguments() : args;
                bool first = true;
                foreach (var t in types) {
                    if (first == false) { fullName += ","; } else { first = false; }
                    fullName += GetFullName(t, types);
                }
                fullName += ">";
            } else {
                int index = fullName.IndexOf("[");
                if (index >= 0) fullName = fullName.Substring(0, index);
                index = fullName.LastIndexOf("`");
                int num = int.Parse(fullName.Substring(index + 1, fullName.Length - index - 1));
                fullName = fullName.Substring(0, index);
                index = fullName.LastIndexOf(".");
                fullName = fullName.Substring(index + 1, fullName.Length - index - 1);
                var types = args == null ? type.GetGenericArguments() : args;
                fullName += "<";
                for (var i = 0; i < num;++i) {
                    if (i != 0) { fullName += ","; }
                    fullName += GetFullName(types[types.Length - num + i]);
                }
                fullName += ">";
                var tttt = new Type[types.Length - num];
                Array.Copy(types, 0, tttt, 0, tttt.Length);
                fullName = GetFullName(type.DeclaringType, tttt) + "." + fullName;
            }
        }
        return fullName;
    }
    public static void SortType(List<Type> types) {
        types.Sort(new ComparerType());
    }

}
