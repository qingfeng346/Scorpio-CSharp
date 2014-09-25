using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Variable
{
    public abstract class ScriptPrimitiveObject<T> : ScriptObject
    {
        protected T m_Value;
        public T Value { get { return m_Value; } set { m_Value = value; } }
        public override object ObjectValue { get { return Value; } }
        public ScriptPrimitiveObject()
        {
            this.Value = default(T);
        }
        public ScriptPrimitiveObject(T value)
        {
            this.Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
