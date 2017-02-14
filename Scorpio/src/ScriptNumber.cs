using System;
using Scorpio.CodeDom;
using Scorpio.Exception;
namespace Scorpio
{
    public class NumberType {
        public const int TypeDouble = 0;
        public const int TypeLong = 1;
        public const int TypeInt = 2;
        public const int TypeSByte = 3;
        public const int TypeByte = 4;
        public const int TypeShort = 5;
        public const int TypeUShort = 6;
        public const int TypeUInt = 7;
        public const int TypeULong = 8;
        public const int TypeFloat = 9;
    }
    //脚本数字类型
    public abstract class ScriptNumber : ScriptObject
    {
        protected ScriptNumber(Script script) : base(script) { }
        public override ObjectType Type { get { return ObjectType.Number; } }
        public virtual ScriptNumber Calc(CALC c) {                                      //数字计算
            throw new ExecutionException(m_Script, "数字类型不支持Calc函数");
        }
        public virtual ScriptNumber Minus() {                                           //取相反值 -
            throw new ExecutionException(m_Script, "数字类型不支持Minus函数");
        }
        public virtual ScriptNumber Negative() {                                        //取反操作 ~
            throw new ExecutionException(m_Script, "数字类型不支持Negative函数");
        }
        public virtual ScriptNumber Abs() {                                             //取绝对值
            throw new ExecutionException(m_Script, "数字类型不支持Abs函数");
        }
        public virtual ScriptNumber Floor() {                                           //取数的整数
            throw new ExecutionException(m_Script, "数字类型不支持Floor函数");
        }
        public virtual ScriptNumber Clamp(ScriptNumber min, ScriptNumber max) {         //取值的区间
            throw new ExecutionException(m_Script, "数字类型不支持Clamp函数");
        }
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
