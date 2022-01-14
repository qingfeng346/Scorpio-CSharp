using System.Reflection;
using Scorpio.Exception;
using System;
using Scorpio.Tools;
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
            m_Property.CanRead.Assert($"Type:[{m_Type.FullName}]  Property:[{m_Property.Name}] 不支持 GetValue");
            return m_Property.GetValue(obj, null);
        }
        public override void SetValue(object obj, object val) {
            m_Property.CanWrite.Assert($"Type:[{m_Type.FullName}]  Property:[{m_Property.Name}] 不支持 SetValue");
            m_Property.SetValue(obj, val, null);
        }
    }
}
