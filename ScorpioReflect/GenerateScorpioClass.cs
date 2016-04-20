using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        private Type m_Type;                        					            //类型
        private string m_ScorpioClassName;          					            //最终生成的类的名字
        private string m_FullName;                  					            //类的全名
        private List<FieldInfo> m_Fields = new List<FieldInfo>();                   //所有公共变量
        private List<EventInfo> m_InatanceEvents = new List<EventInfo>();           //所有实例event
        private List<EventInfo> m_StaticEvents = new List<EventInfo>();         	//所有静态event
        private List<PropertyInfo> m_InatancePropertys = new List<PropertyInfo>();  //所有实例属性
        private List<PropertyInfo> m_StaticPropertys = new List<PropertyInfo>();   	//所有静态属性
        private List<EventInfo> m_Events = new List<EventInfo>();                   //所有的event
        private List<PropertyInfo> m_Propertys = new List<PropertyInfo>();          //所有的属性
        private List<MethodInfo> m_Methods = new List<MethodInfo>();             	//所有函数
        private List<MethodInfo> m_PropertyEventMethods = new List<MethodInfo>();   //所有event和属性函数
		private List<string> m_Exclude = new List<string>();		//不导出的变量 属性 或 函数
        public string ScorpioClassName { get { return m_ScorpioClassName; } }
        public GenerateScorpioClass(Type type) {
            m_Type = type;
            m_ScorpioClassName = "ScorpioClass_" + GetClassName(type);
            m_FullName = GetFullName(m_Type);
            m_Events.AddRange(m_Type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            m_Propertys.AddRange(m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy));
        }
		public void AddExclude(string name) {
			if (!m_Exclude.Contains(name)) {
				m_Exclude.Add(name);
			}
		}
		public void DelExclude(string name) {
			m_Exclude.Remove(name);
		}
		public void ClearExclude() {
			m_Exclude.Clear();
		}
        public string Generate() {
			var fields = m_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			m_Fields.Clear();
			foreach (var field in fields) {
				if (!m_Exclude.Contains(field.Name)) {
					m_Fields.Add(field);
				}
			}
			var instanceEvents = m_Type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			m_InatanceEvents.Clear();
			foreach (var instanceEvent in instanceEvents) {
				if (!m_Exclude.Contains(instanceEvent.Name)) {
					m_InatanceEvents.Add(instanceEvent);
				}
			}
			var staticEvents = m_Type.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			m_StaticEvents.Clear();
			foreach (var staticEvent in staticEvents) {
				if (!m_Exclude.Contains(staticEvent.Name)) {
					m_StaticEvents.Add(staticEvent);
				}
			}
			var instancePropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			m_InatancePropertys.Clear();
			foreach (var instanceProperty in instancePropertys) {
				if (!m_Exclude.Contains(instanceProperty.Name)) {
					m_InatancePropertys.Add(instanceProperty);
				}
			}
			var staticPropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			m_StaticPropertys.Clear();
			foreach (var staticProperty in staticPropertys) {
				if (!m_Exclude.Contains(staticProperty.Name)) {
					m_StaticPropertys.Add(staticProperty);
				}
			}
			MethodInfo[] methods = null;
			//IsAbstract和IsSealed同时为true的话为static class, 不是静态类不会同时有这两个属性
			if (m_Type.IsAbstract && m_Type.IsSealed) {
				methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			} else {
				methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			}
			m_Methods.Clear();
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
				if (m_Exclude.Contains(name)) { continue; }
				m_Methods.Add(method);
			}
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