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
        public ScriptBoolean() : base() { }
        public ScriptBoolean(bool value) : base(value) { }
        protected override void Initialize_impl()
        {
            Type = ObjectType.Boolean;
        }
        public ScriptBoolean Inverse()
        {
            return Value ? False : True;
        }
    }
}
