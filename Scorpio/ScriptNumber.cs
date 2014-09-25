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
    public abstract class ScriptNumber : IScriptPrimitiveObject
    {
        public enum NUMBER_TYPE
        {
            DOUBLE,
            LONG,
            ULONG,
        }
        public override ObjectType Type { get { return ObjectType.Number; } }
        public abstract NUMBER_TYPE NumberType { get; }
        public abstract ScriptNumber Calc(CALC c);
        public abstract ScriptNumber Negative();
        public abstract bool Compare(TokenType type, CodeOperator oper, ScriptNumber num);
        public int ToInt32()
        {
            return Convert.ToInt32(ObjectValue);
        }
        public double ToDouble()
        {
            return Convert.ToDouble(ObjectValue);
        }
        public long ToLong()
        {
            return Convert.ToInt64(ObjectValue);
        }
        public ulong ToULong()
        {
            return Convert.ToUInt64(ObjectValue);
        }
        public override string ToString()
        {
            return ObjectValue.ToString();
        }
    }
}
