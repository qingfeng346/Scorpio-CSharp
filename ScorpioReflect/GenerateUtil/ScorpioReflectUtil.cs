using System;
using System.Collections.Generic;
public class ScorpioReflectUtil {
    struct ComparerType : IComparer<Type> {
        public int Compare(Type x, Type y) {
            return x.FullName.CompareTo(y.FullName);
        }
    }
    //获得一个类的完整名字 模板类名字要计算一下
    public static string GetFullName(Type type) {
        var fullName = type.FullName;
        if (type.IsGenericType) {
            var index = fullName.IndexOf("`");
            fullName = fullName.Substring(0, index);
            fullName += "<";
            var types = type.GetGenericArguments();
            bool first = true;
            foreach (var t in types) {
                if (first == false) { fullName += ","; } else { first = false; }
                fullName += GetFullName(t);
            }
            fullName += ">";
        } else {
            fullName = fullName.Replace("+", ".");
        }
        return fullName;
    }
    public static void SortType(List<Type> types) {
        types.Sort(new ComparerType());
    }

}
