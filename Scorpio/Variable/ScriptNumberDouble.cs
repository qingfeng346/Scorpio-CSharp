using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScriptNumberDouble : ScriptNumber
    {
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return 0; } }
        public override object ObjectValue { get { return m_Value; } }
        public double Value { get { return m_Value; } }
        public double m_Value;
        public ScriptNumberDouble(Script script, double value) : base(script)
        {
            m_Value = value;
        }
        public override ScriptNumber Calc(CALC c)
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
                    return Script.CreateDouble(m_Value++);
                case CALC.POST_DECREMENT:
                    return Script.CreateDouble(m_Value--);
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            m_Value = -m_Value;
            return this;
        }
        public override ScriptObject Assign()
        {
            return Script.CreateDouble(m_Value);
        }
        public override double ToDouble()
        {
            return m_Value;
        }
        public override ScriptObject ComputePlus(ScriptNumber obj) 
        {
            return Script.CreateDouble(m_Value + obj.ToDouble());
        }
        public override ScriptObject ComputeMinus(ScriptNumber obj)
        {
            return Script.CreateDouble(m_Value - obj.ToDouble());
        }
        public override ScriptObject ComputeMultiply(ScriptNumber obj)
        {
            return Script.CreateDouble(m_Value * obj.ToDouble());
        }
        public override ScriptObject ComputeDivide(ScriptNumber obj)
        {
            return Script.CreateDouble(m_Value / obj.ToDouble());
        }
        public override ScriptObject ComputeModulo(ScriptNumber obj)
        {
            return Script.CreateDouble(m_Value % obj.ToDouble());
        }
        public override ScriptObject AssignPlus(ScriptNumber obj)
        {
            m_Value += obj.ToDouble();
            return this;
        }
        public override ScriptObject AssignMinus(ScriptNumber obj)
        {
            m_Value -= obj.ToDouble();
            return this;
        }
        public override ScriptObject AssignMultiply(ScriptNumber obj)
        {
            m_Value *= obj.ToDouble();
            return this;
        }
        public override ScriptObject AssignDivide(ScriptNumber obj)
        {
            m_Value /= obj.ToDouble();
            return this;
        }
        public override ScriptObject AssignModulo(ScriptNumber obj)
        {
            m_Value %= obj.ToDouble();
            return this;
        }
        public override bool Compare(TokenType type, CodeOperator oper, ScriptNumber num)
        {
            ScriptNumberDouble val = num as ScriptNumberDouble;
            if (val == null) throw new ExecutionException("数字比较 两边的数字类型不一致 请先转换再比较 ");
            switch (type)
            {
                case TokenType.Greater:
                    return m_Value > val.m_Value;
                case TokenType.GreaterOrEqual:
                    return m_Value >= val.m_Value;
                case TokenType.Less:
                    return m_Value < val.m_Value;
                case TokenType.LessOrEqual:
                    return m_Value <= val.m_Value;
            }
            return false;
        }
        public override ScriptObject Clone()
        {
            return Script.CreateDouble(m_Value);
        }
    }
}
