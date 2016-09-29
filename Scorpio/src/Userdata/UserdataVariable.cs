using System;
using System.Reflection;
using Scorpio;
using Scorpio.Exception;
namespace Scorpio.Userdata
{
    public abstract class UserdataVariable {
        protected Script m_Script;
        public string Name;
        public Type FieldType;
        public abstract object GetValue(object obj);
        public abstract void SetValue(object obj, object val);
    }
    public class UserdataField : UserdataVariable {
        private FieldInfo m_Field;
        public UserdataField(Script script, FieldInfo info) {
            m_Script = script;
            Name = info.Name;
            FieldType = info.FieldType;
            m_Field = info;
        }
        public override object GetValue(object obj) {
            return m_Field.GetValue(obj);
        }
        public override void SetValue(object obj, object val) {
            m_Field.SetValue(obj, val);
        }
    }
    public class UserdataProperty : UserdataVariable {
        private PropertyInfo m_Property;
        public UserdataProperty(Script script, PropertyInfo info) {
            m_Script = script;
            Name = info.Name;
            FieldType = info.PropertyType;
            m_Property = info;
        }
        public override object GetValue(object obj) {
            if (m_Property.CanRead)
                return m_Property.GetValue(obj, null);
            throw new ExecutionException(m_Script, "Property [" + Name + "] 不支持GetValue");
        }
        public override void SetValue(object obj, object val) {
            if (m_Property.CanWrite)
                m_Property.SetValue(obj, val, null);
            else
                throw new ExecutionException(m_Script, "Property [" + Name + "] 不支持SetValue");
        }
    }
    public class UserdataEvent : UserdataVariable {
        private EventInfo m_Event;
        public UserdataEvent(Script script, EventInfo info) {
            m_Script = script;
            Name = info.Name;
            FieldType = info.EventHandlerType;
            m_Event = info;
        }
        public override object GetValue(object obj) {
            return new BridgeEventInfo(obj, m_Event);
        }
        public override void SetValue(object obj, object val) {
            throw new ExecutionException(m_Script, "Event [" + Name + "] 不支持SetValue");
        }
    }
}
