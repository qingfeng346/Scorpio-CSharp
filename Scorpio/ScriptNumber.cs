using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;
using Scorpio.Variable;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本数字类型
    public abstract class ScriptNumber : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Number; } }
        public abstract ScriptNumber Calc(CALC c);
        public abstract ScriptNumber Negative();
        public abstract bool Compare(TokenType type, CodeOperator oper, ScriptNumber num);
        protected ScriptNumber(Script script) : base(script) { }
        public int ToInt32()
        {
            return Util.ToInt32(ObjectValue);
        }
        public virtual double ToDouble()
        {
            return Util.ToDouble(ObjectValue);
        }
        public virtual long ToLong()
        {
            return Util.ToInt64(ObjectValue);
        }
    }
}
