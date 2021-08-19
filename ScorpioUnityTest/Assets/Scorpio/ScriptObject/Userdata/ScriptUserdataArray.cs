using System;
using System.Collections;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    /// <summary> 数组 object </summary>
    public class ScriptUserdataArray : ScriptUserdataObject {
        protected IList m_Array;
        protected Type m_ElementType;
        public ScriptUserdataArray(IList array, UserdataType type) : base(array, type) {
            m_Array = array;
            m_ElementType = type.Type.GetElementType();
        }
        public override ScriptValue GetValue(double index) {
            return ScriptValue.CreateValue(m_Array[(int)index]);
        }
        public override ScriptValue GetValue(long index) {
            return ScriptValue.CreateValue(m_Array[(int)index]);
        }
        public override ScriptValue GetValue(object index) {
            return ScriptValue.CreateValue(m_Array[Convert.ToInt32(index)]);
        }
        public override void SetValue(double index, ScriptValue value) {
            m_Array[(int)index] = Util.ChangeType(value, m_ElementType);
        }
        public override void SetValue(long index, ScriptValue value) {
            m_Array[(int)index] = Util.ChangeType(value, m_ElementType);
        }
        public override void SetValue(object index, ScriptValue value) {
            m_Array[Convert.ToInt32(index)] = Util.ChangeType(value, m_ElementType);
        }
    }
}
