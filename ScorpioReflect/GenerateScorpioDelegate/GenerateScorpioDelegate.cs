using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioDelegate {
        struct ComparerType : IComparer<Type> {
            public int Compare(Type x, Type y) {
                return x.FullName.CompareTo(y.FullName);
            }
        }
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private const string Template = @"using System;
using Scorpio;
using Scorpio.Userdata;
namespace __Namespace {
    public class __ClassName : DelegateTypeFactory {
        public static void Initialize() {
            ScriptUserdataDelegateType.SetFactory(new __ClassName());
        }
        public Delegate CreateDelegate(Script script, Type type, ScriptFunction func) {
__CreateDelegate
            throw new Exception(""Delegate Type is not found : "" + type + ""  func : "" + func);
        }
    }
}";
        public string Namespace = "ScorpioDelegate";
        public string ClassName = "ScorpioDelegateFactory";
        private List<Type> m_Delegates = new List<Type>();
        public void AddType(Type type) {
            if (!m_Delegates.Contains(type))
                m_Delegates.Add(type);
        }
        private string GetFullName(Type type) {
            var fullName = type.FullName;
            if (type.IsGenericType) {
                var index = fullName.IndexOf("`");
                fullName = fullName.Substring(0, index);
                fullName += "<";
                var types = type.GetGenericArguments();
                bool first = true;
                foreach (var t in types) {
                    if (first == false) { fullName += ","; } else { first = false; }
                    fullName += GetFullName(t);
                }
                fullName += ">";
            } else {
                fullName = fullName.Replace("+", ".");
            }
            return fullName;
        }
        public string Generate() {
            m_Delegates.Sort(new ComparerType());
            string str = Template;
            str = str.Replace("__Namespace", string.IsNullOrEmpty(Namespace) ? "ScorpioDelegate" : Namespace);
            str = str.Replace("__ClassName", string.IsNullOrEmpty(ClassName) ? "ScorpioDelegateFactory" : ClassName);
            str = str.Replace("__CreateDelegate", CreateDelegate());
            return str;
        }
        public string CreateDelegate() {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var type in m_Delegates) {
                if (!TYPE_DELEGATE.IsAssignableFrom(type)) { continue; }
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendLine("            if (type == typeof(__Name)".Replace("__Name", GetFullName(type)));
                builder.Append("                return new __Name(".Replace("__Name", GetFullName(type)));
                var InvokeMethod = type.GetMethod("Invoke");
                var parameters = InvokeMethod.GetParameters();
                string pars = "";
                bool firstPar = true;
                foreach (var parameter in parameters)
                {
                    if (firstPar) { firstPar = false; } else { pars += ","; }
                    pars += parameter.Name;
                }
                builder.Append("(" + pars + ") => { ");
                var returnType = InvokeMethod.ReturnType;
                if (returnType == typeof(void)) {
                    builder.Append("func.call(" + pars + "); ");
                } else if (returnType == typeof(bool)) {
                    builder.Append("return script.CreateObject(func.call(" + pars + ")).LogicOperation();");
                } else if (returnType == typeof(string)) {
                    builder.Append("return script.CreateObject(func.call(" + pars + ")).ToString();");
                } else if (returnType == typeof(sbyte) || returnType == typeof(byte) ||
                            returnType == typeof(short) || returnType == typeof(ushort) ||
                            returnType == typeof(int) || returnType == typeof(uint) ||
                            returnType == typeof(long) || returnType == typeof(ulong) ||
                            returnType == typeof(float) || returnType == typeof(double) || returnType == typeof(decimal)) {
                    string str = "return (__Type)Convert.ChangeType(script.CreateObject(func.call(" + pars + ")).ObjectValue, typeof(__Type));";
                    str = str.Replace("__Type", GetFullName(returnType));
                    builder.Append(str);
                } else if (typeof(ScriptObject).IsAssignableFrom(returnType)) {
                    builder.Append("return script.CreateObject(func.call(" + pars + "));");
                } else {
                    string str = "return (__Type)script.CreateObject(func.call(" + pars + ")).ObjectValue;";
                    str = str.Replace("__Type", GetFullName(returnType));
                    builder.Append(str);
                }
                builder.Append(" });");
            }
            return builder.ToString();
        }
    }
}
