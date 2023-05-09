using System;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Userdata;
namespace Scorpio {
    public static class ScorpioTypeManager {
        private static List<Assembly> m_Assembly = new List<Assembly>();                                                //所有代码集合
        private static HashSet<Type> m_ExtensionType = new HashSet<Type>();                                                   //所有扩展类
        private static Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();                   //所有的类集合
        private static Dictionary<Type, ScriptValue> m_UserdataTypes = new Dictionary<Type, ScriptValue>();             //所有的类集合
        public static UserdataType GetType(Type type) {
            if (m_Types.TryGetValue(type, out var value)) {
                return value;
            }
            var userdataType = new UserdataTypeReflect(type);
            //查找符合要求的已载入的扩展Type
            foreach (var extensionType in m_ExtensionType) {
                ForEachExtension(extensionType, (thisType, method) => {
                    if (thisType.IsAssignableFrom(type)) {
                        userdataType.AddExtensionMethod(method);
                    }
                });
            }
            return m_Types[type] = userdataType;
        }
        static void ForEachExtension(Type type, Action<Type, MethodInfo> action) {
            var methods = type.GetMethods(Script.BindingFlag);
            foreach (var method in methods) {
                if (!method.IsExtensionMethod()) { continue; }
                //第1个参数就是 type 的基类
                action(method.GetParameters()[0].ParameterType, method);
            }
        }
        //加载扩展类
        public static void LoadExtension(string type) {
            LoadExtension(LoadType(type));
        }
        //加载扩展类
        public static void LoadExtension(Type type) {
            if (type == null || m_ExtensionType.Contains(type) || !type.IsExtensionType()) { return; }
            m_ExtensionType.Add(type);
            ForEachExtension(type, (thisType, methodInfo) => {
                //查找符合要求的所有已载入的Type
                foreach (var pair in m_Types) {
                    if (pair.Value is UserdataTypeReflect && thisType.IsAssignableFrom(pair.Key)) {
                        ((UserdataTypeReflect)pair.Value).AddExtensionMethod(methodInfo);
                    }
                }
            });
        }
        public static ScriptValue GetUserdataType(string name) {
            var type = LoadType(name);
            if (type == null) return ScriptValue.Null;
            return GetUserdataType(type);
        }
        public static ScriptValue GetUserdataType(Type type) {
            if (m_UserdataTypes.TryGetValue(type, out var value)) {
                return value;
            }
            if (type.IsEnum)
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataEnumType(type));
            else if (ScorpioUtil.TYPE_DELEGATE.IsAssignableFrom(type))
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataDelegateType(type));
            else
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataType(type, GetType(type)));
        }
        //设置快速反射
        public static void SetFastReflectClass(Type type, IScorpioFastReflectClass value) {
            if (value == null || type == null) { return; }
            m_Types[type] = new UserdataTypeFastReflect(type, value);
        }
        public static UserdataTypeFastReflect GetFastReflectClass(Type type) {
            if (type == null) { return null; }
            if (m_Types.TryGetValue(type, out var value)) {
                return value as UserdataTypeFastReflect;
            }
            return null;
        }
        public static bool IsFastReflectClass(Type type) {
            if (type == null) { return false; }
            if (m_Types.TryGetValue(type, out var value)) {
                return value is UserdataTypeFastReflect;
            }
            return false;
        }
        public static void PushAssembly(Assembly assembly) {
            if (assembly == null) return;
            if (!m_Assembly.Contains(assembly))
                m_Assembly.Add(assembly);
        }
        public static Type LoadType(string name) {
            var type = Type.GetType(name, false, false);
            if (type != null) return type;
            for (int i = 0; i < m_Assembly.Count; ++i) {
                type = m_Assembly[i].GetType(name);
                if (type != null) return type;
            }
            return null;
        }
        
    }
}
