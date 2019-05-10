using System;
using System.Reflection;
namespace Scorpio {
    public static class ScriptExtensions {
        public static Type GetTypeInfo(this Type type) {
            return type;
        }
        public static MethodInfo GetMethodInfo(this Delegate del) {
            return del.Method;
        }
    }
}
