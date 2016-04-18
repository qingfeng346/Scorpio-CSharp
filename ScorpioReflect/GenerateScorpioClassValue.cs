using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //生成GetValue函数
        private string GenerateGetValue() {
            string fieldStr = @"        if (name == ""{0}"") return {1}.{0};";
            string methodStr = @"        if (name == ""{0}"") return {1}.GetInstance(m_Script, obj);";
            StringBuilder builder = new StringBuilder();
            bool first = true;
            //所有类变量
            foreach (var field in m_Fields) {
                if (first) { first = false; } else { builder.AppendLine(); }
                if (field.IsStatic) {
                    builder.AppendFormat(fieldStr, field.Name, m_FullName);
                } else {
                    builder.AppendFormat(fieldStr, field.Name, "((" + m_FullName + ")obj)");
                }
            }
            //实例属性
            foreach (var property in m_InatancePropertys) {
                if (property.CanRead && property.GetGetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, "((" + m_FullName + ")obj)");
                }
            }
            //静态属性
            foreach (var property in m_StaticPropertys) {
                if (property.CanRead && property.GetGetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, m_FullName);
                }
            }
            //所有的函数 排除event和属性函数
            foreach (var method in m_Methods) {
                if (m_PropertyEventMethods.Contains(method)) { continue; }
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(methodStr, method.Name, m_ScorpioClassName + "_" + method.Name);
            }
            return builder.ToString();
        }
        //生成SetValue函数
        private string GenerateSetValue() {
            string fieldStr = @"        if (name == ""{0}"") {{ {1}.{0} = ({2})(Util.ChangeType(m_Script, value, typeof({2}))); return; }}";
            StringBuilder builder = new StringBuilder();
            bool first = true;
            //类变量
            foreach (var field in m_Fields) {
                if (first) { first = false; } else { builder.AppendLine(); }
                if (field.IsStatic) {
                    builder.AppendFormat(fieldStr, field.Name, m_FullName, GetFullName(field.FieldType));
                } else {
                    builder.AppendFormat(fieldStr, field.Name, "((" + m_FullName + ")obj)", GetFullName(field.FieldType));
                }
            }
            //实例属性
            foreach (var property in m_InatancePropertys) {
                if (property.CanWrite && property.GetSetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, "((" + m_FullName + ")obj)", GetFullName(property.PropertyType));
                }
            }
            //静态属性
            foreach (var property in m_StaticPropertys) {
                if (property.CanWrite && property.GetSetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, m_FullName, GetFullName(property.PropertyType));
                }
            }
            return builder.ToString();
        }
    }
}
