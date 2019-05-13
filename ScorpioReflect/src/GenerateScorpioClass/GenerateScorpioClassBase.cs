using System;
using System.Reflection;
using System.Text;
using Scorpio.Commons;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioClass {
        //类模板
        public const string ClassTemplate = @"using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
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
        //单个函数模板
        public const string MethodTemplate = @"
    public class __name : ScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _method;
        public static UserdataMethodFastReflect GetMethod(Script script) {
            if (_method == null) {
                var methods = new List<ScorpioFastReflectMethodInfo>();__getallmethod
                _method = new UserdataMethodFastReflect(script, typeof(__fullname), ""__methodname"", methods.ToArray(), new __name()); 
            }
            return _method;
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {__execute
            default: throw new Exception(""__fullname 找不到合适的函数 : __methodname    type : "" + methodIndex);
            }
        }
    }";
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
            var builder = new StringBuilder();
            for (var i = 0; i < methods.Length; ++i) {
                builder.Append(@"
                methods.Add(" + GetScorpioMethod(methods[i], i) + ");");
            }
            return builder.ToString();
        }
    }
}
