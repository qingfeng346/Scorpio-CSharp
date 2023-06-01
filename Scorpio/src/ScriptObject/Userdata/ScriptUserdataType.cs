using System;
using System.Collections.Generic;
using Scorpio.Function;
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
        public override Type ValueType => ScorpioUtil.TYPE_TYPE;
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            using var ret = new ScriptValue(m_UserdataType.CreateInstance(parameters, length));
            return ret;
        }
        public override ScriptValue GetValue(string key) {
            if (m_Methods.TryGetValue(key, out var value)) {
                return value;
            }
            var ret = m_UserdataType.GetValue(null, key);
            if (ret is UserdataMethod) {
                key = string.Intern(key);
                return m_Methods[key] = new ScriptValue(new ScriptStaticMethodFunction((UserdataMethod)ret, key));
            }
            using var v = ScriptValue.CreateValue(ret);
            return v;
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(null, key, value);
        }
        public override string ToString() { return m_ValueType.Name; }
    }
}
