using System;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    /// <summary> 普通Object Type类型 </summary>
    public class DefaultScriptUserdataObjectType : ScriptUserdata
    {
        protected UserdataType m_UserdataType;
        public DefaultScriptUserdataObjectType(Script script, Type value, UserdataType type) : base(script)
        {
            this.Value = value;
            this.ValueType = value;
            this.m_UserdataType = type;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return m_UserdataType.CreateInstance(parameters);
        }
        public override ScriptObject GetValue(object key)
        {
            if (!(key is string)) throw new ExecutionException(Script, "ObjectType GetValue只支持String类型");
            return Script.CreateObject(m_UserdataType.GetValue(null, (string)key));
        }
        public override void SetValue(object key, ScriptObject value)
        {
            if (!(key is string)) throw new ExecutionException(Script, "ObjectType SetValue只支持String类型");
            m_UserdataType.SetValue(null, (string)key, value);
        }
    }
}
