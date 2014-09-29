using System;
using System.Collections.Generic;
using System.Text;
using Scorpio;
using Scorpio.CodeDom;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio.Variable
{
    public class ScriptNumberULong : ScriptNumber
    {
        public override ObjectType Type { get { return ObjectType.Number; } }
        public override int BranchType { get { return 2; } }
        public override object ObjectValue { get { return Value; } }
        public ulong Value { get; private set; }
        private Script m_Script;
        public ScriptNumberULong(Script script, ulong value)
        {
            m_Script = script;
            Value = value;
        }
        public override ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    ++Value;
                    break;
                case CALC.PRE_DECREMENT:
                    --Value;
                    break;
                case CALC.POST_INCREMENT:
                    return m_Script.CreateNumber(Value++);
                case CALC.POST_DECREMENT:
                    return m_Script.CreateNumber(Value--);
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            return this;
        }
        public override ulong ToULong()
        {
            return Value;
        }
        public override ScriptObject Plus(ScriptObject obj)
        {
            return new ScriptNumberULong(m_Script, Value + ((ScriptNumber)obj).ToULong());
        }
        public override ScriptObject Minus(ScriptObject obj)
        {
            return new ScriptNumberULong(m_Script, Value - ((ScriptNumber)obj).ToULong());
        }
        public override ScriptObject Multiply(ScriptObject obj)
        {
            return new ScriptNumberULong(m_Script, Value * ((ScriptNumber)obj).ToULong());
        }
        public override ScriptObject Divide(ScriptObject obj)
        {
            return new ScriptNumberULong(m_Script, Value / ((ScriptNumber)obj).ToULong());
        }
        public override ScriptObject Modulo(ScriptObject obj)
        {
            return new ScriptNumberULong(m_Script, Value % ((ScriptNumber)obj).ToULong());
        }
        public override bool Compare(TokenType type, CodeOperator oper, ScriptNumber num)
        {
            ScriptNumberULong val = num as ScriptNumberULong;
            if (val == null) throw new ExecutionException("数字比较 两边的数字类型不一致 请先转换再比较 ");
            switch (type)
            {
                case TokenType.Equal:
                    return Value == val.Value;
                case TokenType.NotEqual:
                    return Value != val.Value;
                case TokenType.Greater:
                    return Value > val.Value;
                case TokenType.GreaterOrEqual:
                    return Value >= val.Value;
                case TokenType.Less:
                    return Value < val.Value;
                case TokenType.LessOrEqual:
                    return Value <= val.Value;
            }
            return false;
        }
        public override string ToString()
        {
            return ObjectValue.ToString();
        }
        public override ScriptObject Clone()
        {
            return new ScriptNumberULong(m_Script, Value);
        }
    }
}
