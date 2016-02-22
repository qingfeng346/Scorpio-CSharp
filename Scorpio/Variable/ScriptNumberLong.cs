using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScriptNumberLong : ScriptNumber
    {
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return 1; } }
        public override object ObjectValue { get { return m_Value; } }
        public long Value { get { return m_Value; } }
        public long m_Value;
        public ScriptNumberLong(Script script, long value) : base(script)
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
                    return Script.CreateLong(m_Value++);
                case CALC.POST_DECREMENT:
                    return Script.CreateLong(m_Value--);
                default:
                    return this;
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            return Script.CreateLong(-m_Value);
        }
		public override ScriptNumber Abs ()
		{
			if (m_Value >= 0)
				return Script.CreateLong(m_Value);
			return Script.CreateLong(-m_Value);
		}
		public override ScriptNumber Floor ()
		{
			return Script.CreateLong (m_Value);
		}
		public override ScriptNumber Clamp (ScriptNumber min, ScriptNumber max)
		{
			if (m_Value < min.ToLong ())
				return Script.CreateLong (min.ToLong());
			if (m_Value > max.ToLong ())
				return Script.CreateLong (max.ToLong ());
			return Script.CreateLong (m_Value);
		}
        public override ScriptObject Assign()
        {
            return Script.CreateLong(m_Value);
        }
        public override long ToLong()
        {
            return m_Value;
        }
        public override bool Compare(TokenType type, ScriptObject num)
        {
            ScriptNumberLong val = num as ScriptNumberLong;
            if (val == null) throw new ExecutionException(Script, "数字比较 两边的数字类型不一致 请先转换再比较 ");
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
                    throw new ExecutionException(Script, "Long类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(Script, "赋值逻辑计算 右边值必须为数字类型");
            switch (type)
            {
                case TokenType.Plus:
                    return Script.CreateLong(m_Value + val.ToLong());
                case TokenType.Minus:
                    return Script.CreateLong(m_Value - val.ToLong());
                case TokenType.Multiply:
                    return Script.CreateLong(m_Value * val.ToLong());
                case TokenType.Divide:
                    return Script.CreateLong(m_Value / val.ToLong());
                case TokenType.Modulo:
                    return Script.CreateLong(m_Value % val.ToLong());
                case TokenType.InclusiveOr:
                    return Script.CreateLong(m_Value | val.ToLong());
                case TokenType.Combine:
                    return Script.CreateLong(m_Value & val.ToLong());
                case TokenType.XOR:
                    return Script.CreateLong(m_Value ^ val.ToLong());
                case TokenType.Shr:
                    return Script.CreateLong(m_Value >> val.ToInt32());
                case TokenType.Shi:
                    return Script.CreateLong(m_Value << val.ToInt32());
                default:
                    throw new ExecutionException(Script, "Long不支持的运算符 " + type);
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(Script, "赋值逻辑计算 右边值必须为数字类型");
            switch (type)
            {
                case TokenType.AssignPlus:
                    m_Value += val.ToLong();
                    return this;
                case TokenType.AssignMinus:
                    m_Value -= val.ToLong();
                    return this;
                case TokenType.AssignMultiply:
                    m_Value *= val.ToLong();
                    return this;
                case TokenType.AssignDivide:
                    m_Value /= val.ToLong();
                    return this;
                case TokenType.AssignModulo:
                    m_Value %= val.ToLong();
                    return this;
                case TokenType.AssignInclusiveOr:
                    m_Value |= val.ToLong();
                    return this;
                case TokenType.AssignCombine:
                    m_Value &= val.ToLong();
                    return this;
                case TokenType.AssignXOR:
                    m_Value ^= val.ToLong();
                    return this;
                case TokenType.AssignShr:
                    m_Value >>= val.ToInt32();
                    return this;
                case TokenType.AssignShi:
                    m_Value <<= val.ToInt32();
                    return this;
                default:
                    throw new ExecutionException(Script, "Long不支持的运算符 " + type);
            }
        }
        public override ScriptObject Clone()
        {
            return Script.CreateLong(m_Value);
        }
    }
}
