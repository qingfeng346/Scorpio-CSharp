using System;
using System.Text;
namespace Scorpio {
    public class ScriptStringBuilder : ScriptInstance {
        public StringBuilder Builder { get; } = new StringBuilder();
        public ScriptStringBuilder(Script script) : base(ObjectType.StringBuilder, script.TypeStringBuilderValue) { }
        internal ScriptStringBuilder(Script script, ScriptValue[] parameters, int length) : this(script) {
            for (var i = 0; i < length; ++i) {
                Builder.Append(parameters[i]);
            }
        }
        public override ScriptValue GetValue(object index) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                return new ScriptValue(Builder[Convert.ToInt32(index)]);
            }
            return base.GetValue(index);
        }
        public override void SetValue(object index, ScriptValue value) {
            if (index is double || index is long || index is sbyte || index is byte || index is short || index is ushort || index is int || index is uint || index is float) {
                Builder[Convert.ToInt32(index)] = value.ToChar();
            } else {
                base.SetValue(index, value);
            }
        }
        public override string ToString() {
            return Builder.ToString();
        }
    }
}
