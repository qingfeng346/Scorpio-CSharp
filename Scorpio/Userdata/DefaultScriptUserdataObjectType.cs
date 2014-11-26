using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;
namespace Scorpio.Userdata
{
    /// <summary> 普通Object Type类型 </summary>
    public class DefaultScriptUserdataObjectType : ScriptUserdata
    {
        private UserdataType m_Type;
        public DefaultScriptUserdataObjectType(Script script, Type value, UserdataType type) : base(script)
        {
            this.Value = value;
            this.ValueType = value;
            this.m_Type = type;
        }
        public override object Call(ScriptObject[] parameters)
        {
            return m_Type.CreateInstance(parameters);
        }
        public override ScriptObject GetValue(string strName)
        {
            object ret = m_Type.GetValue(Value, strName);
            if (ret is UserdataMethod) {
                UserdataMethod method = ret as UserdataMethod;
                if (method.IsStatic)
                    return Script.CreateFunction(new ScorpioStaticMethod(strName, method));
                else
                    return Script.CreateFunction(new ScorpioTypeMethod(strName, method));
            }
            return Script.CreateObject(ret);
        }
        public override void SetValue(string strName, ScriptObject value)
        {
            m_Type.SetValue(Value, strName, value);
        }
    }
}
