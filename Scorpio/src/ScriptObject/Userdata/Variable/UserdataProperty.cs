using System.Reflection;
using Scorpio.Tools;
namespace Scorpio.Userdata {
    public class UserdataProperty : UserdataVariable {
        private PropertyInfo m_Property;
        public UserdataProperty(PropertyInfo info) {
            m_Property = info;
            FieldType = info.PropertyType;
        }
        public override object GetValue(object obj) {
            m_Property.CanRead.Assert($"Property:[{m_Property.Name}] 不支持 GetValue");
            return m_Property.GetValue(obj, null);
        }
        public override void SetValue(object obj, object val) {
            m_Property.CanWrite.Assert($"Property:[{m_Property.Name}] 不支持 SetValue");
            m_Property.SetValue(obj, val, null);
        }
    }
}
