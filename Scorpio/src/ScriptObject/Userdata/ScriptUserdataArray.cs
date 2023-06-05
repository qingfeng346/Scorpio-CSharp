using System;
using System.Collections;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    /// <summary> 数组 object </summary>
    public class ScriptUserdataArray : ScriptUserdataObject {
        protected IList m_Array;
        protected Type m_ElementType;
        public ScriptUserdataArray(Script script) : base(script) { }
        public ScriptUserdataArray Set(UserdataType type, IList array) {
            base.Set(type, array);
            m_Array = array;
            m_ElementType = type.Type.GetElementType();
            return this;
        }
        public override ScriptValue GetValue(double index) {
            using (var ret = ScriptValue.CreateValue(m_Script, m_Array[(int)index])) {
                return ret;
            }
        }
        public override ScriptValue GetValue(long index) {
            using (var ret = ScriptValue.CreateValue(m_Script, m_Array[(int)index])) {
                return ret;
            }
        }
        public override ScriptValue GetValue(object index) {
            using (var ret = ScriptValue.CreateValue(m_Script, Convert.ToInt32(index))) {
                return ret;
            }
        }
        public override void SetValue(double index, ScriptValue value) {
            m_Array[(int)index] = value.ChangeType(m_ElementType);
        }
        public override void SetValue(long index, ScriptValue value) {
            m_Array[(int)index] = value.ChangeType(m_ElementType);
        }
        public override void SetValue(object index, ScriptValue value) {
            m_Array[Convert.ToInt32(index)] = value.ChangeType(m_ElementType);
        }
    }
}
