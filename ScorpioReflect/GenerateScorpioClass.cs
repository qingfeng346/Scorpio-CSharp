using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
public class GenerateScorpioClass
{
    public const string ClassTemplate = @"using System;
namespace __namespace {
    public class __class : Scorpio.Userdata.IScorpioReflectClass {
        public object GetValue(object obj, string name) {
__getvalue_content
            throw new Exception(""__fullname 找不到变量 : "" + name);
        }
        public void SetValue(object obj, string name, object value) {
__setvalue_content
            throw new Exception(""__fullname 找不到变量 : "" + name);
        }
    }
__methods_content
}";
    public const string MethodTemplate = @"
    public class __name : Scorpio.Userdata.IScorpioReflectMethod {
        private static __name _instance;
        public static __name GetInstance() { 
            if (_instance == null)
                _instance = new __name(); 
            return _instance; 
        }
        public object Call(object obj, string type, object[] args) {
__execute
            throw new Exception(""__fullname 找不到合适的函数 : __methodname    type : "" + type);
        }
    }";
    private Type m_Type;
    private string m_ScorpioClassName;
    private string m_FullName;
    private FieldInfo[] m_Fields;
    private PropertyInfo[] m_InatancePropertys;
    private PropertyInfo[] m_StaticPropertys;
    private MethodInfo[] m_Methods;
    private List<MethodInfo> m_PropertyMethods = new List<MethodInfo>();
    public string ScorpioClassName { get { return m_ScorpioClassName; } }
    public GenerateScorpioClass(Type type)
    {
        m_Type = type;
        m_ScorpioClassName = "ScorpioClass_" + m_Type.Namespace + "_" + m_Type.Name;
        m_FullName = m_Type.FullName;
        m_Fields = m_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        m_InatancePropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        m_StaticPropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Static);
        m_Methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        foreach (var property in propertys) {
            if (property.CanWrite) {
                m_PropertyMethods.Add(property.GetSetMethod());
            }
            if (property.CanRead) {
                m_PropertyMethods.Add(property.GetGetMethod());
            }
        }
    }
    public string Generate() {
        string str = ClassTemplate;
        str = str.Replace("__getvalue_content", GenerateGetValue());
        str = str.Replace("__setvalue_content", GenerateSetValue());
        str = str.Replace("__methods_content", GenerateMethod());
        str = str.Replace("__namespace", m_Type.Namespace);
        str = str.Replace("__class", m_ScorpioClassName);
        str = str.Replace("__fullname", m_FullName);
        return str;
    }
    public string GenerateGetValue()
    {
        string fieldStr = @"            if (name == ""{0}"") return {1}.{0};";
        string methodStr = @"            if (name == ""{0}"") return {1}.GetInstance();";
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (var field in m_Fields) {
            if (first) { first = false; } else { builder.AppendLine(); }
            if (field.IsStatic) {
                builder.AppendFormat(fieldStr, field.Name, m_FullName);
            } else {
                builder.AppendFormat(fieldStr, field.Name, "((" + m_FullName + ")obj)");
            }
        }
        PropertyInfo[] propertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in m_InatancePropertys) {
            if (property.CanRead) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, property.Name, "((" + m_FullName + ")obj)");
            }
        }
        foreach (var property in m_StaticPropertys) {
            if (property.CanRead) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, property.Name, m_FullName);
            }
        }
        foreach (var method in m_Methods) {
            if (m_PropertyMethods.Contains(method)) { continue; }
            if (first) { first = false; } else { builder.AppendLine(); }
            builder.AppendFormat(methodStr, method.Name, m_ScorpioClassName + "_" + method.Name);
        }
        return builder.ToString();
    }
    public string GenerateSetValue() {
        string fieldStr = @"            if (name == ""{0}"") {{ {1}.{0} = ({2})value; return; }}";
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (var field in m_Fields) {
            if (first) { first = false; } else { builder.AppendLine(); }
            if (field.IsStatic) {
                builder.AppendFormat(fieldStr, field.Name, m_FullName, field.FieldType.FullName);
            } else {
                builder.AppendFormat(fieldStr, field.Name, "((" + m_FullName + ")obj)", field.FieldType.FullName);
            }
        }
        foreach (var property in m_InatancePropertys) {
            if (property.CanWrite) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, property.Name, "((" + m_FullName + ")obj)", property.PropertyType.FullName);
            }
        }
        foreach (var property in m_StaticPropertys) {
            if (property.CanWrite) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.AppendFormat(fieldStr, property.Name, m_FullName, property.PropertyType.FullName);
            }
        }
        return builder.ToString();
    }
    public string GenerateMethod() {
        StringBuilder builder = new StringBuilder();
        List<string> methods = new List<string>();
        foreach (var method in m_Methods) {
            if (m_PropertyMethods.Contains(method)) { continue; }
            string name = method.Name;
            if (methods.Contains(name)) { continue; }
            methods.Add(name);
            string str = MethodTemplate;
            str = str.Replace("__name", m_ScorpioClassName + "_" + name);
            str = str.Replace("__methodname", name);
            str = str.Replace("__execute", GenerateMethodExecute(name));
            builder.Append(str);
        }
        return builder.ToString();
    }
    public string GenerateMethodExecute(string name) {
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (MethodInfo method in m_Methods) {
            if (method.Name != name) { continue; }
            if (first) { first = false; } else { builder.AppendLine(); }
            string par = "";
            var infos = method.GetParameters();
            for (int i = 0; i < infos.Length; ++i) {
                par += (infos[i].ParameterType.FullName + "+");
            }
            string call = method.IsStatic ? m_FullName : "((" + m_FullName + ")obj)";
            call += "." + name + "(";
            for (int i = 0; i < infos.Length; ++i) {
                if (i != 0) { call += ","; }
                call += "(" + infos[i].ParameterType.FullName + ")args[" + i + "]";
            }
            call += ")";
            if (method.ReturnType == typeof(void)) {
                builder.AppendFormat("            if (type == \"{0}\") {{ {1}; return null; }}", par, call);
            } else {
                builder.AppendFormat("            if (type == \"{0}\") return {1};", par, call);
            }
        }
        return builder.ToString();
    }
}