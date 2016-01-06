using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    //脚本null类型
    public class ScriptNull : ScriptObject
    {
        public ScriptNull(Script script) : base(script) { }
        public override ObjectType Type { get { return ObjectType.Null; } }
        public override object ObjectValue { get { return null; } }
        public override string ToString() { return "null"; }
        public override string ToJson() { return "null"; }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ScriptNull)) return false;
            return true;
        }
        public override int GetHashCode() { return 0; }
        public override bool LogicOperation() { return false; }
    }
}
