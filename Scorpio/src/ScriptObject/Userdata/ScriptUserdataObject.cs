using Scorpio.Exception;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    /// <summary> 普通Object类型 </summary>
    public class ScriptUserdataObject : ScriptUserdata {
        protected UserdataType m_UserdataType;
        public ScriptUserdataObject(object value, UserdataType type) {
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            this.m_UserdataType = type;
        }
        public override ScriptValue GetValue(string key) {
            return ScriptValue.CreateValue(m_UserdataType.GetInstanceValue(key, m_Value));
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(m_Value, key, value);
        }
        #region [] get set 重载
        public override ScriptValue GetValue(double index) {
            return GetValueInternal(new ScriptValue(index));
        }
        public override ScriptValue GetValue(long index) {
            return GetValueInternal(new ScriptValue(index));
        }
        public override ScriptValue GetValue(object index) {
            return GetValueInternal(ScriptValue.CreateValue(index));
        }
        private ScriptValue GetValueInternal(ScriptValue key) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            ScorpioUtil.Parameters[0] = key;
            return ScriptValue.CreateValue(func.Call(false, m_Value, ScorpioUtil.Parameters, 1));
        }

        public override void SetValue(double index, ScriptValue value) {
            SetValueInternal(new ScriptValue(index), value);
        }
        public override void SetValue(long index, ScriptValue value) {
            SetValueInternal(new ScriptValue(index), value);
        }
        public override void SetValue(object index, ScriptValue value) {
            SetValueInternal(ScriptValue.CreateValue(index), value);
        }
        private void SetValueInternal(ScriptValue key, ScriptValue value) {
            var func = m_UserdataType.GetOperator(UserdataOperator.SetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = key;
            parameters[1] = value;
            func.Call(false, m_Value, parameters, 2);
        }
        #endregion
        #region 运算符重载
        public override ScriptValue Plus(ScriptValue obj) {
            return CallOperator(UserdataOperator.PlusIndex, obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            return CallOperator(UserdataOperator.MinusIndex, obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            return CallOperator(UserdataOperator.MultiplyIndex, obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            return CallOperator(UserdataOperator.DivideIndex, obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            return CallOperator(UserdataOperator.ModuloIndex, obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            return CallOperator(UserdataOperator.InclusiveOrIndex, obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            return CallOperator(UserdataOperator.CombineIndex, obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            return CallOperator(UserdataOperator.XORIndex, obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            return CallOperator(UserdataOperator.ShiIndex, obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            return CallOperator(UserdataOperator.ShrIndex, obj);
        }
        public override bool Less(ScriptValue obj) {
            return CallCompare(UserdataOperator.LessIndex, obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            return CallCompare(UserdataOperator.LessOrEqualIndex, obj);
        }
        public override bool Greater(ScriptValue obj) {
            return CallCompare(UserdataOperator.GreaterIndex, obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            return CallCompare(UserdataOperator.GreaterOrEqualIndex, obj);
        }
        //public override bool Equals(ScriptValue obj) {
        //    //var func = m_UserdataType.GetOperator(UserdataOperator.EqualIndex);
        //    //if (func != null) {
        //    //    var parameters = ScorpioUtil.Parameters;
        //    //    parameters[0] = ThisValue;
        //    //    parameters[1] = obj;
        //    //    if (func.CallNoThrow(true, null, parameters, 2, out var result)) {
        //    //        return (bool)result;
        //    //    }
        //    //}
        //    //switch (obj.valueType) {
        //    //    case ScriptValue.nullValueType: return false;
        //    //    case ScriptValue.scriptValueType: return m_Value == obj.scriptValue.Value;
        //    //    default: return false;
        //    //}
        //}
        bool CallCompare(int operatorIndex, ScriptValue obj) {
            var func = m_UserdataType.GetOperator(operatorIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[{UserdataOperator.GetOperatorByIndex(operatorIndex)}]运算符重载");
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = ThisValue;
            parameters[1] = obj;
            return (bool)func.Call(true, null, parameters, 2);
        }
        ScriptValue CallOperator(int operatorIndex, ScriptValue obj) {
            var func = m_UserdataType.GetOperator(operatorIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[{UserdataOperator.GetOperatorByIndex(operatorIndex)}]运算符重载");
            var parameters = ScorpioUtil.Parameters;
            parameters[0] = ThisValue;
            parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, parameters, 2));
        }
        #endregion
    }
}
