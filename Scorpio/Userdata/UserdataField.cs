using System;
using System.Reflection;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    public class UserdataField
    {
        private Script m_Script;
        public string Name;
        public Type FieldType;
        private FieldInfo m_Field;
        private PropertyInfo m_Property;
        private EventInfo m_Event;
        public UserdataField(Script script, FieldInfo info)
        {
            m_Script = script;
            Name = info.Name;
            FieldType = info.FieldType;
            m_Field = info;
        }
        public UserdataField(Script script, PropertyInfo info)
        {
            m_Script = script;
            Name = info.Name;
            FieldType = info.PropertyType;
            m_Property = info;
        }
        public UserdataField(Script script, EventInfo info)
        {
            m_Script = script;
            Name = info.Name;
            FieldType = info.EventHandlerType;
            m_Event = info;
        }
        public object GetValue(object obj)
        {
            if (m_Field != null)
                return m_Field.GetValue(obj);
            else if (m_Property != null)
                return m_Property.GetValue(obj, null);
            else if (m_Event != null)
                return new BridgeEventInfo(obj, m_Event);
            throw new ExecutionException(m_Script, "变量 [" + Name + "] 不支持GetValue");
        }
        public void SetValue(object obj, object val)
        {
            if (m_Field != null)
                m_Field.SetValue(obj, val);
            else if (m_Property != null)
                m_Property.SetValue(obj, val, null);
            else
                throw new ExecutionException(m_Script, "变量 [" + Name + "] 不支持SetValue");
        }
    }
}
