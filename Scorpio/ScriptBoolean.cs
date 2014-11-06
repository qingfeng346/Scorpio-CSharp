using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;

namespace Scorpio
{
    //脚本bool类型
    public class ScriptBoolean : ScriptObject
    {
        public static readonly ScriptBoolean True = new ScriptBoolean(true);
        public static readonly ScriptBoolean False = new ScriptBoolean(false);
        public override ObjectType Type { get { return ObjectType.Boolean; } }
        public override object ObjectValue { get { return Value; } }
        public bool Value { get; set; }
        public ScriptBoolean(bool value) : base(null)
        {
            this.Value = value;
        }
        public ScriptBoolean Inverse()
        {
            return Value ? False : True;
        }
        public static ScriptBoolean Get(bool b)
        {
            return b ? True : False;
        }
    }
}
