using System;
using System.Collections.Generic;
using Scorpio;
using Scorpio.Exception;
using Scorpio.Variable;
namespace Scorpio.Userdata {
    /// <summary> 普通Object Type类型 </summary>
    public class ScriptUserdataObjectType : ScriptUserdata {
        protected UserdataType m_UserdataType;
        protected Dictionary<String, ScriptObject> m_Methods = new Dictionary<String, ScriptObject>();
        public ScriptUserdataObjectType(Script script, Type value, UserdataType type) : base(script) {
            this.m_Value = value;
            this.m_ValueType = value;
            this.m_UserdataType = type;
        }
        public override object Call(ScriptObject[] parameters) {
            return m_UserdataType.CreateInstance(parameters);
        }
        public override ScriptObject GetValue(object key) {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "ObjectType GetValue只支持String类型");
            if (m_Methods.ContainsKey(name)) return m_Methods[name];
            var ret = m_UserdataType.GetValue(null, name);
            if (ret is UserdataMethod) {
                UserdataMethod method = (UserdataMethod)ret;
                ScriptObject value =  m_Script.CreateObject(method.IsStatic ? (ScorpioMethod)new ScorpioStaticMethod(name, method) : (ScorpioMethod)new ScorpioTypeMethod(m_Script, name, method, m_ValueType));
                m_Methods.Add(name, value);
                return value;
            }
            return m_Script.CreateObject(ret);
        }
        public override void SetValue(object key, ScriptObject value) {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "ObjectType SetValue只支持String类型");
            m_UserdataType.SetValue(null, name, value);
        }
    }
}
