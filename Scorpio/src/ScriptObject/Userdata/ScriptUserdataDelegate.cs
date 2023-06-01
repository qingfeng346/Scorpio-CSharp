using System;
using Scorpio.Tools;
namespace Scorpio.Userdata
{
    /// <summary> delegate object </summary>
    public class ScriptUserdataDelegate : ScriptUserdata {
        private class FunctionParameter {
            public Type ParameterType;
            public object DefaultValue;
            public FunctionParameter(Type type, object defaultValue) {
                this.ParameterType = type;
                this.DefaultValue = defaultValue;
            }
        }
        private Delegate m_Delegate;
        private FunctionParameter[] m_Parameters;
        private object[] m_Objects;
        public ScriptUserdataDelegate(Script script, Delegate value) : base(script) {
            this.m_Delegate = value;
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            var method = m_Delegate.Method;
            var parameters = method.GetParameters();
            var length = parameters.Length;
            m_Objects = new object[length];
            m_Parameters = new FunctionParameter[length];
            for (int i = 0; i < length; ++i) {
                var parameter = parameters[i];
                m_Parameters[i] = new FunctionParameter(parameter.ParameterType, parameter.DefaultValue);
            }
        }
        public override void Free() {
            throw new NotImplementedException();
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            for (int i = 0; i < m_Parameters.Length; i++) {
                var parameter = m_Parameters[i];
                if (i >= parameters.Length) {
                    m_Objects[i] = parameter.DefaultValue;
                } else {
                    m_Objects[i] = parameters[i].ChangeType(parameter.ParameterType);
                }
            }
            return ScriptValue.CreateValue(m_Delegate.DynamicInvoke(m_Objects));
        }
        public override ScriptValue Plus(ScriptValue value) {
            return ScriptValue.CreateValue(Delegate.Combine(m_Delegate, (Delegate)value.ChangeType(m_ValueType)));
        }
        public override ScriptValue Minus(ScriptValue value) {
            if (value.valueType == ScriptValue.scriptValueType && value.scriptValue is ScriptUserdataDelegate) {
                return ScriptValue.CreateValue(Delegate.Remove(m_Delegate, (value.scriptValue as ScriptUserdataDelegate).m_Delegate));
            }
            return base.Minus(value); 
        }
        public override ScriptValue GetValue(string key) {
            if (key == "Type") {
                return ScorpioTypeManager.GetUserdataType(m_ValueType);
            }
            return base.GetValue(key);
        }
        public override string ToString() { return m_Value.ToString(); }
    }
}
