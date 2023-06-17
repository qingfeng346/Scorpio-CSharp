using System;
using System.Collections.Generic;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    /// <summary> 普通Object Type类型 </summary>
    public class ScriptUserdataType : ScriptUserdata {
        protected UserdataType m_UserdataType;
        protected Dictionary<string, ScriptValue> m_Methods = new Dictionary<string, ScriptValue>();            //所有函数
        public ScriptUserdataType(Script script, Type value, UserdataType type) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
            this.m_UserdataType = type;
        }
        public override void Free() {
            m_Value = null;
            m_ValueType = null;
            m_UserdataType = null;
            m_Methods.Free();
        }
        public override void gc() {
            m_Methods.Free();
        }
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            return new ScriptValue(m_UserdataType.CreateInstance(m_Script, parameters, length));
        }
        public override ScriptValue GetValue(string key) {
            if (m_Methods.TryGetValue(key, out var value)) {
                return value;
            }
            var ret = m_UserdataType.GetValue(m_Script, null, key);
            if (ret is UserdataMethod) {
                key = string.Intern(key);
                return m_Methods[key] = new ScriptValue(m_Script.NewStaticMethod().Set(key, (UserdataMethod)ret));
            }
            return ScriptValue.CreateValueNoReference(script, ret);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(null, key, value);
        }
        public override string ToString() { return m_ValueType.FullName; }
    }
}
