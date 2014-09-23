using System;
using System.Collections.Generic;
using System.Text;
using Scorpio.CodeDom;
using Scorpio.Variable;
using Scorpio.Compiler;
using Scorpio.Exception;
namespace Scorpio
{
    //脚本数字类型
    public class ScriptNumber : IScriptPrimitiveObject
    {
        private enum NUMBER_TYPE
        {
            DOUBLE,
            LONG,
            ULONG,
        }
        private NUMBER_TYPE m_NumberType;
        private double m_Double;
        private long m_Long;
        private ulong m_ULong;
        public ScriptNumber(double value)
        {
            Initialize(value);
        }
        public ScriptNumber(long value)
        {
            Initialize(value);
        }
        public ScriptNumber(ulong value)
        {
            Initialize(value);
        }
        private void Initialize(object value)
        {
            if (value is long) {
                m_Long = (long)value;
                m_NumberType = NUMBER_TYPE.LONG;
            } else if (value is ulong) {
                m_ULong = (ulong)value;
                m_NumberType = NUMBER_TYPE.ULONG;
            } else {
                m_Double = Convert.ToDouble(value);
                m_NumberType = NUMBER_TYPE.DOUBLE;
            }
            Type = ObjectType.Number;
        }
        public override object ObjectValue
        {
            get 
            {
                if (m_NumberType == NUMBER_TYPE.DOUBLE)
                    return m_Double;
                else if (m_NumberType == NUMBER_TYPE.LONG)
                    return m_Long;
                return m_ULong;
            }
        }
        public ScriptNumber Calc(CALC c)
        {
            switch (c)
            {
                case CALC.PRE_INCREMENT:
                    if (m_NumberType == NUMBER_TYPE.DOUBLE)
                        ++m_Double;
                    else if (m_NumberType == NUMBER_TYPE.LONG)
                        ++m_Long;
                    else if (m_NumberType == NUMBER_TYPE.ULONG)
                        ++m_ULong;
                    break;
                case CALC.PRE_DECREMENT:
                    if (m_NumberType == NUMBER_TYPE.DOUBLE)
                        --m_Double;
                    else if (m_NumberType == NUMBER_TYPE.LONG)
                        --m_Long;
                    else if (m_NumberType == NUMBER_TYPE.ULONG)
                        --m_ULong;
                    break;
                case CALC.POST_INCREMENT:
                    if (m_NumberType == NUMBER_TYPE.DOUBLE)
                        return new ScriptNumber(m_Double++);
                    else if (m_NumberType == NUMBER_TYPE.LONG)
                        return new ScriptNumber(m_Long++);
                    else if (m_NumberType == NUMBER_TYPE.ULONG)
                        return new ScriptNumber(m_ULong++);
                    break;
                case CALC.POST_DECREMENT:
                    if (m_NumberType == NUMBER_TYPE.DOUBLE)
                        return new ScriptNumber(m_Double--);
                    else if (m_NumberType == NUMBER_TYPE.LONG)
                        return new ScriptNumber(m_Long--);
                    else if (m_NumberType == NUMBER_TYPE.ULONG)
                        return new ScriptNumber(m_ULong--);
                    break;
            }
            return this;
        }
        public ScriptNumber Negative()
        {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double = -m_Double;
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long = -m_Long;
            return this;
        }
        public int ToInt32()
        {
            return Convert.ToInt32(ObjectValue);
        }
        public double ToDouble()
        {
            return Convert.ToDouble(ObjectValue);
        }
        public long ToLong()
        {
            return Convert.ToInt64(ObjectValue);
        }
        public ulong ToULong()
        {
            return Convert.ToUInt64(ObjectValue);
        }
        public override void Assign(ScriptObject obj)
        {
            Initialize(((ScriptNumber)obj).ObjectValue);
        }
        public override ScriptObject Plus(ScriptObject obj) {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double += ((ScriptNumber)obj).ToDouble();
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long += ((ScriptNumber)obj).ToLong();
            else
                m_ULong += ((ScriptNumber)obj).ToULong();
            return this;
        }
        public override ScriptObject Minus(ScriptObject obj)
        {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double -= ((ScriptNumber)obj).ToDouble();
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long -= ((ScriptNumber)obj).ToLong();
            else
                m_ULong -= ((ScriptNumber)obj).ToULong();
            return this;
        }
        public override ScriptObject Multiply(ScriptObject obj)
        {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double *= ((ScriptNumber)obj).ToDouble();
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long *= ((ScriptNumber)obj).ToLong();
            else
                m_ULong *= ((ScriptNumber)obj).ToULong();
            return this;
        }
        public override ScriptObject Divide(ScriptObject obj)
        {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double /= ((ScriptNumber)obj).ToDouble();
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long /= ((ScriptNumber)obj).ToLong();
            else
                m_ULong /= ((ScriptNumber)obj).ToULong();
            return this;
        }
        public override ScriptObject Modulo(ScriptObject obj)
        {
            if (m_NumberType == NUMBER_TYPE.DOUBLE)
                m_Double %= ((ScriptNumber)obj).ToDouble();
            else if (m_NumberType == NUMBER_TYPE.LONG)
                m_Long %= ((ScriptNumber)obj).ToLong();
            else
                m_ULong %= ((ScriptNumber)obj).ToULong();
            return this;
        }
        public bool Compare(TokenType type, CodeOperator oper, ScriptNumber num)
        {
            if (m_NumberType != num.m_NumberType) 
                throw new ExecutionException("数字比较 两边的数字类型不一致 请先转换再比较 ", oper);
            if (m_NumberType == NUMBER_TYPE.DOUBLE) {
                if (type == TokenType.Equal)
                    return m_Double == num.m_Double;
                else if (type == TokenType.NotEqual)
                    return m_Double != num.m_Double;
                else if (type == TokenType.Greater)
                    return m_Double > num.m_Double;
                else if (type == TokenType.GreaterOrEqual)
                    return m_Double >= num.m_Double;
                else if (type == TokenType.Less)
                    return m_Double < num.m_Double;
                else if (type == TokenType.LessOrEqual)
                    return m_Double <= num.m_Double;
            } else if (m_NumberType == NUMBER_TYPE.LONG) {
                if (type == TokenType.Equal)
                    return m_Long == num.m_Long;
                else if (type == TokenType.NotEqual)
                    return m_Long != num.m_Long;
                else if (type == TokenType.Greater)
                    return m_Long > num.m_Long;
                else if (type == TokenType.GreaterOrEqual)
                    return m_Long >= num.m_Long;
                else if (type == TokenType.Less)
                    return m_Long < num.m_Long;
                else if (type == TokenType.LessOrEqual)
                    return m_Long <= num.m_Long;
            } else if (m_NumberType == NUMBER_TYPE.ULONG) {
                if (type == TokenType.Equal)
                    return m_ULong == num.m_ULong;
                else if (type == TokenType.NotEqual)
                    return m_ULong != num.m_ULong;
                else if (type == TokenType.Greater)
                    return m_ULong > num.m_ULong;
                else if (type == TokenType.GreaterOrEqual)
                    return m_ULong >= num.m_ULong;
                else if (type == TokenType.Less)
                    return m_ULong < num.m_ULong;
                else if (type == TokenType.LessOrEqual)
                    return m_ULong <= num.m_ULong;
            }
            return false;
        }

    }
}
