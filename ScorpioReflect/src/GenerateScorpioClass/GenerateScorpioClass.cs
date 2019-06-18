using System;
using System.Reflection;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio.ScorpioReflect {
    //过滤不生成的变量 函数 属性 和 事件 
    public interface ClassFilter {
        bool Check(GenerateScorpioClass generate, Type type, FieldInfo fieldInfo);
        bool Check(GenerateScorpioClass generate, Type type, EventInfo eventInfo);
        bool Check(GenerateScorpioClass generate, Type type, PropertyInfo propertyInfo);
        bool Check(GenerateScorpioClass generate, Type type, MethodInfo methodInfo);
    }
    public partial class GenerateScorpioClass {
        struct ComparerFieldInfo : IComparer<FieldInfo> {
            public int Compare(FieldInfo x, FieldInfo y) {
                return x.Name.CompareTo(y.Name);
            }
        }
        struct ComparerEventInfo : IComparer<EventInfo> {
            public int Compare(EventInfo x, EventInfo y) {
                return x.Name.CompareTo(y.Name);
            }
        }
        struct ComparerPropertyInfo : IComparer<PropertyInfo> {
            public int Compare(PropertyInfo x, PropertyInfo y) {
                return x.Name.CompareTo(y.Name);
            }
        }
        struct ComparerMethodInfo : IComparer<MethodInfo> {
            public int Compare(MethodInfo x, MethodInfo y) {
                return x.Name.CompareTo(y.Name);
            }
        }
        private Type m_Type;                        					            //类型
        private string m_ScorpioClassName;          					            //最终生成的类的名字
        private string m_FullName;                  					            //类的全名
        private ClassFilter m_ClassFilter;                                          //生成类过滤
        private List<FieldInfo> m_Fields = new List<FieldInfo>();                   //过滤的公共变量
        private List<FieldInfo> m_AllFields = new List<FieldInfo>();                //所有公共变量
        private List<EventInfo> m_Events = new List<EventInfo>();                   //过滤的事件
        private List<EventInfo> m_AllEvents = new List<EventInfo>();                //所有的事件
        private List<PropertyInfo> m_Propertys = new List<PropertyInfo>();          //过滤的属性
        private List<PropertyInfo> m_AllPropertys = new List<PropertyInfo>();       //所有的属性
        private List<MethodInfo> m_Methods = new List<MethodInfo>();             	//过滤的函数
        private List<MethodInfo> m_AllMethods = new List<MethodInfo>();             //所有的函数
        public List<FieldInfo> AllFields { get { return m_AllFields; } }
        public List<EventInfo> AllEvents { get { return m_AllEvents; } }
        public List<PropertyInfo> AllPropertys { get { return m_AllPropertys; } }
        public List<MethodInfo> AllMethods { get { return m_AllMethods; } }
        public string ScorpioClassName { get { return m_ScorpioClassName; } }
        public GenerateScorpioClass(Type type) {
            m_Type = type;
            m_ScorpioClassName = "ScorpioClass_" + GetClassName(type);
            m_FullName = ScorpioReflectUtil.GetFullName(m_Type);
            m_AllFields.AddRange(m_Type.GetFields(ScorpioReflectUtil.BindingFlag));
            m_AllEvents.AddRange(m_Type.GetEvents(ScorpioReflectUtil.BindingFlag));
            var propertys = m_Type.GetProperties(ScorpioReflectUtil.BindingFlag);
            foreach (var property in propertys) {
                //如果是 get 则参数是0个  set 参数是1个  否则就可能是 [] 的重载
                if ((property.CanRead && property.GetGetMethod().GetParameters().Length == 0) ||
                    (property.CanWrite && property.GetSetMethod().GetParameters().Length == 1)) {
                    m_AllPropertys.Add(property);
                }
            }
            var methods = (m_Type.IsAbstract && m_Type.IsSealed) ? m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy) : m_Type.GetMethods(ScorpioReflectUtil.BindingFlag);
            foreach (var method in methods) {
                //屏蔽掉 模版函数 模板函数只能使用反射
                if (Util.IsGenericMethod(method)) { continue; }
                m_AllMethods.Add(method);
            }
        }
        public void SetClassFilter(ClassFilter classFilter) {
            m_ClassFilter = classFilter;
        }
        void Init() {
            {
                m_Fields.Clear();
                foreach (var field in m_AllFields) {
                    if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, field)) { continue; }
                    m_Fields.Add(field);
                }
                m_Fields.Sort(new ComparerFieldInfo());
            }
            {
                m_Events.Clear();
                foreach (var eve in m_AllEvents) {
                    if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, eve)) { continue; }
                    m_Events.Add(eve);
                }
                m_Events.Sort(new ComparerEventInfo());
            }
            {
                m_Propertys.Clear();
                foreach (var property in m_AllPropertys) {
                    if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, property)) { continue; }
                    m_Propertys.Add(property);
                }
                m_Propertys.Sort(new ComparerPropertyInfo());
            }
            {
                m_Methods.Clear();
                foreach (var method in m_AllMethods) {
                    if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, method)) { continue; }
                    bool check = true;
                    foreach (var property in m_AllPropertys) {
                        if (property.GetGetMethod() == method || property.GetSetMethod() == method) {
                            if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, property)) {
                                check = false;
                                break;
                            }
                        }
                    }
                    if (check) {
                        foreach (var eve in m_AllEvents) {
                            if (eve.GetAddMethod() == method || eve.GetRemoveMethod() == method) {
                                if (m_ClassFilter != null && !m_ClassFilter.Check(this, m_Type, eve)) {
                                    check = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (check) m_Methods.Add(method);
                }
                m_Methods.Sort(new ComparerMethodInfo());
            }
        }
        public string Generate() {
            Init();
            return ClassTemplate.Replace("__getvariabletype_content", GenerateGetVariableType())
                    .Replace("__method_content", GenerateGetMethod())
                    .Replace("__getvalue_content", GenerateGetValue())
                    .Replace("__setvalue_content", GenerateSetValue())
                    .Replace("__constructor_content", GenerateConstructor())
                    .Replace("__methods_content", GenerateMethod())
                    .Replace("__class", m_ScorpioClassName)
                    .Replace("__fullname", m_FullName);
        }
    }
}