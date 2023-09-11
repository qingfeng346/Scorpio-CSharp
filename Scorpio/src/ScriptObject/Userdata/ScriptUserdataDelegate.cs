using System;
using Scorpio.Exception;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    /// <summary> delegate object </summary>
    public class ScriptUserdataDelegate : ScriptUserdata {
        private Delegate m_Delegate;
        private Type[] m_Types;
        private object[] m_Objects;
        public ScriptUserdataDelegate(Delegate value) : base() {
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            this.m_Delegate = value;
            var method = m_Delegate.Method;
            var parameters = method.GetParameters();
            var length = parameters.Length;
            if (length > 0) {
                m_Objects = new object[length];
                m_Types = new Type[length];
                for (int i = 0; i < length; ++i) {
                    m_Types[i] = parameters[i].ParameterType;
                }
            } else {
                m_Objects = ScorpioUtil.OBJECT_EMPTY;
                m_Types = ScorpioUtil.TYPE_EMPTY;
            }
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            try {
                if (length != m_Types.Length)
                    throw new ExecutionException("参数数量错误");
                for (int i = 0; i < length; i++) {
                    m_Objects[i] = parameters[i].ChangeType(m_Types[i]);
                }
                return ScriptValue.CreateValue(m_Delegate.DynamicInvoke(m_Objects));
            } finally {
                Array.Clear(m_Objects, 0, m_Objects.Length);
            }
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
    }
}
