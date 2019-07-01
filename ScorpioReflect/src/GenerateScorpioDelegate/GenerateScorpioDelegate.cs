using System;
using System.Text;
using System.Collections.Generic;
namespace Scorpio.ScorpioReflect {
    public partial class GenerateScorpioDelegate {
        private static readonly Type TYPE_DELEGATE = typeof(Delegate);
        private const string Template = @"using System;
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
            if (!m_Delegates.Contains(type))
                m_Delegates.Add(type);
        }
        public string Generate() {
            ScorpioReflectUtil.SortType(m_Delegates);
            return Template.Replace("__CreateDelegate", CreateDelegate());
        }
        public string CreateDelegate() {
            var builder = new StringBuilder();
            //bool first = true;
            foreach (var type in m_Delegates) {
                if (!TYPE_DELEGATE.IsAssignableFrom(type)) { continue; }
                var InvokeMethod = type.GetMethod("Invoke");
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


                var fullName = ScorpioReflectUtil.GetFullName(type);
                builder.Append($@"
        if (delegateType == typeof({fullName})) {{
            return new {fullName}( ({pars}) => {{ {call}; }} );
        }}");
                //if (first) { first = false; } else { builder.AppendLine(); }
                //builder.AppendLine("            if (type == typeof(__Name))".Replace("__Name", ScorpioReflectUtil.GetFullName(type)));
                //builder.Append("                return new __Name(".Replace("__Name", ScorpioReflectUtil.GetFullName(type)));
                
                //builder.Append("(" + pars + ") => { ");
                //var returnType = InvokeMethod.ReturnType;
                //if (returnType == typeof(void)) {
                //    builder.Append("func.call(" + pars + ");");
                //} else if (returnType == typeof(bool)) {
                //    builder.Append("return script.CreateObject(func.call(" + pars + ")).LogicOperation();");
                //} else if (returnType == typeof(string)) {
                //    builder.Append("return script.CreateObject(func.call(" + pars + ")).ToString();");
                //} else if (returnType == typeof(sbyte) || returnType == typeof(byte) ||
                //            returnType == typeof(short) || returnType == typeof(ushort) ||
                //            returnType == typeof(int) || returnType == typeof(uint) ||
                //            returnType == typeof(long) || returnType == typeof(ulong) ||
                //            returnType == typeof(float) || returnType == typeof(double) || returnType == typeof(decimal)) {
                //    string str = "return (__Type)Convert.ChangeType(script.CreateObject(func.call(" + pars + ")).ObjectValue, typeof(__Type));";
                //    str = str.Replace("__Type", ScorpioReflectUtil.GetFullName(returnType));
                //    builder.Append(str);
                //} else if (typeof(ScriptObject).IsAssignableFrom(returnType)) {
                //    builder.Append("return script.CreateObject(func.call(" + pars + "));");
                //} else {
                //    string str = "return (__Type)script.CreateObject(func.call(" + pars + ")).ObjectValue;";
                //    str = str.Replace("__Type", ScorpioReflectUtil.GetFullName(returnType));
                //    builder.Append(str);
                //}
                //builder.Append(" });");
            }
            return builder.ToString();
        }
    }
}
