using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        private Type m_Type;                        //类型
        private string m_ScorpioClassName;          //最终生成的类的名字
        private string m_FullName;                  //类的全名
        private FieldInfo[] m_Fields;               //所有公共变量
        private EventInfo[] m_InatanceEvents;       //所有实例event
        private EventInfo[] m_StaticEvents;         //所有静态event
        private PropertyInfo[] m_InatancePropertys; //所有实例属性
        private PropertyInfo[] m_StaticPropertys;   //所有静态属性
        private MethodInfo[] m_Methods;             //所有函数
        private List<MethodInfo> m_PropertyEventMethods = new List<MethodInfo>();       //所有event和属性函数
        public string ScorpioClassName { get { return m_ScorpioClassName; } }
        public GenerateScorpioClass(Type type) {
            m_Type = type;
            m_ScorpioClassName = "ScorpioClass_" + GetClassName(type);
            m_FullName = GetFullName(m_Type);
            m_Fields = m_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            m_InatanceEvents = m_Type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            m_StaticEvents = m_Type.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            m_InatancePropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            m_StaticPropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            MethodInfo[] methods = null;
            //IsAbstract和IsSealed同时为true的话为static class, 不是静态类不会同时有这两个属性
            if (type.IsAbstract && type.IsSealed) {
                methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            } else {
                methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            }
            List<MethodInfo> ms = new List<MethodInfo>();
            foreach (var method in methods) {
                string name = method.Name;
                if (name == "op_Equality" || name == "op_Inequality" || name == "op_Implicit")
                    continue;
                bool RetvalOrOut = false;
                var pars = method.GetParameters();
                foreach (var par in pars) {
                    if (par.IsRetval || par.IsOut) {
                        RetvalOrOut = true;
                        break;
                    }
                }
                if (RetvalOrOut) { continue; }
                //if (!method.IsGenericMethod) ms.Add(method);
                ms.Add(method);
            }
            m_Methods = ms.ToArray();

            foreach (var property in m_InatancePropertys) {
                if (property.CanWrite) { m_PropertyEventMethods.Add(property.GetSetMethod()); }
                if (property.CanRead) { m_PropertyEventMethods.Add(property.GetGetMethod()); }
            }
            foreach (var property in m_StaticPropertys) {
                if (property.CanWrite) { m_PropertyEventMethods.Add(property.GetSetMethod()); }
                if (property.CanRead) { m_PropertyEventMethods.Add(property.GetGetMethod()); }
            }
            foreach (var even in m_InatanceEvents) {
                m_PropertyEventMethods.Add(even.GetAddMethod());
                m_PropertyEventMethods.Add(even.GetRemoveMethod());
            }
            foreach (var even in m_StaticEvents) {
                m_PropertyEventMethods.Add(even.GetAddMethod());
                m_PropertyEventMethods.Add(even.GetRemoveMethod());
            }
        }
        public string Generate() {
            string str = ClassTemplate;
            str = str.Replace("__getvalue_content", GenerateGetValue());
            str = str.Replace("__setvalue_content", GenerateSetValue());
            str = str.Replace("__constructor_content", GenerateConstructor());
            str = str.Replace("__methods_content", GenerateMethod());
            str = str.Replace("__class", m_ScorpioClassName);
            str = str.Replace("__fullname", m_FullName);
            return str;
        }
    }
}