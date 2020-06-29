using System;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    /// <summary> 数组 object </summary>
    public class ScriptUserdataArray : ScriptUserdataObject {
        protected Array m_Array;
        protected Type m_ElementType;
        public ScriptUserdataArray(Array array, UserdataType type) : base(array, type) {
            m_Array = array;
            m_ElementType = type.Type.GetElementType();
        }
        public override ScriptValue GetValue(object index) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                return ScriptValue.CreateValue(m_Array.GetValue(Convert.ToInt32(index)));
            }
            return base.GetValue(index);
        }
        public override void SetValue(object index, ScriptValue value) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                m_Array.SetValue(Util.ChangeType(value, m_ElementType), Convert.ToInt32(index));
            } else {
                base.SetValue(index, value);
            }
        }
    }
}
