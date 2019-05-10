using System;
using System.Collections.Generic;
using Scorpio.Function;
using Scorpio.Exception;
using Scorpio.Commons;
namespace Scorpio.Userdata {
    /// <summary> 委托 Delegate Type </summary>
    public class ScriptUserdataDelegateType : ScriptUserdata {
        public ScriptUserdataDelegateType(Script script, Type value) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
        }
        public override Type ValueType { get { return Util.TYPE_TYPE; } }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return m_Script.CreateObject(ScorpioDelegateFactory.CreateDelegate(m_Script, m_ValueType, parameters[0].scriptValue));
        }
        public override string ToString() { return m_ValueType.Name; }
    }
}
