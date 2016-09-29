using System;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 普通Object Type类型 </summary>
    public class ScriptUserdataObjectType : ScriptUserdata
    {
        protected UserdataType m_UserdataType;
        public ScriptUserdataObjectType(Script script, Type value, UserdataType type) : base(script)
        {
            this.m_Value = value;
            this.m_ValueType = value;
            this.m_UserdataType = type;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return m_UserdataType.CreateInstance(parameters);
        }
        public override ScriptObject GetValue(object key)
        {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "ObjectType GetValue只支持String类型");
            return m_Script.CreateObject(m_UserdataType.GetValue(null, name));
        }
        public override void SetValue(object key, ScriptObject value)
        {
            string name = key as string;
            if (name == null) throw new ExecutionException(m_Script, "ObjectType SetValue只支持String类型");
            m_UserdataType.SetValue(null, name, value);
        }
    }
}
