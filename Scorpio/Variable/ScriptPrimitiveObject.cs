using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
namespace Scorpio.Variable
{
    public abstract class ScriptPrimitiveObject<T> : ScriptObject
    {
        public T Value { get; set; }
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
