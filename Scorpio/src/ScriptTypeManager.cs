using System;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Tools;
using Scorpio.Userdata;
namespace Scorpio {
    public partial class Script {
        private List<Assembly> m_Assembly = new List<Assembly>();                                                //所有代码集合
        private List<Type> m_ExtensionType = new List<Type>();                                                   //所有扩展类
        private Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();                   //所有的类集合
        private Dictionary<Type, ScriptValue> m_UserdataTypes = new Dictionary<Type, ScriptValue>();             //所有的类集合
        public UserdataType GetType(Type type) {
            if (m_Types.TryGetValue(type, out var value)) {
                return value;
            }
            var userdataType = new UserdataTypeReflect(type);
            LoadExtension(type, userdataType);
            return m_Types[type] = userdataType;
        }
        private void LoadExtension(Type type, UserdataTypeReflect userdataType) {
            foreach (var extensionType in m_ExtensionType) {
                var methods = extensionType.GetMethods(Script.BindingFlag);
                foreach (var method in methods) {
                    if (!method.IsExtensionMethod()) { continue; }
                    //第1个参数就是 this 类
                    if (method.GetParameters()[0].ParameterType.IsAssignableFrom(type)) {
                        userdataType.AddExtensionMethod(method);
                    }
                }
            }
        }
        public ScriptValue GetUserdataType(string name) {
            var type = LoadType(name);
            if (type == null) return ScriptValue.Null;
            return GetUserdataType(type);
        }
        public ScriptValue GetUserdataType(Type type) {
            if (m_UserdataTypes.TryGetValue(type, out var value)) {
                return value;
            }
            if (type.IsEnum)
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataEnumType(this, type));
            else if (ScorpioUtil.TYPE_DELEGATE.IsAssignableFrom(type))
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataDelegateType(this, type));
            else
                return m_UserdataTypes[type] = new ScriptValue(new ScriptUserdataType(this, type, GetType(type)));
        }
        public void SetFastReflectClass(Type type, IScorpioFastReflectClass value) {
            if (value == null || type == null) { return; }
            m_Types[type] = new UserdataTypeFastReflect(type, value);
        }
        public UserdataTypeFastReflect GetFastReflectClass(Type type) {
            if (type == null) { return null; }
            if (m_Types.TryGetValue(type, out var value)) {
                return value as UserdataTypeFastReflect;
            }
            return null;
        }
        public bool IsFastReflectClass(Type type) {
            if (type == null) { return false; }
            if (m_Types.TryGetValue(type, out var value)) {
                return value is UserdataTypeFastReflect;
            }
            return false;
        }
        public void PushAssembly(Assembly assembly) {
            if (assembly == null) return;
            if (!m_Assembly.Contains(assembly))
                m_Assembly.Add(assembly);
        }
        public Type LoadType(string name) {
            var type = Type.GetType(name, false, false);
            if (type != null) return type;
            for (int i = 0; i < m_Assembly.Count; ++i) {
                type = m_Assembly[i].GetType(name);
                if (type != null) return type;
            }
            return null;
        }
        //加载扩展类
        public void LoadExtension(string type) {
            LoadExtension(LoadType(type));
        }
        public void LoadExtension(Type type) {
            if (type == null || !type.IsExtensionType() || m_ExtensionType.Contains(type)) { return; }
            m_ExtensionType.Add(type);
            var methods = type.GetMethods(BindingFlag);
            foreach (var method in methods) {
                if (!method.IsExtensionMethod()) { continue; }
                //第1个参数就是 this 类
                var thisType = method.GetParameters()[0].ParameterType;
                foreach (var pair in m_Types) {
                    //thisType 是类型的基类
                    if (pair.Value is UserdataTypeReflect && thisType.IsAssignableFrom(pair.Key)) {
                        ((UserdataTypeReflect)pair.Value).AddExtensionMethod(method);
                    }
                }
            }
        }
    }
}
