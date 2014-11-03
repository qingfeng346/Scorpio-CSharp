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
        public override object ObjectValue { get { return Value; } }
        public long Value { get; private set; }
        public ScriptNumberLong(Script script, long value) : base(script)
        {
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
                    return Script.CreateLong(Value++);
                case CALC.POST_DECREMENT:
                    return Script.CreateLong(Value--);
            }
            return this;
        }
        public override ScriptNumber Negative()
        {
            Value = -Value;
            return this;
        }
        public override ScriptObject Assign()
        {
            return Script.CreateLong(Value);
        }
        public override long ToLong()
        {
            return Value;
        }
        public override ScriptObject Plus(ScriptNumber obj)
        {
            return Script.CreateLong(Value + obj.ToLong());
        }
        public override ScriptObject Minus(ScriptNumber obj)
        {
            return Script.CreateLong(Value - obj.ToLong());
        }
        public override ScriptObject Multiply(ScriptNumber obj)
        {
            return Script.CreateLong(Value * obj.ToLong());
        }
        public override ScriptObject Divide(ScriptNumber obj)
        {
            return Script.CreateLong(Value / obj.ToLong());
        }
        public override ScriptObject Modulo(ScriptNumber obj)
        {
            return Script.CreateLong(Value % obj.ToLong());
        }
        public override ScriptObject AssignPlus(ScriptNumber obj)
        {
            Value += obj.ToLong();
            return this;
        }
        public override ScriptObject AssignMinus(ScriptNumber obj)
        {
            Value -= obj.ToLong();
            return this;
        }
        public override ScriptObject AssignMultiply(ScriptNumber obj)
        {
            Value *= obj.ToLong();
            return this;
        }
        public override ScriptObject AssignDivide(ScriptNumber obj)
        {
            Value /= obj.ToLong();
            return this;
        }
        public override ScriptObject AssignModulo(ScriptNumber obj)
        {
            Value %= obj.ToLong();
            return this;
        }
        public override bool Compare(TokenType type, CodeOperator oper, ScriptNumber num)
        {
            ScriptNumberLong val = num as ScriptNumberLong;
            if (val == null) throw new ExecutionException("数字比较 两边的数字类型不一致 请先转换再比较 ");
            switch (type)
            {
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
        public override ScriptObject Clone()
        {
            return Script.CreateLong(Value);
        }
    }
}
