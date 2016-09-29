using System;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScriptNumberLong : ScriptNumber
    {
        private long m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeLong; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public long Value { get { return m_Value; } }
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
                    return new ScriptNumberLong(m_Script, m_Value++);
                case CALC.POST_DECREMENT:
                    return new ScriptNumberLong(m_Script, m_Value--);
                default:
                    return this;
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            return new ScriptNumberLong(m_Script, -m_Value);
        }
		public override ScriptNumber Abs ()
		{
			if (m_Value >= 0)
				return new ScriptNumberLong(m_Script, m_Value);
			return new ScriptNumberLong(m_Script, -m_Value);
		}
		public override ScriptNumber Floor ()
		{
			return new ScriptNumberLong(m_Script, m_Value);
		}
		public override ScriptNumber Clamp (ScriptNumber min, ScriptNumber max)
		{
            long val = min.ToLong();
			if (m_Value < val)
				return new ScriptNumberLong(m_Script, val);
            val = max.ToLong();
			if (m_Value > val)
				return new ScriptNumberLong(m_Script, val);
			return new ScriptNumberLong(m_Script, m_Value);
		}
        public override ScriptObject Assign()
        {
            return new ScriptNumberLong(m_Script, m_Value);
        }
        public override long ToLong()
        {
            return m_Value;
        }
        public override bool Compare(TokenType type, ScriptObject num)
        {
            ScriptNumberLong val = num as ScriptNumberLong;
            if (val == null) throw new ExecutionException(m_Script, this, "数字比较 两边的数字类型不一致 请先转换再比较 ");
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
                    throw new ExecutionException(m_Script, this, "Long类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(m_Script, this, "赋值逻辑计算 右边值必须为数字类型");
            switch (type)
            {
                case TokenType.Plus:
                    return new ScriptNumberLong(m_Script, m_Value + val.ToLong());
                case TokenType.Minus:
                    return new ScriptNumberLong(m_Script, m_Value - val.ToLong());
                case TokenType.Multiply:
                    return new ScriptNumberLong(m_Script, m_Value * val.ToLong());
                case TokenType.Divide:
                    return new ScriptNumberLong(m_Script, m_Value / val.ToLong());
                case TokenType.Modulo:
                    return new ScriptNumberLong(m_Script, m_Value % val.ToLong());
                case TokenType.InclusiveOr:
                    return new ScriptNumberLong(m_Script, m_Value | val.ToLong());
                case TokenType.Combine:
                    return new ScriptNumberLong(m_Script, m_Value & val.ToLong());
                case TokenType.XOR:
                    return new ScriptNumberLong(m_Script, m_Value ^ val.ToLong());
                case TokenType.Shr:
                    return new ScriptNumberLong(m_Script, m_Value >> val.ToInt32());
                case TokenType.Shi:
                    return new ScriptNumberLong(m_Script, m_Value << val.ToInt32());
                default:
                    throw new ExecutionException(m_Script, this, "Long不支持的运算符 " + type);
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(m_Script, this, "赋值逻辑计算 右边值必须为数字类型");
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
                    throw new ExecutionException(m_Script, this, "Long不支持的运算符 " + type);
            }
        }
        public override ScriptObject Clone()
        {
            return new ScriptNumberLong(m_Script, m_Value);
        }
    }
}
