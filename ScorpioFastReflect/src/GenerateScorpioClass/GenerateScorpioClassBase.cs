using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Scorpio.Tools;
namespace Scorpio.FastReflect {
    public partial class GenerateScorpioClass {
        //类模板
        public const string ClassTemplate = @"using System;
using System.Collections.Generic;
using System.Reflection;
using Scorpio;
using Scorpio.Userdata;
using Scorpio.Tools;
using Scorpio.Exception;__extensions_using
public class __class : IScorpioFastReflectClass {__reflect_content
    public UserdataMethodFastReflect GetConstructor() {
        return __class_Constructor.GetInstance();
    }
    public Type GetVariableType(string name) {
        switch (name) {__getvariabletype_content
            default: throw new ExecutionException(""__fullname [GetVariableType] 找不到变量 : "" + name);
        }
    }
    public UserdataMethod GetMethod(string name) {
        switch (name) {__method_content
            default: return null;
        }
    }
    public bool GetValue(object obj, string name, out object value) {
        switch (name) {__getvalue_content
            default: value = null; return false;
        }
    }
    public void SetValue(object obj, string name, ScriptValue value) {
        switch (name) {__setvalue_content
            default: throw new ExecutionException(""__fullname [SetValue] 找不到变量 : "" + name);
        }
    }
__constructor_content
__methods_content
}";
        //单个函数模板
        public const string MethodTemplate = @"
    public class __name : IScorpioFastReflectMethod {
        private static UserdataMethodFastReflect _instance = null;
        public static UserdataMethodFastReflect GetInstance() {
            if (_instance != null) { return _instance; }
            var methodInfos = new ScorpioFastReflectMethodInfo[] {__getallmethod
            };
            return _instance = new UserdataMethodFastReflect(typeof(__fullname), ""__methodname"", methodInfos, new __name()); 
        }
        public object Call(object obj, int methodIndex, object[] args) {
            switch (methodIndex) {__execute
                default: throw new ExecutionException(""__fullname 找不到合适的函数 : __methodname    type : "" + methodIndex);
            }
        }
    }";
        //根据MethodInfo生成一个ScorpioMethodInfo对象
        private string GetScorpioMethod(bool isStatic, ParameterInfo[] parameters, int index) {
            var paramType = "null";
            var parameterType = new StringBuilder("new Type[]{");
            var refOut = new StringBuilder("new bool[]{");
            bool first = true;
            foreach (var parameter in parameters) {
                if (first) { first = false; } else { parameterType.Append(","); refOut.Append(","); }
                if (parameter.IsRetvalOrOut()) {
                    parameterType.Append($"typeof({parameter.ParameterType.GetElementType().GetFullName()})");
                    refOut.Append("true");
                } else {
                    refOut.Append("false");
                    parameterType.Append($"typeof({parameter.ParameterType.GetFullName()})");
                    if (parameter.IsParams()) {
                        paramType = $"typeof({parameter.ParameterType.GetElementType().GetFullName()})";
                    }
                }
            }
            parameterType.Append("}");
            refOut.Append("}");
            return string.Format(@"new ScorpioFastReflectMethodInfo({0}, {1}, {2}, {3}, {4})", isStatic ? "true" : "false", parameterType.ToString(), refOut.ToString(), paramType, index);
        }
        private string GetExecuteMethod(ParameterInfo[] pars, bool hasReturn, string call) {
            var callBuilder = new StringBuilder();
            var hasRefOut = false;
            for (var j = 0; j < pars.Length; ++j) {
                if (pars[j].IsRetvalOrOut()) {
                    hasRefOut = true;
                    var typeName = ScorpioFastReflectUtil.GetFullName(pars[j].ParameterType.GetElementType());
                    callBuilder.Append($@"
                    var retval{j} = args[{j}] == null ? default({typeName}) : ({typeName})args[{j}]; ");
                }
            }
            if (hasRefOut) {
                if (hasReturn) {
                    callBuilder.Append($@"
                    var __Result = {call};");
                } else {
                    callBuilder.Append($@"
                    {call};");
                }
                for (var j = 0; j < pars.Length; ++j) {
                    if (pars[j].IsRetvalOrOut()) {
                        callBuilder.Append($@"
                    args[{j}] = retval{j};");
                    }
                }
                if (hasReturn) {
                    callBuilder.Append(@"
                    return __Result;
               ");
                } else {
                    callBuilder.Append(@"
                    return null;
               ");
                }
                return callBuilder.ToString();
            } else {
                if (hasReturn) {
                    callBuilder.Append($"return {call};");
                } else {
                    callBuilder.Append($"{call}; return null;");
                }
                return callBuilder.ToString();
            }
        }
        private string GetScorpioMethodCall(ParameterInfo[] pars) {
            var builder = new StringBuilder();
            for (int i = 0; i < pars.Length; ++i) {
                if (i != 0) { builder.Append(", "); }
                var par = pars[i];
                if (par.IsOut) {
                    builder.Append($"out retval{i}");
                } else if (par.ParameterType.IsByRef) {
                    builder.Append($"ref retval{i}");
                } else {
                    var typeName = ScorpioFastReflectUtil.GetFullName(par.ParameterType);
                    builder.Append($"({typeName})args[{i}]");
                }
            }
            return builder.ToString();
        }
        private string GetScorpioMethodArgs(ParameterInfo[] pars, int index) {
            return "(" + ScorpioFastReflectUtil.GetFullName(pars[index].ParameterType) + ")args[" + index + "]";
        }
        private string GetScorpioVariable(bool IsStatic, string name) {
            return (IsStatic ? FullName : $"(({FullName})obj)") + "." + name;
        }
        private string GetAllMethod(List<ScorpioMethod> methods) {
            var builder = new StringBuilder();
            for (var i = 0; i < methods.Count; ++i) {
                var method = methods[i];
                builder.Append($@"
                {GetScorpioMethod(method.IsStatic, method.parameterInfos, i)},");
            }
            return builder.ToString();
        }
    }
}
