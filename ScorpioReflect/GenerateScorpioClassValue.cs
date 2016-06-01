using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //生成GetValue函数
        private string GenerateGetValue() {
            string fieldStr = @"        if (name == ""{0}"") return {1};";
            string methodStr = @"        if (name == ""{0}"") return {1}.GetInstance(m_Script, obj);";
            StringBuilder builder = new StringBuilder();
            bool first = true;
            //所有类变量
            foreach (var field in m_Fields) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(field, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; } 
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name));
            }
            //所有属性
            foreach (var property in m_Propertys) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(property, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; }
                if (property.CanRead && property.GetGetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetGetMethod(false).IsStatic, property.Name));
                }
            }
            //所有的函数
            foreach (var method in m_Methods) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(method, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; }
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(methodStr, method.Name, m_ScorpioClassName + "_" + method.Name);
            }
            return builder.ToString();
        }
        //生成SetValue函数
        private string GenerateSetValue() {
            string fieldStr = @"        if (name == ""{0}"") {{ {1} = ({2})(Util.ChangeType(m_Script, value, typeof({2}))); return; }}";
            StringBuilder builder = new StringBuilder();
            bool first = true;
            //类变量
            foreach (var field in m_Fields) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(field, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; }
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name), GetFullName(field.FieldType));
            }
            //所有属性
            foreach (var property in m_Propertys) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(property, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; }
                if (property.CanRead && property.GetSetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetSetMethod(false).IsStatic, property.Name), GetFullName(property.PropertyType));
                }
            }
            return builder.ToString();
        }
    }
}
