using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScorpioReflect
{
    public class Test
    {
        public int a;
        public int b;
        public int GetA()
        {
            return a;
        }
        public void Func()
        {

        }
        public int AAAA { set { a = value; } }
        public static int c;
        public string dddd { get; set; }
        public static int eee { get { return c; } }
        public void TestFunc(int a, int b)
        {

        }
        public void TestFunc(string b, int c)
        {

        }
    }
    public struct Vector2
    {
        public float x;
        public float y;
    }
    class Program
    {
        static void Main(string[] args)
        {
            Out(typeof(Test));
            Out(typeof(Vector2));
        }
        static List<MethodInfo> pInfos = new List<MethodInfo>();
        static void Out(Type type)
        {
            pInfos.Clear();
            string str = @"namespace __namespace {
    public class __class {
        public object GetValue(object obj, string name) {
__getvalue_content
            return null;
        }
        public void SetValue(object obj, string name, object value) {
__setvalue_content
        }
    }
__methods_content
}";
            string name = "ScorpioClass_" + type.Namespace + "_" + type.Name;
            str = str.Replace("__namespace", type.Namespace);
            str = str.Replace("__class", name);
            str = str.Replace("__fullname", type.FullName);
            str = str.Replace("__getvalue_content", GetValue(name, type));
            str = str.Replace("__setvalue_content", SetValue(name, type));
            str = str.Replace("__methods_content", GetMethods(name, type));
            File.WriteAllText(@"C:\Users\qingf\Desktop\ConsoleApplication1\ConsoleApplication1\" + name + ".cs", str);
        }
        static string GetValue(string name, Type type)
        {
            string fieldStr = @"            if (name == ""{0}"") return {1}.{0};";
            StringBuilder builder = new StringBuilder();
            {
                FieldInfo[] fields = type.GetFields();
                foreach (var field in fields)
                {
                    if (field.IsStatic)
                    {
                        builder.AppendFormat(fieldStr, field.Name, type.FullName);
                    }
                    else {
                        builder.AppendFormat(fieldStr, field.Name, "((" + type.FullName + ")obj)");
                    }
                    builder.AppendLine();
                }
            }
            {
                PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                foreach (var property in propertys)
                {
                    if (property.CanWrite)
                    {
                        pInfos.Add(property.GetSetMethod());
                    }
                    if (property.CanRead)
                    {
                        pInfos.Add(property.GetGetMethod());
                    }
                }
            }
            {
                PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in propertys)
                {
                    if (property.CanRead)
                    {
                        builder.AppendFormat(fieldStr, property.Name, "((" + type.FullName + ")obj)");
                        builder.AppendLine();
                    }
                }
            }
            {
                PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (var property in propertys)
                {
                    if (property.CanRead)
                    {
                        builder.AppendFormat(fieldStr, property.Name, type.FullName);
                        builder.AppendLine();
                    }
                }
            }
            {
                string methodStr = @"            if (name == ""{0}"") return {1}.GetInstance();";
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    if (pInfos.Contains(method)) { continue; }
                    builder.AppendFormat(methodStr, method.Name, name + "_" + method.Name);
                    builder.AppendLine();
                }
            }
            {
                string methodStr = @"            if (name == ""{0}"") return {1}.GetInstance();";
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var method in methods)
                {
                    if (pInfos.Contains(method)) { continue; }
                    builder.AppendFormat(methodStr, method.Name, name + "_" + method.Name);
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
        static string SetValue(string name, Type type)
        {
            string fieldStr = @"            if (name == ""{0}"") {{ {1}.{0} = ({2})value; return; }}";
            StringBuilder builder = new StringBuilder();
            FieldInfo[] fields = type.GetFields();
            foreach (var field in fields) {
                if (field.IsStatic) {
                    builder.AppendFormat(fieldStr, field.Name, type.FullName, field.FieldType.FullName);
                } else {
                    builder.AppendFormat(fieldStr, field.Name, "((" + type.FullName + ")obj)", field.FieldType.FullName);
                }
                builder.AppendLine();
            }
            PropertyInfo[] propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in propertys) {
                if (property.CanWrite) {
                    builder.AppendFormat(fieldStr, property.Name, "((" + type.FullName + ")obj)", property.PropertyType.FullName);
                    builder.AppendLine();
                }
            }
            propertys = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (var property in propertys) {
                if (property.CanWrite) {
                    builder.AppendFormat(fieldStr, property.Name, type.FullName, property.PropertyType.FullName);
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
        static string GetMethods(string name, Type type)
        {
            StringBuilder builder = new StringBuilder();
            List<string> ms = new List<string>();
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                if (pInfos.Contains(method)) { continue; }
                if (ms.Contains(method.Name)) {
                    continue;
                }
                ms.Add(method.Name);
                builder.Append(GetMethod(name, method.Name, methods, type));
            }
            return builder.ToString();
        }
        static string GetMethod(string name, string methodName, MethodInfo[] methods, Type type)
        {
            string str = @"
    public class __name {
        private static __name _instance;
        public static __name GetInstance() { if (_instance == null) _instance = new __name(); return _instance; }
        public object Call(object obj, string type, object[] args) {
__execute
            return null;
        }
    }";
            str = str.Replace("__execute", GetMethodExecute(name, methodName, methods, type));
            str = str.Replace("__name", name + "_" + methodName);
            return str;
        }
        static string GetMethodExecute(string name, string methodName, MethodInfo[] methods, Type type)
        {
            StringBuilder builder = new StringBuilder();
            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName)
                {
                    string w = "";
                    var infos = method.GetParameters();
                    for (int i=0;i<infos.Length;++i)
                    {
                        w += (infos[i].ParameterType.FullName + "+");
                    }
                    //builder.AppendFormat("if (type == ""{0}"") ")
                    string call = method.IsStatic ? type.FullName : "((" + type.FullName + ")obj)";
                    call += "." + method.Name + "(";
                    for (int i = 0; i < infos.Length; ++i) {
                        if (i != 0) { call += ","; }
                        call += "(" + infos[i].ParameterType.FullName + ")args[" + i + "]"; 
                    }
                    call += ")";
                    if (method.ReturnType == typeof(void)) {
                        builder.AppendFormat("            if (type == \"{0}\") {{ {1}; return null; }}", w, call);
                    } else {
                        builder.AppendFormat("            if (type == \"{0}\") return {1};", w, call);
                    }
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }
    }
}
