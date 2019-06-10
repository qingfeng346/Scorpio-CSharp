using System;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    public static class TypeManager {
        private static List<Assembly> m_Assembly = new List<Assembly>();       //所有代码集合
        private static Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();                                      //所有的类集合
        private static Dictionary<Type, ScriptValue> m_UserdataTypes = new Dictionary<Type, ScriptValue>();                                //所有的类集合
        public static UserdataType GetType(Type type) {
            if (m_Types.ContainsKey(type))
                return m_Types[type];
            return m_Types[type] = new UserdataTypeReflect(type);
        }
        public static ScriptValue GetUserdataType(string name) {
            Type type = LoadType(name);
            if (type == null) return ScriptValue.Null;
            return GetUserdataType(type);
        }
        public static ScriptValue GetUserdataType(Type type) {
            if (m_UserdataTypes.ContainsKey(type))
                return m_UserdataTypes[type];
            if (Util.IsDelegate(type))
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataDelegateType(type));
            else if (type.IsEnum)
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataEnumType(type));
            else
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataType(type, GetType(type)));
        }
        public static void SetFastReflectClass(Type type, ScorpioFastReflectClass value) {
            if (value == null || type == null) { return; }
            m_Types[type] = new UserdataTypeFastReflect(type, value);
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
        //加载扩展函数
        public static void LoadExtension(string type) {
            LoadExtension(LoadType(type));
        }
        public static void LoadExtension(Type type) {
            if (type == null) return;
            if (!Util.IsExtensionType(type)) return;
            var methods = type.GetMethods(Script.BindingFlag);
            foreach (var method in methods) {
                if (Util.IsExtensionMethod(method)) {
                    GetType(method.GetParameters()[0].ParameterType).AddExtensionMethod(method);
                }
            }
        }
    }
}
