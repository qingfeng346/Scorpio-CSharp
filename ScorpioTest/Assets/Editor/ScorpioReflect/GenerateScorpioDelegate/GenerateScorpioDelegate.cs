using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioDelegate {
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private const string Template = @"using System;
using System.Collections.Generic;
using Scorpio;
public class DelegateFactory : IDelegateFactory {
    private static Dictionary<Type, Func<ScriptObject, Delegate>> delegates = new Dictionary<Type, Func<ScriptObject, Delegate>>();
    public static void Initialize() {
        ScorpioDelegateFactory.SetFactory(new DelegateFactory());__CreateDelegate
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {
        Func<ScriptObject, Delegate> func;
        if (delegates.TryGetValue(delegateType, out func)) {
            return func(scriptObject);
        }
        throw new Exception(""Delegate Type is not found : "" + delegateType + ""  scriptObject : "" + scriptObject);
    }
}";
        private const string TemplateIf = @"using System;
using System.Collections.Generic;
using Scorpio;
public class DelegateFactory : IDelegateFactory {
    public static void Initialize() {
        ScorpioDelegateFactory.SetFactory(new DelegateFactory());
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {__CreateDelegate
        throw new Exception(""Delegate Type is not found : "" + delegateType + ""  scriptObject : "" + scriptObject);
    }
}";
        private List<Type> m_Delegates = new List<Type>();
        public void AddType(Type type) {
            if (type == null || !TYPE_DELEGATE.IsAssignableFrom(type) ||
                // MulticastDelegate 是 event
                type == typeof(MulticastDelegate) || string.IsNullOrWhiteSpace(ScorpioReflectUtil.GetFullName(type)) ||
                m_Delegates.Contains(type)) { return; }
            m_Delegates.Add(type);
        }
        public string Generate(int buildType) {
            ScorpioReflectUtil.SortType(m_Delegates);
            return (buildType == 0 ? Template : TemplateIf).Replace("__CreateDelegate", CreateDelegate(buildType));
        }
        string CreateDelegate(int buildType) {
            var builder = new StringBuilder();
            foreach (var type in m_Delegates) {
                var fullName = ScorpioReflectUtil.GetFullName(type);
                var InvokeMethod = type.GetMethod("Invoke");
                var parameters = InvokeMethod.GetParameters();
                var pars = "";
                for (int i = 0; i < parameters.Length; ++i) {
                    if (i != 0) { pars += ","; }
                    pars += $"arg{i}";
                }
                var invoke = parameters.Length == 0 ? $"scriptObject.call(ScriptValue.Null)" : $"scriptObject.call(ScriptValue.Null,{pars})";
                var returnType = InvokeMethod.ReturnType;
                var returnFullName = ScorpioReflectUtil.GetFullName(returnType);
                var call = "";
                if (returnType == typeof(void)) {
                    call = $"{invoke}";
                } else if (returnType == typeof(ScriptValue)) {
                    call = $"return {invoke}";
                } else if (returnType == typeof(bool)) {
                    call = $"return {invoke}.IsTrue";
                } else if (returnType == typeof(string)) {
                    call = $"return {invoke}.ToString()";
                } else if (returnType == typeof(sbyte) || returnType == typeof(byte) ||
                            returnType == typeof(short) || returnType == typeof(ushort) ||
                            returnType == typeof(int) || returnType == typeof(uint) ||
                            returnType == typeof(long) || returnType == typeof(ulong) ||
                            returnType == typeof(float) || returnType == typeof(double) || returnType == typeof(decimal)) {
                    call = $"return ({returnFullName})Convert.ChangeType({invoke}.Value, typeof({returnFullName}))";
                } else {
                    call = $"return ({returnFullName}){invoke}.Value";
                }
                var func = $"return new {fullName}( ({pars}) => {{ {call}; }} );";
                if (buildType == 0) {
                    builder.Append($@"
        delegates[typeof({fullName})] = (scriptObject) => {{ {func} }};");
                } else {
                    builder.Append($@"
        if (delegateType == typeof({fullName}))
            {func}");
                }
            }
            return builder.ToString();
        }
    }
}
