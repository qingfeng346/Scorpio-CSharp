using System;
using System.Reflection;
using System.Text;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        public const string ClassTemplate = @"using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Variable;
public class __class : IScorpioFastReflectClass {
    private Script m_Script;
    public __class(Script script) {
        m_Script = script;
    }
    public FastReflectUserdataMethod GetConstructor() {
        return __class_Constructor.GetMethod(m_Script);
    }
    public object GetValue(object obj, string name) {
__getvalue_content
        throw new Exception(""__fullname 找不到变量 : "" + name);
    }
    public void SetValue(object obj, string name, ScriptObject value) {
__setvalue_content
        throw new Exception(""__fullname 找不到变量 : "" + name);
    }
__constructor_content
__methods_content
}";
        public const string MethodTemplate = @"
    public class __name : IScorpioFastReflectMethod {
        private static ScorpioMethodInfo[] _methods;
        private static FastReflectUserdataMethod _method;
        private static ScorpioMethod _instance;
        static __name() {
            List<ScorpioMethodInfo> methods = new List<ScorpioMethodInfo>();
__getallmethod
            _methods = methods.ToArray();
        }
        public static FastReflectUserdataMethod GetMethod(Script script) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(__methodstatic, script, typeof(__fullname), ""__methodname"", _methods, new __name()); 
            }
            return _method;
        }
        public static ScorpioMethod GetInstance(Script script, object obj) {
            if (_method == null) {
                _method = new FastReflectUserdataMethod(__methodstatic, script, typeof(__fullname), ""__methodname"", _methods, new __name()); 
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
        private string GetScorpioMethod(MethodBase method) {
            if (!method.IsGenericMethod || !method.ContainsGenericParameters) {
                string name = method.Name;
                string isStatic = method.IsStatic ? "true" : "false";
                string parameterType = "new Type[] {";
                string param = "false";
                string paramType = "null";
                var pars = method.GetParameters();
                bool first = true;
                foreach (var par in pars) {
                    if (first) { first = false; } else { parameterType += ","; }
                    parameterType += "typeof(" + ScorpioReflectUtil.GetFullName(par.ParameterType) + ")";
                    if (Util.IsParamArray(par)) {
                        param = "true";
                        paramType = "typeof(" + ScorpioReflectUtil.GetFullName(par.ParameterType.GetElementType()) + ")";
                    }
                }
                parameterType += "}";
                return string.Format(@"new ScorpioMethodInfo(""{0}"", {1}, {2}, {3}, {4}, ""{5}"")", name, isStatic, parameterType, param, paramType, GetScorpioMethodParameterTypes(method));
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
            bool first = true;
            foreach (var method in methods) {
                if (first) { first = false; } else { builder.AppendLine(); }
                builder.Append("            methods.Add(" + GetScorpioMethod(method) + ");");
            }
            return builder.ToString();
        }
    }
}
