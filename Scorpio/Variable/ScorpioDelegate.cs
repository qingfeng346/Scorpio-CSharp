using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Variable
{
    public class ScorpioDelegate
    {
        private class FunctionParameter
        {
            public Type ParameterType;
            public object DefaultValue;
            public FunctionParameter(Type type, object def) {
                this.ParameterType = type;
                this.DefaultValue = def;
            }
        }
        private Delegate m_Delegate;
        private int m_ParameterCount = 0;
        private List<FunctionParameter> m_Parameters = new List<FunctionParameter>();
        private object[] m_Objects;
        public ScorpioDelegate(Delegate Dele)
        {
            m_Delegate = Dele;
            var infos = Dele.Method.GetParameters();
            int length = infos.Length;
            m_Objects = new object[length];
            for (int i = 0; i < length;++i ) {
                var p = infos[i];
                m_Parameters.Add(new FunctionParameter(p.ParameterType, p.DefaultValue));
            }
            m_ParameterCount = m_Parameters.Count;
        }
        public object Call(ScriptObject[] parameters)
        {
            FunctionParameter parameter;
            ScriptObject par;
            for (int i = 0; i < m_ParameterCount; i++) {
                parameter = m_Parameters[i];
                if (i >= parameters.Length) {
                    m_Objects[i] = parameter.DefaultValue;
                }  else {
                    par = parameters[i];
                    if (par is IScriptPrimitiveObject) {
                        m_Objects[i] = Convert.ChangeType(((IScriptPrimitiveObject)par).ObjectValue, parameter.ParameterType);
                    } else if (par is ScriptUserdata) {
                        m_Objects[i] = ((ScriptUserdata)par).Value;
                    } else {
                        m_Objects[i] = par;
                    }
                }
            }
            return m_Delegate.DynamicInvoke(m_Objects);
        }
    }
}
