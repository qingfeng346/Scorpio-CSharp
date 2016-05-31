using System;
using Scorpio.CodeDom;
namespace Scorpio
{
    //脚本数字类型
    public abstract class ScriptNumber : ScriptObject
    {
        protected ScriptNumber(Script script) : base(script) { }
        public override ObjectType Type { get { return ObjectType.Number; } }
        public abstract ScriptNumber Calc(CALC c);
        public abstract ScriptNumber Negative();										//取相反值
		public abstract ScriptNumber Abs();												//取绝对值
		public abstract ScriptNumber Floor();											//取数的整数
		public abstract ScriptNumber Clamp(ScriptNumber min, ScriptNumber max);			//取值的区间
		public ScriptNumber Sqrt () {													//取平方根
			return m_Script.CreateDouble (Math.Sqrt (ToDouble()));
		}
		public ScriptNumber Pow (ScriptNumber value) {									//取几次方
			return m_Script.CreateDouble (Math.Pow (ToDouble(), value.ToDouble()));
		}
        public virtual int ToInt32() { return Util.ToInt32(ObjectValue); }
        public virtual double ToDouble() { return Util.ToDouble(ObjectValue); }
        public virtual long ToLong() { return Util.ToInt64(ObjectValue); }
    }
}
