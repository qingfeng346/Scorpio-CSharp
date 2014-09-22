using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;
using Scorpio.Variable;

namespace Scorpio
{
    //脚本数字类型
    public class ScriptNumber : ScriptPrimitiveObject<double>
    {
        public ScriptNumber() : base() { }
        public ScriptNumber(double value) : base(value) { }
        protected override void Initialize_impl()
        {
            Type = ObjectType.Number;
        }
        public ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    ++m_Value;
                    break;
                case CALC.PRE_DECREMENT:
                    --m_Value;
                    break;
                case CALC.POST_INCREMENT:
                    return new ScriptNumber(m_Value++);
                case CALC.POST_DECREMENT:
                    return new ScriptNumber(m_Value--);
            }
            return this;
        }
        public ScriptNumber Negative()
        {
            m_Value = -m_Value;
            return this;
        }
        public int ToInt32()
        {
            return Convert.ToInt32(Value);
        }
        public override void Assign(ScriptObject obj)
        {
            m_Value = ((ScriptNumber)obj).Value;
        }
        public override ScriptObject Plus(ScriptObject obj) {
            m_Value += ((ScriptNumber)obj).Value;
            return this;
        }
        public override ScriptObject Minus(ScriptObject obj)
        {
            m_Value -= ((ScriptNumber)obj).Value;
            return this;
        }
        public override ScriptObject Multiply(ScriptObject obj)
        {
            m_Value *= ((ScriptNumber)obj).Value;
            return this;
        }
        public override ScriptObject Divide(ScriptObject obj)
        {
            m_Value /= ((ScriptNumber)obj).Value;
            return this;
        }
        public override ScriptObject Modulo(ScriptObject obj)
        {
            m_Value %= ((ScriptNumber)obj).Value;
            return this;
        }
    }
}
