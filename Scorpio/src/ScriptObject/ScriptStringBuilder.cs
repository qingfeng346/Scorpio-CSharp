using System;
using System.Text;
namespace Scorpio {
    public class ScriptStringBuilder : ScriptInstance {
        public StringBuilder Builder { get; } = new StringBuilder();
        public ScriptStringBuilder(Script script) : base(script, ObjectType.StringBuilder) {
            Set(script.TypeStringBuilder);
        }
        public override ScriptValue GetValue(double index) {
            return new ScriptValue(Builder[(int)index]);
        }
        public override ScriptValue GetValue(long index) {
            return new ScriptValue(Builder[(int)index]);
        }
        public override ScriptValue GetValue(object index) {
            return new ScriptValue(Builder[Convert.ToInt32(index)]);
        }
        public override void SetValue(double index, ScriptValue value) {
            Builder[(int)index] = value.ToChar();
        }
        public override void SetValue(long index, ScriptValue value) {
            Builder[(int)index] = value.ToChar();
        }
        public override void SetValue(object index, ScriptValue value) {
            Builder[Convert.ToInt32(index)] = value.ToChar();
        }
        public override string ToString() {
            return Builder.ToString();
        }
    }
}
