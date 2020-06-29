using System;
using Scorpio.Tools;
namespace Scorpio.Userdata {
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
        public ScriptUserdataDelegate(Delegate value) {
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
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            for (int i = 0; i < m_Parameters.Length; i++) {
                var parameter = m_Parameters[i];
                if (i >= parameters.Length) {
                    m_Objects[i] = parameter.DefaultValue;
                } else {
                    m_Objects[i] = Util.ChangeType(parameters[i], parameter.ParameterType);
                }
            }
            return ScriptValue.CreateValue(m_Delegate.DynamicInvoke(m_Objects));
        }
        public override ScriptValue Plus(ScriptValue obj) {
            return ScriptValue.CreateValue(Delegate.Combine(m_Delegate, (Delegate)Util.ChangeType(obj, m_ValueType)));
        }
        public override ScriptValue Minus(ScriptValue obj) {
            if (obj.valueType == ScriptValue.scriptValueType && obj.scriptValue is ScriptUserdataDelegate) {
                return ScriptValue.CreateValue(Delegate.Remove(m_Delegate, (obj.scriptValue as ScriptUserdataDelegate).m_Delegate));
            }
            return base.Minus(obj); 
        }
        public override ScriptValue GetValue(string key) {
            if (key == "Type") {
                return TypeManager.GetUserdataType(m_ValueType);
            }
            return base.GetValue(key);
        }
        public override string ToString() { return m_Value.ToString(); }
    }
}
