using System;
using System.Collections.Generic;
using System.Text;

namespace Scorpio
{
    //脚本null类型
    public class ScriptNull : ScriptObject
    {
        private static ScriptNull s_ScriptNull;
        public static ScriptNull Instance { get { if (s_ScriptNull == null) s_ScriptNull = new ScriptNull(); return s_ScriptNull; } }
        public ScriptNull() : base(null) { }
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
    }
}
