using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //生成GetValue函数
        private string GenerateGetValue() {
            string fieldStr = @"        if (name == ""{0}"") return {1};";
            string methodStr = @"        if (name == ""{0}"") return {1}.GetMethod(m_Script);";
            StringBuilder builder = new StringBuilder();
            bool first = true;
            //所有类变量
            foreach (var field in m_Fields) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name));
            }
            //所有属性
            foreach (var property in m_Propertys) {
                if (property.CanRead && property.GetGetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetGetMethod(false).IsStatic, property.Name));
                }
            }
            //所有的函数
            List<string> methods = new List<string>();
            foreach (var method in m_Methods) {
                string name = method.Name;
                if (methods.Contains(name)) { continue; }
                methods.Add(name);
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(methodStr, name, m_ScorpioClassName + "_" + name);
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
                if (field.IsInitOnly /*readonly 属性*/ || field.IsLiteral /*const 属性*/) { continue; }
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name), ScorpioReflectUtil.GetFullName(field.FieldType));
            }
            //所有属性
            foreach (var property in m_Propertys) {
                if (property.CanRead && property.GetSetMethod(false) != null) {
                    if (first) { first = false; } else { builder.AppendLine(); }
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetSetMethod(false).IsStatic, property.Name), ScorpioReflectUtil.GetFullName(property.PropertyType));
                }
            }
            return builder.ToString();
        }
    }
}
