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
            switch (c) {
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
                default:
                    return this;
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            return Script.CreateDouble(-m_Value);
        }
        public override ScriptObject Assign()
        {
            return Script.CreateDouble(m_Value);
        }
        public override double ToDouble()
        {
            return m_Value;
        }
        public override ScriptObject Compute(TokenType type, ScriptNumber obj)
        {
            switch (type)
            {
                case TokenType.Plus:
                    return Script.CreateDouble(m_Value + obj.ToDouble());
                case TokenType.Minus:
                    return Script.CreateDouble(m_Value - obj.ToDouble());
                case TokenType.Multiply:
                    return Script.CreateDouble(m_Value * obj.ToDouble());
                case TokenType.Divide:
                    return Script.CreateDouble(m_Value / obj.ToDouble());
                case TokenType.Modulo:
                    return Script.CreateDouble(m_Value % obj.ToDouble());
                default:
                    throw new ExecutionException("Double不支持的运算符 " + type);
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptNumber obj)
        {
            switch (type)
            {
                case TokenType.AssignPlus:
                    m_Value += obj.ToDouble();
                    return this;
                case TokenType.AssignMinus:
                    m_Value -= obj.ToDouble();
                    return this;
                case TokenType.AssignMultiply:
                    m_Value *= obj.ToDouble();
                    return this;
                case TokenType.AssignDivide:
                    m_Value /= obj.ToDouble();
                    return this;
                case TokenType.AssignModulo:
                    m_Value %= obj.ToDouble();
                    return this;
                default:
                    throw new ExecutionException("Double不支持的运算符 " + type);
            }
        }
        public override bool Compare(TokenType type, ScriptNumber num)
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
                default:
                    throw new ExecutionException("Number类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Clone()
        {
            return Script.CreateDouble(m_Value);
        }
    }
}
