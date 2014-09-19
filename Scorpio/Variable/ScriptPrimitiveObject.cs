using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio.Variable
{
    public abstract class ScriptPrimitiveObject<T> : IScriptPrimitiveObject
    {
        protected T m_Value;
        public T Value { get { return m_Value; } set { m_Value = value; } }
        public ScriptPrimitiveObject()
        {
            this.Value = default(T);
            Initialize();
        }
        public ScriptPrimitiveObject(T value)
        {
            this.Value = value;
            Initialize();
        }
        private void Initialize()
        {
            Initialize_impl();
        }
        protected abstract void Initialize_impl();
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
