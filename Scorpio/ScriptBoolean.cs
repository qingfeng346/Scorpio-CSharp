using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.Variable;

namespace Scorpio
{
    //脚本bool类型
    public class ScriptBoolean : ScriptPrimitiveObject<bool>
    {
        public static readonly ScriptBoolean True = new ScriptBoolean(true);
        public static readonly ScriptBoolean False = new ScriptBoolean(false);
        public override ObjectType Type { get { return ObjectType.Boolean; } }
        private ScriptBoolean(bool value) : base(value) { }
        public ScriptBoolean Inverse()
        {
            return Value ? False : True;
        }
        public override ScriptObject Clone()
        {
            return this;
        }
        public static ScriptBoolean Get(bool b)
        {
            return b ? True : False;
        }
    }
}
