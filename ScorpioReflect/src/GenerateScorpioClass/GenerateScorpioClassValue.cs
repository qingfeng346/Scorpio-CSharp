using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //反射列表,struct的value会使用反射
        string GenerateReflectList() {
            if (!IsStruct) { return ""; }
            var builder = new StringBuilder();
            foreach (var field in m_Fields) {
                if (field.IsStatic) { continue; }
                builder.Append($@"
    readonly FieldInfo _field_{field.Name} = typeof({FullName}).GetField(""{field.Name}"");");
            }
            foreach (var property in m_Propertys) {
                if (!property.CanWrite || property.GetSetMethod(false) == null || property.GetSetMethod(false).IsStatic) { continue; }
                builder.Append($@"
    readonly PropertyInfo _property_{property.Name} = typeof({FullName}).GetProperty(""{property.Name}"");");
            }
            return builder.ToString();
        }
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
            foreach (var name in m_MethodNames) {
                builder.AppendFormat(methodStr, name, ScorpioClassName + "_" + name);
            }
            return builder.ToString();
        }
        //生成SetValue函数
        private string GenerateSetValue() {
            var builder = new StringBuilder();
            var reflectFormat = @"
            case ""{0}"": {{ {1}.SetValue(obj, Util.ChangeType(value, typeof({2}))); return; }}";
            var normalFormat = @"
            case ""{0}"": {{ {1} = ({2})(Util.ChangeType(value, typeof({2}))); return; }}";
            //类变量
            foreach (var field in m_Fields) {
                if (field.IsInitOnly /*readonly 属性*/ || field.IsLiteral /*const 属性*/) { continue; }
                var fieldFullName = ScorpioReflectUtil.GetFullName(field.FieldType);
                if (IsStruct && !field.IsStatic) {
                    builder.AppendFormat(reflectFormat, field.Name, $"_field_{field.Name}", fieldFullName);
                } else {
                    builder.AppendFormat(normalFormat, field.Name, GetScorpioVariable(field.IsStatic, field.Name), fieldFullName);
                }
            }
            //类属性
            foreach (var property in m_Propertys) {
                MethodInfo setMethod;
                if (!property.CanWrite || (setMethod = property.GetSetMethod(false)) == null) { continue; }
                var propertyFullName = ScorpioReflectUtil.GetFullName(property.PropertyType);
                if (IsStruct && !setMethod.IsStatic) {
                    builder.AppendFormat(reflectFormat, property.Name, $"_property_{property.Name}", propertyFullName);
                } else {
                    builder.AppendFormat(normalFormat, property.Name, GetScorpioVariable(setMethod.IsStatic, property.Name), propertyFullName);
                }
            }
            return builder.ToString();
        }
    }
}
