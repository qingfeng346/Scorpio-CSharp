using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        private static Dictionary<string, string> Operators = new Dictionary<string, string>() {
            {"op_Addition", "+"},
            {"op_Subtraction", "-"},
            {"op_Multiply", "*"},
            {"op_Division", "/"},
            {"op_Modulus", "%"},
            {"op_BitwiseOr", "|"},
            {"op_BitwiseAnd", "&"},
            {"op_ExclusiveOr", "^"},
            {"op_GreaterThan", ">"},
            {"op_LessThan", "<"},
            {"op_Equality", "=="},
            {"op_Inequality", "!="},
        };
        //获得构造函数
        private string GenerateConstructor() {
            var Constructors = m_Type.GetConstructors(ScorpioReflectUtil.BindingFlag);
            var builder = new StringBuilder();
            for (var i = 0; i < Constructors.Length; ++i) {
                builder.AppendFormat(@"
            case {0}: return new __fullname({1});", i, GetScorpioMethodCall(Constructors[i]));
            }
            string str = MethodTemplate;
            str = str.Replace("__getallmethod", GetAllMethod(Constructors));
            str = str.Replace("__name", m_ScorpioClassName + "_Constructor");
            str = str.Replace("__methodname", "Constructor");
            str = str.Replace("__execute", builder.ToString());
            return str;
        }
        private string GenerateMethod() {
            StringBuilder builder = new StringBuilder();
            List<string> methods = new List<string>();
            foreach (var method in m_Methods) {
                string name = method.Name;
                if (methods.Contains(name)) { continue; }
                methods.Add(name);
                builder.Append(GenerateMethodExecute(name));
            }
            return builder.ToString();
        }
        //运算符重载函数 + - * / [] 等
        private string GetSpeciaMethodExecute(MethodInfo method, string variable, ParameterInfo[] pars) {
            if (!method.IsSpecialName) { return ""; }
            string name = method.Name;
            if (Operators.ContainsKey(name)) {
                return string.Format("return {0} {1} {2};", GetScorpioMethodArgs(pars, 0), Operators[name], GetScorpioMethodArgs(pars, 1));
            }
            if (name == "get_Item") {
                return string.Format("return {0}[{1}];", variable, GetScorpioMethodArgs(pars, 0));
            } else if (name == "set_Item") {
                return string.Format("{0}[{1}] = {2}; return null;", variable, GetScorpioMethodArgs(pars, 0), GetScorpioMethodArgs(pars, 1));
            }
            return "";
        }
        private string GetEventMethodExecute(MethodInfo method, string variable, ParameterInfo[] pars) {
            foreach (var eve in m_AllEvents) {
                if (eve.GetAddMethod() == method) {
                    return string.Format("{0}.{1} += {2}; return null;", variable, eve.Name, GetScorpioMethodArgs(pars, 0));
                } else if (eve.GetRemoveMethod() == method) {
                    return string.Format("{0}.{1} -= {2}; return null;", variable, eve.Name, GetScorpioMethodArgs(pars, 0));
                }
            }
            return "";
        }
        private string GetPropertyMethodExecute(MethodInfo method, string variable, ParameterInfo[] pars) {
            foreach (var property in m_AllPropertys)
            {
                if (property.GetGetMethod() == method) {
                    return string.Format("return {0}.{1};", variable, property.Name);
                } else if (property.GetSetMethod() == method) {
                    return string.Format("{0}.{1} = {2}; return null;", variable, property.Name, GetScorpioMethodArgs(pars, 0));
                }
            }
            return "";
        }
        //生成一个函数的类
        private string GenerateMethodExecute(string name) {
            var methods = new List<MethodInfo>();
            foreach (var method in m_Methods) {
                if (method.Name == name) {
                    methods.Add(method);
                }
            }
            var builder = new StringBuilder();
            for (var i = 0; i < methods.Count; ++i) {
                var method = methods[i];
                string parameterCall = GetScorpioMethodCall(method);
                string variable = (method.IsStatic ? m_FullName : "((" + m_FullName + ")obj)");
                ParameterInfo[] pars = method.GetParameters();
                //运算符重载函数 + - * / [] 等
                string execute = GetSpeciaMethodExecute(method, variable, pars);
                if (!string.IsNullOrEmpty(execute)) { goto finish; }
                execute = GetEventMethodExecute(method, variable, pars);
                if (!string.IsNullOrEmpty(execute)) { goto finish; }
                execute = GetPropertyMethodExecute(method, variable, pars);
                if (!string.IsNullOrEmpty(execute)) { goto finish; }
                string call = variable + "." + name + "(" + parameterCall + ")";
                if (method.ReturnType == typeof(void)) {
                    execute = string.Format("{0}; return null;", call);
                } else {
                    execute = string.Format("return {0};", call);
                }
            finish:
                builder.AppendFormat(@"
            case {0}: {{ {1} }}", i, execute);
            }
            string str = MethodTemplate;
            str = str.Replace("__getallmethod", GetAllMethod(methods.ToArray()));
            str = str.Replace("__name", m_ScorpioClassName + "_" + name);
            str = str.Replace("__methodname", name);
            str = str.Replace("__execute", builder.ToString());
            return str;
        }
        string GenerateGetVariableType() {
            var templateStr = @"
            case ""{0}"": return typeof({1});";
            var builder = new StringBuilder();
            //所有类变量
            m_Fields.ForEach((field) => builder.AppendFormat(templateStr, field.Name, ScorpioReflectUtil.GetFullName(field.FieldType)) );
            //所有属性
            m_Propertys.ForEach((property) => builder.AppendFormat(templateStr, property.Name, ScorpioReflectUtil.GetFullName(property.PropertyType)) );
            //所有的函数
            var methods = new List<string>();
            foreach (var method in m_Methods) {
                string name = method.Name;
                if (methods.Contains(name) || method.ReturnType == typeof(void)) { continue; }
                methods.Add(name);
                builder.AppendFormat(templateStr, method.Name, ScorpioReflectUtil.GetFullName(method.ReturnType));
            }
            return builder.ToString();
        }
    }
}
