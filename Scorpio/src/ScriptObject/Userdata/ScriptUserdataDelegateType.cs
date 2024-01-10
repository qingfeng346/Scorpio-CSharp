using System;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    /// <summary> 委托 Delegate Type </summary>
    public class ScriptUserdataDelegateType : ScriptUserdata {
        public ScriptUserdataDelegateType(Type value) {
            this.m_Value = value;
            this.m_ValueType = value;
        }
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override string ToString() { return m_ValueType.FullName; }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptUserdataDelegate(ScorpioDelegateFactoryManager.CreateDelegate(m_ValueType, parameters[0].scriptValue)));
        }
    }
}
