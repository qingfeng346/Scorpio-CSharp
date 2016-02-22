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
		public override ScriptNumber Abs ()
		{
			if (m_Value >= 0)
				return Script.CreateDouble(m_Value);
			return Script.CreateDouble(-m_Value);
		}
		public override ScriptNumber Floor ()
		{
			return Script.CreateDouble (Math.Floor (m_Value));
		}
		public override ScriptNumber Clamp (ScriptNumber min, ScriptNumber max)
		{
			if (m_Value < min.ToDouble ())
				return Script.CreateDouble (min.ToDouble());
			if (m_Value > max.ToDouble ())
				return Script.CreateDouble (max.ToDouble ());
			return Script.CreateDouble (m_Value);
		}
        public override ScriptObject Assign()
        {
            return Script.CreateDouble(m_Value);
        }
        public override double ToDouble()
        {
            return m_Value;
        }
        public override bool Compare(TokenType type, ScriptObject obj)
        {
            ScriptNumberDouble val = obj as ScriptNumberDouble;
            if (val == null) throw new ExecutionException(Script, "数字比较 两边的数字类型不一致 请先转换再比较");
            switch (type) {
                case TokenType.Greater:
                    return m_Value > val.m_Value;
                case TokenType.GreaterOrEqual:
                    return m_Value >= val.m_Value;
                case TokenType.Less:
                    return m_Value < val.m_Value;
                case TokenType.LessOrEqual:
                    return m_Value <= val.m_Value;
                default:
                    throw new ExecutionException(Script, "Double类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(Script, "逻辑计算 右边值必须为数字类型");
            switch (type) {
                case TokenType.Plus:
                    return Script.CreateDouble(m_Value + val.ToDouble());
                case TokenType.Minus:
                    return Script.CreateDouble(m_Value - val.ToDouble());
                case TokenType.Multiply:
                    return Script.CreateDouble(m_Value * val.ToDouble());
                case TokenType.Divide:
                    return Script.CreateDouble(m_Value / val.ToDouble());
                case TokenType.Modulo:
                    return Script.CreateDouble(m_Value % val.ToDouble());
                default:
                    throw new ExecutionException(Script, "Double不支持的运算符 " + type);
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(Script, "赋值逻辑计算 右边值必须为数字类型");
            switch (type) {
                case TokenType.AssignPlus:
                    m_Value += val.ToDouble();
                    return this;
                case TokenType.AssignMinus:
                    m_Value -= val.ToDouble();
                    return this;
                case TokenType.AssignMultiply:
                    m_Value *= val.ToDouble();
                    return this;
                case TokenType.AssignDivide:
                    m_Value /= val.ToDouble();
                    return this;
                case TokenType.AssignModulo:
                    m_Value %= val.ToDouble();
                    return this;
                default:
                    throw new ExecutionException(Script, "Double不支持的运算符 " + type);
            }
        }
        public override ScriptObject Clone()
        {
            return Script.CreateDouble(m_Value);
        }
    }
}
