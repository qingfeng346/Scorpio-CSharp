using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Scorpio.ScorpioReflect {
    public class GenerateScorpioInterface {
        //类模板
        public const string ClassTemplate = @"using System;
using Scorpio;
public class __class : __interface {
    public ScriptValue Value { get; set; }
    public ScriptValue __Call(string functionName, params object[] args) {
        var func = Value.GetValue(functionName);
        if (func.valueType == ScriptValue.scriptValueType) {
            try {
                return func.call(Value, args);
            } catch (System.Exception e) {
                throw new System.Exception($""ScriptInterce Call is error Type:__class  Function:{functionName} error:{e.ToString()}"");
            }
        }
        return ScriptValue.Null;
    }__method_content
}";

        private Type m_Type;
        public string FullName { get; }
        public string ScorpioClassName { get; }
        public List<MethodInfo> Methods { get; }
        public GenerateScorpioInterface(Type type) {
            m_Type = type;
            FullName = ScorpioReflectUtil.GetFullName(m_Type);
            ScorpioClassName = "ScorpioInterface_" + ScorpioReflectUtil.GetGenerateClassName(type);
            Methods = new List<MethodInfo>(type.GetMethods());
        }
        public string Generate() {
            return ClassTemplate.Replace("__class", ScorpioClassName)
                    .Replace("__interface", FullName)
                    .Replace("__method_content", GenerateMethods());
        }
        string GenerateMethods() {
            var builder = new StringBuilder();
            foreach (var method in Methods) {
                var parameters = method.GetParameters();
                var parameterStrs = "";
                var parameterArgs = "";
                for (int i = 0; i < parameters.Length; ++i) {
                    if (i != 0) { parameterStrs += ", ";  }
                    parameterStrs += $"{parameters[i].ParameterType.GetFullName()} {parameters[i].Name}";
                    parameterArgs += $", {parameters[i].Name}";
                }
                var call = ScorpioReflectUtil.ReturnString($@"__Call(""{method.Name}""{parameterArgs})", method.ReturnType);
                builder.Append($@"
    public {method.ReturnType.GetFullName()} {method.Name}({parameterStrs}) {{
        {call};
    }}");
            }
            return builder.ToString();
        }
    }
}
