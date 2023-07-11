using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.FastReflect {
    public partial class GenerateScorpioDelegate {
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private const string Template = @"using System;
using System.Collections.Generic;
using Scorpio;
public class __FactoryName : IScorpioDelegateFactory {
    private static Dictionary<Type, Func<ScriptObject, Delegate>> delegates = new Dictionary<Type, Func<ScriptObject, Delegate>>();
    public static void Initialize() {
        ScorpioDelegateFactoryManager.SetFactory(new DelegateFactory());__DelegateList__CreateDelegate
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
public class __FactoryName : IScorpioDelegateFactory {
    public static void Initialize() {
        ScorpioDelegateFactoryManager.SetFactory(new __FactoryName());__DelegateList
    }
    public Delegate CreateDelegate(Type delegateType, ScriptObject scriptObject) {__CreateDelegate
        throw new Exception(""Delegate Type is not found : "" + delegateType + ""  scriptObject : "" + scriptObject);
    }
}";
        public struct Option {
            public int buildType;           //build 类型
            public bool generateList;       //是否生成 DelegateList
            public string className;        //生成的仓库类名
        }
        private List<Type> m_Delegates = new List<Type>();
        public Option option { get; set; }
        public void AddType(Type type) {
            if (type == null || !TYPE_DELEGATE.IsAssignableFrom(type) ||
                // MulticastDelegate 是 event
                type == typeof(MulticastDelegate) || string.IsNullOrWhiteSpace(ScorpioFastReflectUtil.GetFullName(type)) ||
                m_Delegates.Contains(type)) { return; }
            m_Delegates.Add(type);
        }
        public string Generate() {
            m_Delegates.SortType();
            return (option.buildType == 0 ? TemplateIf : Template).Replace("__FactoryName", option.className).Replace("__DelegateList", DelegateList()).Replace("__CreateDelegate", CreateDelegate());
        }
        string DelegateList() {
            if (!option.generateList) { return ""; }
            var builder = new StringBuilder();
            foreach (var type in m_Delegates) {
                var fullName = ScorpioFastReflectUtil.GetFullName(type);
                builder.Append($@"
        script.SetGlobal(""{fullName}"", ScriptValue.CreateValue(typeof({fullName})));");
            }
            return builder.ToString();
        }
        string CreateDelegate() {
            var builder = new StringBuilder();
            foreach (var type in m_Delegates) {
                var fullName = ScorpioFastReflectUtil.GetFullName(type);
                var InvokeMethod = type.GetMethod("Invoke");
                var parameters = InvokeMethod.GetParameters();
                var pars = "";
                for (int i = 0; i < parameters.Length; ++i) {
                    if (i != 0) { pars += ","; }
                    pars += $"arg{i}";
                }
                var invoke = parameters.Length == 0 ? $"scriptObject.call(ScriptValue.Null)" : $"scriptObject.call(ScriptValue.Null,{pars})";
                var call = ScorpioFastReflectUtil.ReturnString(invoke, InvokeMethod.ReturnType);
                var func = $"return new {fullName}( ({pars}) => {{ {call}; }} );";
                if (option.buildType == 0) {
                    builder.Append($@"
        if (delegateType == typeof({fullName}))
            {func}");
                } else {
                    builder.Append($@"
        delegates[typeof({fullName})] = (scriptObject) => {{ {func} }};");
                }
            }
            return builder.ToString();
        }
    }
}
