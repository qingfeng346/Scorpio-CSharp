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
        private string GenerateConstructor() {
            var Constructors = m_Type.GetConstructors();
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var constructor in Constructors) {
                if (first) { first = false; } else { builder.AppendLine(); }
                string parameterTypes = GetScorpioMethodParameterTypes(constructor);
                string call = GetScorpioMethodCall(constructor);
                builder.AppendFormat("		    if (type == \"{0}\") return new __fullname({1});", parameterTypes, call);
            }
            string str = MethodTemplate;
            str = str.Replace("__getallmethod", GetAllMethod(Constructors));
            str = str.Replace("__getmethod", InstanceMethodTemplate);
            str = str.Replace("__name", m_ScorpioClassName + "_Constructor");
            str = str.Replace("__methodstatic", "false");
            str = str.Replace("__methodname", "Constructor");
            str = str.Replace("__execute", builder.ToString());
            return str;
        }
        private string GenerateMethod() {
            StringBuilder builder = new StringBuilder();
            List<string> methods = new List<string>();
            foreach (var method in m_Methods) {
                if (m_ClassFilter != null && !m_ClassFilter.Check(method, m_Fields, m_Events, m_Propertys, m_Methods)) { continue; }
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
            foreach (var @event in m_Events) {
                if (@event.GetAddMethod() == method) {
                    return string.Format("{0}.{1} += {2}; return null;", variable, @event.Name, GetScorpioMethodArgs(pars, 0));
                } else if (@event.GetRemoveMethod() == method) {
                    return string.Format("{0}.{1} -= {2}; return null;", variable, @event.Name, GetScorpioMethodArgs(pars, 0));
                }
            }
            return "";
        }
        private string GetPropertyMethodExecute(MethodInfo method, string variable, ParameterInfo[] pars) {
            foreach (var property in m_Propertys)
            {
                if (property.GetGetMethod() == method) {
                    return string.Format("return {0}.{1};", variable, property.Name);
                } else if (property.GetSetMethod() == method) {
                    return string.Format("{0}.{1} = {2};", variable, property.Name, GetScorpioMethodArgs(pars, 0));
                }
            }
            return "";
        }
        private string GenerateMethodExecute(string name) {
            bool isStatic = false;
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (var method in m_Methods) {
                if (method.Name == name && !method.IsGenericMethod) {
                    methods.Add(method);
                    isStatic = method.IsStatic;
                }
            }
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var method in methods) {
                if (first) { first = false; } else { builder.AppendLine(); }
                string parameterTypes = GetScorpioMethodParameterTypes(method);
                string parameterCall = GetScorpioMethodCall(method);
                string variable = (method.IsStatic ? m_FullName : "((" + m_FullName + ")obj)");
                ParameterInfo[] pars = method.GetParameters();
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
                builder.AppendFormat("            if (type == \"{0}\") {{ {1} }}", parameterTypes, execute);
            }
            string str = MethodTemplate;
            str = str.Replace("__getallmethod", GetAllMethod(methods.ToArray()));
            str = str.Replace("__getmethod", isStatic ? StaticMethodTemplate : InstanceMethodTemplate);
            str = str.Replace("__name", m_ScorpioClassName + "_" + name);
            str = str.Replace("__methodstatic", isStatic ? "true" : "false");
            str = str.Replace("__methodname", name);
            str = str.Replace("__execute", builder.ToString());
            return str;
        }
    }
}
