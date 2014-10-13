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
        public ScriptPrimitiveObject(T value) : base(null)
        {
            this.Value = value;
        }
        public ScriptPrimitiveObject(Script script, T value) : base(script)
        {
            this.Value = value;
        }
    }
}
