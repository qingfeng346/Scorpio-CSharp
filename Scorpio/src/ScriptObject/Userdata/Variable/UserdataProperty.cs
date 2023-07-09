using System.Reflection;
using Scorpio.Tools;
using Scorpio.Exception;
namespace Scorpio.Userdata {
    public class UserdataProperty : UserdataVariable {
        private PropertyInfo m_Property;
        public UserdataProperty(PropertyInfo info) {
            m_Property = info;
            FieldType = info.PropertyType;
        }
        public override object GetValue(object obj) {
#if SCORPIO_ASSERT
            if (!m_Property.CanRead)
                throw new ExecutionException($"Property:[{m_Property.Name}] 不支持 GetValue");
#endif
            return m_Property.GetValue(obj, null);
        }
        public override void SetValue(object obj, object val) {
#if SCORPIO_ASSERT
            if (!m_Property.CanWrite)
                throw new ExecutionException($"Property:[{m_Property.Name}] 不支持 SetValue");
#endif
            m_Property.SetValue(obj, val, null);
        }
    }
}
