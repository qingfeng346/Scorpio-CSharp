using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //生成GetValue函数
        private string GenerateGetValue() {
            var fieldStr = @"
            case ""{0}"": return {1};";
            var methodStr = @"
            case ""{0}"": return {1}.GetInstance();";
            var builder = new StringBuilder();
            //所有类变量
            m_Fields.ForEach((field) => builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name)) );
            //所有属性
            foreach (var property in m_Propertys) {
                if (property.CanRead && property.GetGetMethod(false) != null) {
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetGetMethod(false).IsStatic, property.Name));
                }
            }
            //所有的函数
            var methods = new List<string>();
            foreach (var method in m_Methods) {
                string name = method.Name;
                if (methods.Contains(name)) { continue; }
                methods.Add(name);
                builder.AppendFormat(methodStr, name, m_ScorpioClassName + "_" + name);
            }
            return builder.ToString();
        }
        //生成SetValue函数
        private string GenerateSetValue() {
            var fieldStr = @"
            case ""{0}"": {{ {1} = ({2})(Util.ChangeType(value, typeof({2}))); return; }}";
            var builder = new StringBuilder();
            //类变量
            foreach (var field in m_Fields) {
                if (field.IsInitOnly /*readonly 属性*/ || field.IsLiteral /*const 属性*/) { continue; }
                builder.AppendFormat(fieldStr, field.Name, GetScorpioVariable(field.IsStatic, field.Name), ScorpioReflectUtil.GetFullName(field.FieldType));
            }
            //所有属性
            foreach (var property in m_Propertys) {
                if (property.CanRead && property.GetSetMethod(false) != null) {
                    builder.AppendFormat(fieldStr, property.Name, GetScorpioVariable(property.GetSetMethod(false).IsStatic, property.Name), ScorpioReflectUtil.GetFullName(property.PropertyType));
                }
            }
            return builder.ToString();
        }
    }
}
