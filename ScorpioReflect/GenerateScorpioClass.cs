using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.ScorpioReflect {
    //过滤不生成的变量 函数 属性 和 事件 
    public interface ClassFilter {
        bool Check(FieldInfo fieldInfo, List<FieldInfo> fieldInfos, List<EventInfo> eventInfos, List<PropertyInfo> propertyInfos, List<MethodInfo> methodInfos);
        bool Check(EventInfo eventInfo, List<FieldInfo> fieldInfos, List<EventInfo> eventInfos, List<PropertyInfo> propertyInfos, List<MethodInfo> methodInfos);
        bool Check(PropertyInfo propertyInfo, List<FieldInfo> fieldInfos, List<EventInfo> eventInfos, List<PropertyInfo> propertyInfos, List<MethodInfo> methodInfos);
        bool Check(MethodInfo methodInfo, List<FieldInfo> fieldInfos, List<EventInfo> eventInfos, List<PropertyInfo> propertyInfos, List<MethodInfo> methodInfos);
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
        private List<FieldInfo> m_Fields = new List<FieldInfo>();                   //所有公共变量
        private List<EventInfo> m_Events = new List<EventInfo>();                   //所有的事件
        private List<PropertyInfo> m_Propertys = new List<PropertyInfo>();          //所有的属性
        private List<MethodInfo> m_Methods = new List<MethodInfo>();             	//所有函数
        public string ScorpioClassName { get { return m_ScorpioClassName; } }
        public GenerateScorpioClass(Type type) {
            m_Type = type;
            m_ScorpioClassName = "ScorpioClass_" + GetClassName(type);
            m_FullName = GetFullName(m_Type);
            m_Fields.AddRange(m_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            m_Fields.Sort(new ComparerFieldInfo());
            m_Events.AddRange(m_Type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            m_Events.Sort(new ComparerEventInfo());
            m_Propertys.AddRange(m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            m_Propertys.Sort(new ComparerPropertyInfo());
            MethodInfo[] methods = null;
            //IsAbstract和IsSealed同时为true的话为static class, 不是静态类不会同时有这两个属性
            if (m_Type.IsAbstract && m_Type.IsSealed) {
                methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            } else {
                methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            }
            foreach (var method in methods) {
                string name = method.Name;
                //屏蔽掉 =重载函数 IsSpecialName 表示为特殊函数 运算符重载 这个值会为true
                if (method.IsSpecialName && name == "op_Implicit") { continue; }
                bool RetvalOrOut = false;
                var pars = method.GetParameters();
                foreach (var par in pars) {
                    if (par.IsRetval || par.IsOut) {
                        RetvalOrOut = true;
                        break;
                    }
                }
                //屏蔽掉 带有 ref 和 out 关键字参数的函数
                if (RetvalOrOut) { continue; }
                m_Methods.Add(method);
            }
            m_Methods.Sort(new ComparerMethodInfo());
        }
        public void SetClassFilter(ClassFilter classFilter) {
            m_ClassFilter = classFilter;
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