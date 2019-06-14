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
            UserdataType value;
            if (m_Types.TryGetValue(type, out value)) {
                return value;
            }
            return m_Types[type] = new UserdataTypeReflect(type);
        }
        public static ScriptValue GetUserdataType(string name) {
            var type = LoadType(name);
            if (type == null) return ScriptValue.Null;
            return GetUserdataType(type);
        }
        public static ScriptValue GetUserdataType(Type type) {
            ScriptValue value;
            if (m_UserdataTypes.TryGetValue(type, out value)) {
                return value;
            }
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
        public static UserdataTypeFastReflect GetFastReflectClass(Type type) {
            if (type == null) { return null; }
            UserdataType value;
            if (m_Types.TryGetValue(type, out value)) {
                return value as UserdataTypeFastReflect;
            }
            return null;
        }
        public static bool IsFastReflectClass(Type type) {
            if (type == null) { return false; }
            UserdataType value;
            if (m_Types.TryGetValue(type, out value)) {
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
