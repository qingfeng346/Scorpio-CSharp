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
            return ScriptValue.CreateValue(m_Array.GetValue(Convert.ToInt32(index)));
        }
        public override void SetValue(object index, ScriptValue value) {
            m_Array.SetValue(Util.ChangeType(value, m_ElementType), Convert.ToInt32(index));
        }
    }
}
