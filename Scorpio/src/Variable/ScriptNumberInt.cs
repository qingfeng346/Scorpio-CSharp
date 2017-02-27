using System;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScriptNumberInt : ScriptNumber
    {
        private int m_Value;
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return NumberType.TypeInt; } }
        public override object ObjectValue { get { return m_Value; } }
        public override object KeyValue { get { return m_Value; } }
        public int Value { get { return m_Value; } }
        public ScriptNumberInt(Script script, int value) : base(script)
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
                    return new ScriptNumberInt(m_Script, m_Value++);
                case CALC.POST_DECREMENT:
                    return new ScriptNumberInt(m_Script, m_Value--);
                default:
                    return this;
            }
            return this;
        }
        public override ScriptNumber Minus() {
            return new ScriptNumberInt(m_Script, -m_Value);
        }
        public override ScriptNumber Negative() {
            return new ScriptNumberInt(m_Script, ~m_Value);
        }
        public override ScriptNumber Abs () {
			if (m_Value >= 0)
				return new ScriptNumberInt(m_Script, m_Value);
			return new ScriptNumberInt(m_Script, -m_Value);
		}
		public override ScriptNumber Floor () {
			return new ScriptNumberInt(m_Script, m_Value);
		}
		public override ScriptNumber Clamp (ScriptNumber min, ScriptNumber max) {
			if (m_Value < min.ToInt32 ())
				return new ScriptNumberInt(m_Script, min.ToInt32());
			if (m_Value > max.ToInt32 ())
				return new ScriptNumberInt(m_Script, max.ToInt32());
			return new ScriptNumberInt(m_Script, m_Value);
		}
        public override ScriptObject Assign() {
            return new ScriptNumberInt(m_Script, m_Value);
        }
        public override int ToInt32()
        {
            return m_Value;
        }
        public override bool Compare(TokenType type, ScriptObject num)
        {
            ScriptNumberInt val = num as ScriptNumberInt;
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
                    throw new ExecutionException(m_Script, this, "Int类型 操作符[" + type + "]不支持");
            }
        }
        public override ScriptObject Compute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(m_Script, this, "逻辑计算 右边值必须为数字类型");
            switch (type)
            {
                case TokenType.Plus:
                    return new ScriptNumberInt(m_Script, m_Value + val.ToInt32());
                case TokenType.Minus:
                    return new ScriptNumberInt(m_Script, m_Value - val.ToInt32());
                case TokenType.Multiply:
                    return new ScriptNumberInt(m_Script, m_Value * val.ToInt32());
                case TokenType.Divide:
                    return new ScriptNumberInt(m_Script, m_Value / val.ToInt32());
                case TokenType.Modulo:
                    return new ScriptNumberInt(m_Script, m_Value % val.ToInt32());
                case TokenType.InclusiveOr:
                    return new ScriptNumberInt(m_Script, m_Value | val.ToInt32());
                case TokenType.Combine:
                    return new ScriptNumberInt(m_Script, m_Value & val.ToInt32());
                case TokenType.XOR:
                    return new ScriptNumberInt(m_Script, m_Value ^ val.ToInt32());
                case TokenType.Shr:
                    return new ScriptNumberInt(m_Script, m_Value >> val.ToInt32());
                case TokenType.Shi:
                    return new ScriptNumberInt(m_Script, m_Value << val.ToInt32());
                default:
                    throw new ExecutionException(m_Script, this, "Int不支持的运算符 " + type);
            }
        }
        public override ScriptObject AssignCompute(TokenType type, ScriptObject obj)
        {
            ScriptNumber val = obj as ScriptNumber;
            if (val == null) throw new ExecutionException(m_Script, this, "赋值逻辑计算 右边值必须为数字类型");
            switch (type)
            {
                case TokenType.AssignPlus:
                    m_Value += val.ToInt32();
                    return this;
                case TokenType.AssignMinus:
                    m_Value -= val.ToInt32();
                    return this;
                case TokenType.AssignMultiply:
                    m_Value *= val.ToInt32();
                    return this;
                case TokenType.AssignDivide:
                    m_Value /= val.ToInt32();
                    return this;
                case TokenType.AssignModulo:
                    m_Value %= val.ToInt32();
                    return this;
                case TokenType.AssignInclusiveOr:
                    m_Value |= val.ToInt32();
                    return this;
                case TokenType.AssignCombine:
                    m_Value &= val.ToInt32();
                    return this;
                case TokenType.AssignXOR:
                    m_Value ^= val.ToInt32();
                    return this;
                case TokenType.AssignShr:
                    m_Value >>= val.ToInt32();
                    return this;
                case TokenType.AssignShi:
                    m_Value <<= val.ToInt32();
                    return this;
                default:
                    throw new ExecutionException(m_Script, this, "Int不支持的运算符 " + type);
            }
        }
       
        public override ScriptObject Clone()
        {
            return new ScriptNumberInt(m_Script, m_Value);
        }
    }
}
