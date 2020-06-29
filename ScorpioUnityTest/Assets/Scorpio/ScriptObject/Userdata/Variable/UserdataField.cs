using System.Reflection;
namespace Scorpio.Userdata {
    public class UserdataField : UserdataVariable {
        private FieldInfo m_Field;
        public UserdataField(FieldInfo info) {
            m_Field = info;
            FieldType = info.FieldType;
        }
        public override object GetValue(object obj) {
            return m_Field.GetValue(obj);
        }
        public override void SetValue(object obj, object value) {
            m_Field.SetValue(obj, value);
        }
    }
}
