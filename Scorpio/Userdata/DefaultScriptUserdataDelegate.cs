using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Userdata
{
    /// <summary> 动态委托类型实例 </summary>
    public class DefaultScriptUserdataDelegate : ScriptUserdata
    {
        private class FunctionParameter
        {
            public Type ParameterType;
            public object DefaultValue;
            public FunctionParameter(Type type, object def)
            {
                this.ParameterType = type;
                this.DefaultValue = def;
            }
        }
        private Delegate m_Delegate;
        private int m_ParameterCount = 0;
        private List<FunctionParameter> m_Parameters = new List<FunctionParameter>();
        private object[] m_Objects;
        public DefaultScriptUserdataDelegate(Script script, Delegate value) : base(script)
        {
            this.m_Delegate = value;
            this.Value = value;
            this.ValueType = value.GetType();
            var infos = value.Method.GetParameters();
            var dynamicDelegate = value.Method.Name.Equals(Script.DynamicDelegateName);
            int length = dynamicDelegate ? infos.Length - 1 : infos.Length;
            m_Objects = new object[length];
            for (int i = 0; i < length; ++i)
            {
                var p = infos[dynamicDelegate ? i + 1 : i];
                m_Parameters.Add(new FunctionParameter(p.ParameterType, p.DefaultValue));
            }
            m_ParameterCount = m_Parameters.Count;
        }
        public override ScriptObject Call(ScriptObject[] parameters)
        {
            FunctionParameter parameter;
            for (int i = 0; i < m_ParameterCount; i++)
            {
                parameter = m_Parameters[i];
                if (i >= parameters.Length) {
                    m_Objects[i] = parameter.DefaultValue;
                } else {
                    m_Objects[i] = Util.ChangeType(parameters[i], parameter.ParameterType);
                }
            }
            return Script.CreateObject(m_Delegate.DynamicInvoke(m_Objects));
        }
    }
}
