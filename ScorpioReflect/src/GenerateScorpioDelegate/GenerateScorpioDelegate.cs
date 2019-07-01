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
        private List<Type> m_Delegates = new List<Type>();
        public void AddType(Type type) {
            if (type == null) { return; }
            if (!m_Delegates.Contains(type))
                m_Delegates.Add(type);
        }
        public string Generate() {
            ScorpioReflectUtil.SortType(m_Delegates);
            return Template.Replace("__CreateDelegate", CreateDelegate());
        }
        public string CreateDelegate() {
            var builder = new StringBuilder();
            foreach (var type in m_Delegates) {
                if (!TYPE_DELEGATE.IsAssignableFrom(type)) { continue; }
                // MulticastDelegate 是 event
                if (type == typeof(MulticastDelegate)) { continue; }
                var fullName = ScorpioReflectUtil.GetFullName(type);
                if (string.IsNullOrWhiteSpace(fullName)) { continue; }
                var InvokeMethod = type.GetMethod("Invoke");
                if (InvokeMethod == null) { throw new System.Exception("找不到Invoke函数 : " + type.FullName); }
                var parameters = InvokeMethod.GetParameters();
                var pars = "";
                for (int i = 0; i < parameters.Length; ++i) {
                    if (i != 0) { pars += ","; }
                    pars += $"arg{i}";
                }
                var invoke = parameters.Length == 0 ? $"scriptObject.call(ScriptValue.Null)" : $"scriptObject.call(ScriptValue.Null,{pars})";
                var call = "";
                var returnType = InvokeMethod.ReturnType;
                var returnFullName = ScorpioReflectUtil.GetFullName(returnType);
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
                
                builder.Append($@"
        delegates[typeof({fullName})] = (scriptObject) => {{ return new {fullName}( ({pars}) => {{ {call}; }} ); }};");
            }
            return builder.ToString();
        }
    }
}
