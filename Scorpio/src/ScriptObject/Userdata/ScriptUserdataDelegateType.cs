using System;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    /// <summary> 委托 Delegate Type </summary>
    public class ScriptUserdataDelegateType : ScriptUserdata {
        public ScriptUserdataDelegateType(Type value) {
            this.m_Value = value;
            this.m_ValueType = value;
        }
        public override Type ValueType { get { return Util.TYPE_TYPE; } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(new ScriptUserdataDelegate(ScorpioDelegateFactory.CreateDelegate(m_ValueType, parameters[0].scriptValue)));
        }
        public override string ToString() { return m_ValueType.Name; }
    }
}
