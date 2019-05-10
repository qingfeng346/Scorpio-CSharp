using System.Reflection;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    public class UserdataProperty : UserdataVariable {
        private PropertyInfo m_Property;
        public UserdataProperty(Script script, PropertyInfo info) {
            m_Script = script;
            m_Property = info;
            Name = info.Name;
            FieldType = info.PropertyType;
        }
        public override object GetValue(object obj) {
            if (m_Property.CanRead)
                return m_Property.GetValue(obj, null);
            throw new ExecutionException("Property [" + Name + "] 不支持 GetValue");
        }
        public override void SetValue(object obj, object val) {
            if (m_Property.CanWrite)
                m_Property.SetValue(obj, val, null);
            else
                throw new ExecutionException("Property [" + Name + "] 不支持 SetValue");
        }
    }
}
