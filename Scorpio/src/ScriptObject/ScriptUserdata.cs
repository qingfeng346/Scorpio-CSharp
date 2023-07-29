using System;
namespace Scorpio {
    //语言数据
    public abstract class ScriptUserdata : ScriptObject {
        protected object m_Value;       //值
        protected Type m_ValueType;     //值类型
        public override Type ValueType => m_ValueType;      //值类型，如果是Type则返回 typeof(Type)
        public override Type Type => m_ValueType;           //获取类型
        public override object Value => m_Value;            //值
        public override int GetHashCode() { return m_Value.GetHashCode(); }
        public override string ToString() { return m_Value.ToString(); }
        public ScriptUserdata() : base(ObjectType.UserData) { }
    }
}
