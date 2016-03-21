using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
public class GenerateScorpioClass
{
    public const string ClassTemplate = @"using System;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Variable;
namespace __namespace {
    public class __class : IScorpioFastReflectClass {
        private Script m_Script;
        private MethodInfo[] m_Methods;                                 //所有函数
        public __class(Script script) {
            m_Script = script;
            m_Methods = typeof(__fullname).GetMethods(Script.BindingFlag);
        }
        public FastReflectUserdataMethod GetConstructor() {
            return __class_Constructor.GetMethod(m_Script, m_Methods);
        }
        public object GetValue(object obj, string name) {
__getvalue_content
            throw new Exception(""__fullname 找不到变量 : "" + name);
        }
        public void SetValue(object obj, string name, ScriptObject value) {
__setvalue_content
            throw new Exception(""__fullname 找不到变量 : "" + name);
        }
    }
__constructor_content
__methods_content
}";
    public const string MethodTemplate = @"
    public class __name : IScorpioFastReflectMethod {
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        public static FastReflectUserdataMethod GetMethod(Script script, MethodInfo[] methods) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(__methodtype, __methodstatic, script, typeof(__fullname), ""__methodname"", methods, new __name()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj, MethodInfo[] methods) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(__methodtype, __methodstatic, script, typeof(__fullname), ""__methodname"", methods, new __name()); 
            }__getmethod
        }
        public object Call(object obj, string type, object[] args) {
__execute
            throw new Exception(""__fullname 找不到合适的函数 : __methodname    type : "" + type);
        }
    }";
    public const string StaticMethodTemplate = @"
            if (_instance == null) {
                _instance = new ScorpioStaticMethod(""__methodname"", _method);
            }
            return _instance;
";
    public const string InstanceMethodTemplate = @"
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, ""__methodname"", _method, typeof(__fullname));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, ""__methodname"", _method);
";
    private Type m_Type;
    private string m_ScorpioClassName;
    private string m_FullName;
	private string m_Namespace;
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
		m_ScorpioClassName = m_ScorpioClassName.Replace(".", "_");
        m_FullName = m_Type.FullName;
		m_Namespace = string.IsNullOrEmpty(m_Type.Namespace) ? "ScorpioDefault" : m_Type.Namespace;
        m_Fields = m_Type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        m_InatancePropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        m_StaticPropertys = m_Type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		MethodInfo[] methods = null;
		//IsAbstract和IsSealed同时为true的话为static class, 不是静态类不会同时有这两个属性
		if (type.IsAbstract && type.IsSealed) {
			methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		} else {
			methods = m_Type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		}
        List<MethodInfo> ms = new List<MethodInfo>();
        foreach (var method in methods) {
			string name = method.Name;
			if (name == "op_Equality" || name == "op_Inequality" || name == "op_Implicit")
				continue;
            //if (!method.IsGenericMethod) ms.Add(method);
            ms.Add(method);
        }
        m_Methods = ms.ToArray();
        PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
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
        str = str.Replace("__constructor_content", GenerateConstructor());
        str = str.Replace("__methods_content", GenerateMethod());
		str = str.Replace("__namespace", m_Namespace);
        str = str.Replace("__class", m_ScorpioClassName);
        str = str.Replace("__fullname", m_FullName);
        return str;
    }
    private string GenerateGetValue()
    {
        string fieldStr = @"            if (name == ""{0}"") return {1}.{0};";
        string methodStr = @"            if (name == ""{0}"") return {1}.GetInstance(m_Script, obj, m_Methods);";
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
    private string GenerateSetValue() {
		string fieldStr = @"            if (name == ""{0}"") {{ {1}.{0} = ({2})(Util.ChangeType(m_Script, value, typeof({2}))); return; }}";
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (var field in m_Fields) {
            if (first) { first = false; } else { builder.AppendLine(); }
            if (field.IsStatic) {
                builder.AppendFormat(fieldStr, field.Name, m_FullName, GetFullName(field.FieldType));
            } else {
				builder.AppendFormat(fieldStr, field.Name, "((" + m_FullName + ")obj)", GetFullName(field.FieldType));
            }
        }
        foreach (var property in m_InatancePropertys) {
            if (property.CanWrite) {
                if (first) { first = false; } else { builder.AppendLine(); }
				builder.AppendFormat(fieldStr, property.Name, "((" + m_FullName + ")obj)", GetFullName(property.PropertyType));
            }
        }
        foreach (var property in m_StaticPropertys) {
            if (property.CanWrite) {
                if (first) { first = false; } else { builder.AppendLine(); }
				builder.AppendFormat(fieldStr, property.Name, m_FullName, GetFullName(property.PropertyType));
            }
        }
        return builder.ToString();
    }
    private string GenerateConstructor() {
        var Constructors = m_Type.GetConstructors();
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (var constructor in Constructors) {
            if (first) { first = false; } else { builder.AppendLine(); }
            string par = "";
            var infos = constructor.GetParameters();
            for (int i = 0; i < infos.Length; ++i) {
                par += (infos[i].ParameterType.FullName + "+");
            }
            string call = "";
            for (int i = 0; i < infos.Length; ++i) {
                if (i != 0) { call += ","; }
                call += "(" + infos[i].ParameterType.FullName + ")args[" + i + "]";
            }
            builder.AppendFormat("            if (type == \"{0}\") return new __fullname({1});", par, call);
        }
        string str = MethodTemplate;
        str = str.Replace("__getmethod", InstanceMethodTemplate);
        str = str.Replace("__name", m_ScorpioClassName + "_Constructor");
        str = str.Replace("__methodtype", "0");
        str = str.Replace("__methodstatic", "false");
        str = str.Replace("__methodname", "Constructor");
        str = str.Replace("__execute", builder.ToString());
        return str;
    }
    private string GenerateMethod() {
        StringBuilder builder = new StringBuilder();
        List<string> methods = new List<string>();
        foreach (var method in m_Methods) {
            if (m_PropertyMethods.Contains(method)) { continue; }
            string name = method.Name;
            if (methods.Contains(name)) { continue; }
            methods.Add(name);

            string str = MethodTemplate;
            if (method.IsStatic) {
                str = str.Replace("__getmethod", StaticMethodTemplate);
            } else {
                str = str.Replace("__getmethod", InstanceMethodTemplate);
            }
            str = str.Replace("__name", m_ScorpioClassName + "_" + name);
            str = str.Replace("__methodtype", "1");
            str = str.Replace("__methodstatic", method.IsStatic ? "true" : "false");
            str = str.Replace("__methodname", name);
            str = str.Replace("__execute", GenerateMethodExecute(name));
            builder.Append(str);
        }
        return builder.ToString();
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
    private string GenerateMethodExecute(string name) {
        StringBuilder builder = new StringBuilder();
        bool first = true;
        foreach (MethodInfo method in m_Methods) {
            if (method.Name != name) { continue; }
            if (method.IsGenericMethod) { continue; }
            if (first) { first = false; } else { builder.AppendLine(); }
            string par = "";
            var infos = method.GetParameters();
            for (int i = 0; i < infos.Length; ++i) {
				par += (infos[i].ParameterType.FullName + "+");
            }
            string call = (method.IsStatic ? m_FullName : "((" + m_FullName + ")obj)");
            call += "." + name + "(";
            for (int i = 0; i < infos.Length; ++i) {
                if (i != 0) { call += ","; }
				call += "(" + GetFullName(infos[i].ParameterType) + ")args[" + i + "]";
            }
            call += ")";
            if (method.ReturnType == typeof(void)) {
                builder.AppendFormat("            if (type == \"{0}\") {{ {1}; return null; }}", par, call);
            } else if (name == "op_Addition") {
				builder.AppendFormat("            if (type == \"{0}\") return {1} + {2};", par, "(" + GetFullName(infos[0].ParameterType) + ")args[0]", "(" + GetFullName(infos[1].ParameterType) + ")args[1]");
            } else if (name == "op_Subtraction") {
				builder.AppendFormat("            if (type == \"{0}\") return {1} - {2};", par, "(" + GetFullName(infos[0].ParameterType) + ")args[0]", "(" + GetFullName(infos[1].ParameterType) + ")args[1]");
            } else if (name == "op_Multiply") {
				builder.AppendFormat("            if (type == \"{0}\") return {1} * {2};", par, "(" + GetFullName(infos[0].ParameterType) + ")args[0]", "(" + GetFullName(infos[1].ParameterType) + ")args[1]");
            } else if (name == "op_Division") {
				builder.AppendFormat("            if (type == \"{0}\") return {1} / {2};", par, "(" + GetFullName(infos[0].ParameterType) + ")args[0]", "(" + GetFullName(infos[1].ParameterType) + ")args[1]");
            } else {
                builder.AppendFormat("            if (type == \"{0}\") return {1};", par, call);
            }
        }
        return builder.ToString();
    }
}