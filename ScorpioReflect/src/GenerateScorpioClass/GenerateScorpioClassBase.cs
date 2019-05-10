using System;
using System.Reflection;
using System.Text;
using Scorpio.Commons;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        public const string ClassTemplate = @"using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Variable;
using Scorpio.Commons;
public class __class : ScorpioFastReflectClass {
    private Script m_Script;
    public __class(Script script) {
        m_Script = script;
    }
    public UserdataMethodFastReflect GetConstructor() {
        return __class_Constructor.GetMethod(m_Script);
    }
    public Type GetVariableType(string name) {
__getvariabletype_content
        throw new Exception(""__fullname [GetVariableType] 找不到变量 : "" + name);
    }
    public object GetValue(object obj, string name) {
__getvalue_content
        throw new Exception(""__fullname [GetValue] 找不到变量 : "" + name);
    }
    public void SetValue(object obj, string name, ScriptValue value) {
__setvalue_content
        throw new Exception(""__fullname [SetValue] 找不到变量 : "" + name);
    }
__constructor_content
__methods_content
}";
        public const string MethodTemplate = @"
    public class __name : ScorpioFastReflectMethod {
        private static ScorpioFastReflectMethodInfo[] _methods;
        private static UserdataMethodFastReflect _method;
        private static ScorpioMethod _instance;
        static __name() {
            var methods = new List<ScorpioMethodInfo>();
__getallmethod
            _methods = methods.ToArray();
        }
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                _method = new UserdataMethodFastReflect(script, typeof(__fullname), ""__methodname"", _methods, new __name()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            GetMethod(script);__getmethod
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {__execute
            default: throw new Exception(""__fullname 找不到合适的函数 : __methodname    type : "" + type);
            }
        }
    }";
        public const string StaticMethodTemplate = @"
            if (_instance == null) {
                _instance = new ScorpioStaticMethod(""__methodname"", _method);
            }
            return _instance;";
        public const string InstanceMethodTemplate = @"
            if (obj == null) {
                if (_instance == null) {
                    _instance = new ScorpioTypeMethod(script, ""__methodname"", _method, typeof(__fullname));
                }
                return _instance;
            }
            return new ScorpioObjectMethod(obj, ""__methodname"", _method);";
        //获得最后生成的类的名字 把+和.都换成_
        public string GetClassName(Type type) {
            var fullName = type.FullName;
            if (type.IsGenericType) {
                var index = fullName.IndexOf("`");
                fullName = fullName.Substring(0, index);
                fullName += "_";
                var types = type.GetGenericArguments();
                bool first = true;
                foreach (var t in types) {
                    if (first == false) { fullName += "_"; } else { first = false; }
                    fullName += GetClassName(t);
                }
            }
            fullName = fullName.Replace("+", "_");
            fullName = fullName.Replace(".", "_");
            return fullName;
        }
        //根据MethodInfo生成一个ScorpioMethodInfo对象
        private string GetScorpioMethod(MethodBase method, int index) {
            if (!method.IsGenericMethod || !method.ContainsGenericParameters) {
                string name = method.Name;
                string isStatic = method.IsStatic ? "true" : "false";
                string parameterType = "new Type[] {";
                string paramType = "null";
                var pars = method.GetParameters();
                bool first = true;
                foreach (var par in pars) {
                    if (first) { first = false; } else { parameterType += ","; }
                    parameterType += "typeof(" + ScorpioReflectUtil.GetFullName(par.ParameterType) + ")";
                    if (Util.IsParams(par)) {
                        paramType = "typeof(" + ScorpioReflectUtil.GetFullName(par.ParameterType.GetElementType()) + ")";
                    }
                }
                parameterType += "}";
                return string.Format(@"new ScorpioFastReflectMethodInfo({0}, {1}, {2}, {3})", isStatic, parameterType, paramType, index);
            }
            return "";
        }
        private string GetScorpioMethodParameterTypes(MethodBase method) {
            string parameterTypes = "";
            var pars = method.GetParameters();
            foreach (var par in pars) {
                parameterTypes += ScorpioReflectUtil.GetFullName(par.ParameterType) + "+";
            }
            return parameterTypes;
        }
        private string GetScorpioMethodCall(MethodBase method) {
            var call = "";
            var pars = method.GetParameters();
            for (int i = 0; i < pars.Length; ++i) {
                if (i != 0) { call += ","; }
                call += "(" + ScorpioReflectUtil.GetFullName(pars[i].ParameterType) + ")args[" + i + "]";
            }
            call += "";
            return call;
        }
        private string GetScorpioMethodArgs(ParameterInfo[] pars, int index) {
            return "(" + ScorpioReflectUtil.GetFullName(pars[index].ParameterType) + ")args[" + index + "]";
        }
        private string GetScorpioVariable(bool IsStatic, string name) {
            return (IsStatic ? m_FullName : "((" + m_FullName + ")obj)") + "." + name;
        }
        private string GetAllMethod(MethodBase[] methods) {
            StringBuilder builder = new StringBuilder();
            for (var i = 0; i < methods.Length; ++i) {
                if (i != 0) { builder.AppendLine(); }
                builder.Append("            methods.Add(" + GetScorpioMethod(methods[i], i) + ");");
            }
            return builder.ToString();
        }
    }
}
