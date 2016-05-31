using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 动态委托类型实例 </summary>
    public class DefaultScriptUserdataDelegate : ScriptUserdata {
        private class FunctionParameter {
            public Type ParameterType;
            public object DefaultValue;
            public FunctionParameter(Type type, object def) {
                this.ParameterType = type;
                this.DefaultValue = def;
            }
        }
        private Delegate m_Delegate;
        private List<FunctionParameter> m_Parameters = new List<FunctionParameter>();
        private object[] m_Objects;
        public DefaultScriptUserdataDelegate(Script script, Delegate value) : base(script) {
            this.m_Delegate = value;
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            var method = ScriptExtensions.GetMethodInfo(m_Delegate);
            var infos = method.GetParameters();
            var dynamicDelegate = method.Name.Equals(Script.DynamicDelegateName);
            int length = dynamicDelegate ? infos.Length - 1 : infos.Length;
            m_Objects = new object[length];
            for (int i = 0; i < length; ++i) {
                var p = infos[dynamicDelegate ? i + 1 : i];
                m_Parameters.Add(new FunctionParameter(p.ParameterType, p.DefaultValue));
            }
        }
        public override object Call(ScriptObject[] parameters) {
            FunctionParameter parameter;
            for (int i = 0; i < m_Parameters.Count; i++) {
                parameter = m_Parameters[i];
                if (i >= parameters.Length) {
                    m_Objects[i] = parameter.DefaultValue;
                } else {
                    m_Objects[i] = Util.ChangeType(m_Script, parameters[i], parameter.ParameterType);
                }
            }
            return m_Delegate.DynamicInvoke(m_Objects);
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            switch (type) {
                case TokenType.Plus:
                    return m_Script.CreateObject(Delegate.Combine(m_Delegate, (Delegate)Util.ChangeType(m_Script, obj, ValueType)));
                case TokenType.Minus:
                    return m_Script.CreateObject(Delegate.Remove(m_Delegate, (Delegate)Util.ChangeType(m_Script, obj, ValueType)));
                default:
                    throw new ExecutionException(m_Script, "Delegate 不支持的运算符 " + type);
            }
        }
        public override ScriptObject GetValue(object key) {
            if (!(key is string) || !key.Equals("Type")) throw new ExecutionException(m_Script, "EventInfo GetValue只支持 Type 一个变量");
            return m_Script.CreateObject(ValueType);
        }
    }
}
