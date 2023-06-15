using System;
using Scorpio.Tools;
namespace Scorpio.Userdata
{
    /// <summary> delegate object </summary>
    public class ScriptUserdataDelegate : ScriptUserdata {
        //private class FunctionParameter {
        //    public Type ParameterType;
        //    public object DefaultValue;
        //    public FunctionParameter(Type type, object defaultValue) {
        //        this.ParameterType = type;
        //        this.DefaultValue = defaultValue;
        //    }
        //}
        //private Delegate m_Delegate;
        //private FunctionParameter[] m_Parameters;
        //private object[] m_Objects;
        public ScriptUserdataDelegate(Script script) : base(script) { }
        public ScriptUserdataDelegate Set(Delegate value) {
            //this.m_Delegate = value;
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            //var method = m_Delegate.Method;
            //var parameters = method.GetParameters();
            //var length = parameters.Length;
            //m_Objects = new object[length];
            //m_Parameters = new FunctionParameter[length];
            //for (int i = 0; i < length; ++i) {
            //    var parameter = parameters[i];
            //    m_Parameters[i] = new FunctionParameter(parameter.ParameterType, parameter.DefaultValue);
            //}
            return this;
        }
        public override void Free() {
            m_Value = null;
            m_ValueType = null;
            m_Script.Free(this);
        }
        public override void gc() { }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return ScriptValue.Null;
            //for (int i = 0; i < m_Parameters.Length; i++) {
            //    var parameter = m_Parameters[i];
            //    if (i >= parameters.Length) {
            //        m_Objects[i] = parameter.DefaultValue;
            //    } else {
            //        m_Objects[i] = parameters[i].ChangeType(parameter.ParameterType);
            //    }
            //}
            //return ScriptValue.CreateValue(m_Script, m_Delegate.DynamicInvoke(m_Objects));
        }
        public override ScriptValue Plus(ScriptValue value) {
            return ScriptValue.CreateValue(m_Script, Delegate.Combine((Delegate)m_Value, (Delegate)value.ChangeType(m_ValueType)));
        }
        public override ScriptValue Minus(ScriptValue value) {
            if (value.valueType == ScriptValue.scriptValueType && value.scriptValue is ScriptUserdataDelegate) {
                return ScriptValue.CreateValue(m_Script, Delegate.Remove((Delegate)m_Value, (Delegate)(value.scriptValue as ScriptUserdataDelegate).Value));
            }
            return base.Minus(value);
        }
        public override string ToString() { return m_Value.ToString(); }
    }
}
