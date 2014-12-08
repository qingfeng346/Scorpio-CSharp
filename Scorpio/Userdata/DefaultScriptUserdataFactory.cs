using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Userdata
{
    /// <summary> 默认的Userdata工厂类 </summary>
    public class DefaultScriptUserdataFactory : IScriptUserdataFactory
    {
        private Script m_Script;
        private Dictionary<Type, DefaultScriptUserdataEnum> m_Enums = new Dictionary<Type, DefaultScriptUserdataEnum>();                        //所有枚举集合
        private Dictionary<Type, DefaultScriptUserdataDelegateType> m_Delegates = new Dictionary<Type, DefaultScriptUserdataDelegateType>();    //所有委托类型集合
        private Dictionary<Type, UserdataType> m_Types = new Dictionary<Type, UserdataType>();                                                  //所有的类集合
        public DefaultScriptUserdataEnum GetEnum(Type type)
        {
            if (m_Enums.ContainsKey(type))
                return m_Enums[type];
            DefaultScriptUserdataEnum ret = new DefaultScriptUserdataEnum(m_Script, type);
            m_Enums.Add(type, ret);
            return ret;
        }
        public DefaultScriptUserdataDelegateType GetDelegate(Type type)
        {
            if (m_Delegates.ContainsKey(type))
                return m_Delegates[type];
            DefaultScriptUserdataDelegateType ret = new DefaultScriptUserdataDelegateType(m_Script, type);
            m_Delegates.Add(type, ret);
            return ret;
        }
        public UserdataType GetScorpioType(Type type)
        {
            if (m_Types.ContainsKey(type))
                return m_Types[type];
            UserdataType scorpioType = new UserdataType(m_Script, type);
            m_Types.Add(type, scorpioType);
            return scorpioType;
        }
        public DefaultScriptUserdataFactory(Script script)
        {
            m_Script = script;
        }
        public ScriptUserdata create(Script script, object obj)
        {
            Type type = obj as Type;
            if (type != null) {
                if (Util.IsEnum(type))
                    return GetEnum(type);
                else if (Util.IsDelegateType(type))
                    return GetDelegate(type);
                else
                    return new DefaultScriptUserdataObjectType(script, type, GetScorpioType(type));
            }
            if (obj is Delegate)
                return new DefaultScriptUserdataDelegate(script, (Delegate)obj);
            return new DefaultScriptUserdataObject(script, obj, GetScorpioType(obj.GetType()));
        }
    }
}
