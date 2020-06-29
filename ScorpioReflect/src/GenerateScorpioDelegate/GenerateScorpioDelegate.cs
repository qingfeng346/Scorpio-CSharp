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
    public static void Initialize(Script script) {
        ScorpioDelegateFactory.SetFactory(new DelegateFactory());__DelegateList__CreateDelegate
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {
        if (delegates.TryGetValue(delegateType, out var func)) {
            return func(scriptObject);
        }
        throw new Exception(""Delegate Type is not found : "" + delegateType + ""  scriptObject : "" + scriptObject);
    }
}";
        private const string TemplateIf = @"using System;
using System.Collections.Generic;
using Scorpio;
public class DelegateFactory : IDelegateFactory {
    public static void Initialize(Script script) {
        ScorpioDelegateFactory.SetFactory(new DelegateFactory());__DelegateList
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
            m_Delegates.SortType();
            return (buildType == 0 ? Template : TemplateIf).Replace("__DelegateList", DelegateList()).Replace("__CreateDelegate", CreateDelegate(buildType));
        }
        string DelegateList() {
            var builder = new StringBuilder();
            foreach (var type in m_Delegates) {
                var fullName = ScorpioReflectUtil.GetFullName(type);
                builder.Append($@"
        script.SetGlobal(""{fullName}"", ScriptValue.CreateValue(typeof({fullName})));");
            }
            return builder.ToString();
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
                var call = ScorpioReflectUtil.ReturnString(invoke, InvokeMethod.ReturnType);
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
