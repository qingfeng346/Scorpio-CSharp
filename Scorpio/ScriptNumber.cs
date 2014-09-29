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
        protected Script m_Script;
        public abstract ScriptNumber Calc(CALC c);
        public abstract ScriptNumber Negative();
        public abstract bool Compare(TokenType type, CodeOperator oper, ScriptNumber num);
        public int ToInt32()
        {
            return Convert.ToInt32(ObjectValue);
        }
        public override ScriptObject Assign()
        {
            return m_Script.CreateNumber(ObjectValue);
        }
        public virtual double ToDouble()
        {
            return Convert.ToDouble(ObjectValue);
        }
        public virtual long ToLong()
        {
            return Convert.ToInt64(ObjectValue);
        }
        public virtual ulong ToULong()
        {
            return Convert.ToUInt64(ObjectValue);
        }
        public override string ToString()
        {
            return ObjectValue.ToString();
        }
    }
}
