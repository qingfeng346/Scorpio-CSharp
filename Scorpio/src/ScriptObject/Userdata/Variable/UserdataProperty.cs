using System.Reflection;
using Scorpio.Exception;
using System;
namespace Scorpio.Userdata {
    public class UserdataProperty : UserdataVariable {
        private Type m_Type;
        private PropertyInfo m_Property;
        public UserdataProperty(Type type, PropertyInfo info) {
            m_Type = type;
            m_Property = info;
            FieldType = info.PropertyType;
        }
        public override object GetValue(object obj) {
#if SCORPIO_DEBUG
            if (!m_Property.CanRead) throw new ExecutionException($"Type:[{m_Type.FullName}]  Property:[{m_Property.Name}] 不支持 GetValue");
#endif
            return m_Property.GetValue(obj, null);
        }
        public override void SetValue(object obj, object val) {
#if SCORPIO_DEBUG
            if (!m_Property.CanWrite) throw new ExecutionException($"Type:[{m_Type.FullName}]  Property:[{m_Property.Name}] 不支持 SetValue");
#endif
            m_Property.SetValue(obj, val, null);
        }
    }
}
