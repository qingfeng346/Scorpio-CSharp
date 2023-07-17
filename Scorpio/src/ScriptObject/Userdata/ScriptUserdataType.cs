using System;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    /// <summary> 普通Object Type类型 </summary>
    public class ScriptUserdataType : ScriptUserdata {
        protected UserdataType m_UserdataType;
        public ScriptUserdataType(Script script, Type value, UserdataType type) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
            this.m_UserdataType = type;
        }
        public override void Free() {
            m_Value = null;
            m_ValueType = null;
            m_UserdataType = null;
        }
        public override void gc() {
        }
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(m_UserdataType.CreateInstance(m_Script, parameters, length));
        }
        public override ScriptValue GetValue(string key) {
            return ScriptValue.CreateValueNoReference(m_Script, m_UserdataType.GetStaticValue(m_Script, key));
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(null, key, value);
        }
        public override string ToString() { return m_ValueType.FullName; }
    }
}
