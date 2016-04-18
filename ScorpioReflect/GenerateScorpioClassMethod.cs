using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        private string GenerateConstructor() {
            var Constructors = m_Type.GetConstructors();
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var constructor in Constructors) {
                if (first) { first = false; } else { builder.AppendLine(); }
                string parameterTypes = GetScorpioMethodParameterTypes(constructor);
                string call = GetScorpioMethodCall(constructor);
                builder.AppendFormat("		    if (type == \"{0}\") return new __fullname{1};", parameterTypes, call);
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
                if (m_PropertyEventMethods.Contains(method)) { continue; }
                string name = method.Name;
                if (methods.Contains(name)) { continue; }
                methods.Add(name);
                builder.Append(GenerateMethodExecute(name));
            }
            return builder.ToString();
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
                string call = (method.IsStatic ? m_FullName : "((" + m_FullName + ")obj)");
                call += "." + name + GetScorpioMethodCall(method);
                var pars = method.GetParameters();
                if (method.ReturnType == typeof(void)) {
                    builder.AppendFormat("            if (type == \"{0}\") {{ {1}; return null; }}", parameterTypes, call);
                } else if (name == "op_Addition") {
                    builder.AppendFormat("            if (type == \"{0}\") return {1} + {2};", parameterTypes, "(" + GetFullName(pars[0].ParameterType) + ")args[0]", "(" + GetFullName(pars[1].ParameterType) + ")args[1]");
                } else if (name == "op_Subtraction") {
                    builder.AppendFormat("            if (type == \"{0}\") return {1} - {2};", parameterTypes, "(" + GetFullName(pars[0].ParameterType) + ")args[0]", "(" + GetFullName(pars[1].ParameterType) + ")args[1]");
                } else if (name == "op_Multiply") {
                    builder.AppendFormat("            if (type == \"{0}\") return {1} * {2};", parameterTypes, "(" + GetFullName(pars[0].ParameterType) + ")args[0]", "(" + GetFullName(pars[1].ParameterType) + ")args[1]");
                } else if (name == "op_Division") {
                    builder.AppendFormat("            if (type == \"{0}\") return {1} / {2};", parameterTypes, "(" + GetFullName(pars[0].ParameterType) + ")args[0]", "(" + GetFullName(pars[1].ParameterType) + ")args[1]");
                } else {
                    builder.AppendFormat("            if (type == \"{0}\") return {1};", parameterTypes, call);
                }
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
