using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Tools;

namespace Scorpio.Userdata {
    /// <summary> 普通Object类型 </summary>
    public class ScriptUserdataObject : ScriptUserdata {
        protected UserdataType m_UserdataType;
        protected Dictionary<string, ScriptValue> m_Methods = new Dictionary<string, ScriptValue>();
        public ScriptUserdataObject(Script script) : base(script) { }
        public ScriptUserdataObject Set(UserdataType type, object value) {
            m_UserdataType = type;
            m_Value = value;
            m_ValueType = value.GetType();
            return this;
        }
        public override void Free() {
            m_Methods.Free();
            m_Script.Free(this);
        }
        public override ScriptValue GetValue(string key) {
            if (m_Methods.TryGetValue(key, out var value)) {
                return value;
            }
            var ret = m_UserdataType.GetValue(m_Script, m_Value, key);
            if (ret is UserdataMethod) {
                return m_Methods[key] = new ScriptValue(m_Script.NewInstanceMethod().Set(key, (UserdataMethod)ret, m_Value));
            }
            return ScriptValue.CreateValue(m_Script, ret);
        }
        public override void SetValue(string key, ScriptValue value) { 
            m_UserdataType.SetValue(m_Value, key, value);
        }
        public override ScriptValue GetValue(double index) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = new ScriptValue(index);
                using (var ret = ScriptValue.CreateValue(m_Script, func.Call(m_Script, false, m_Value, parameters.values, 1)))
                    return ret;
            }
        }
        public override ScriptValue GetValue(long index) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = new ScriptValue(index);
                using (var ret = ScriptValue.CreateValue(m_Script, func.Call(m_Script, false, m_Value, parameters.values, 1)))
                    return ret;
            }
        }
        public override ScriptValue GetValue(object index) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = ScriptValue.CreateValue(m_Script, index);
                using (var ret = ScriptValue.CreateValue(m_Script, func.Call(m_Script, false, m_Value, parameters.values, 1)))
                    return ret;
            }
        }
        public override void SetValue(double index, ScriptValue value) {
            var func = m_UserdataType.GetOperator(UserdataOperator.SetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = new ScriptValue(index);
                parameters[1] = value;
                func.Call(m_Script, false, m_Value, parameters.values, 2);
            }
        }
        public override void SetValue(long index, ScriptValue value) {
            var func = m_UserdataType.GetOperator(UserdataOperator.SetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = new ScriptValue(index);
                parameters[1] = value;
                func.Call(m_Script, false, m_Value, parameters.values, 2);
            }
        }
        public override void SetValue(object index, ScriptValue value) {
            var func = m_UserdataType.GetOperator(UserdataOperator.SetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters.values[0] = ScriptValue.CreateValue(m_Script, index);
                parameters[1] = value;
                func.Call(m_Script, false, m_Value, parameters.values, 2);
            }
        }
        public override string ToString() { return m_Value.ToString(); }
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
        public override bool Equals(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.EqualIndex);
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = ThisValue;
                    parameters[1] = obj;
                    if (func.CallNoThrow(m_Script, true, null, parameters.values, 2, out var result)) {
                        return (bool)result;
                    }
                }
            }
            switch (obj.valueType) {
                case ScriptValue.nullValueType: return false;
                case ScriptValue.scriptValueType: return m_Value == obj.scriptValue.Value;
                default: return false;
            }
        }
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
        bool CallCompare(int operatorIndex, ScriptValue obj) {
            var func = m_UserdataType.GetOperator(operatorIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[{UserdataOperator.GetOperatorByIndex(operatorIndex)}]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = ThisValue;
                parameters[1] = obj;
                return (bool)func.Call(m_Script, true, null, parameters.values, 2);
            }
        }
        ScriptValue CallOperator(int operatorIndex, ScriptValue obj) {
            var func = m_UserdataType.GetOperator(operatorIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[{UserdataOperator.GetOperatorByIndex(operatorIndex)}]运算符重载");
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = ThisValue;
                parameters[1] = obj;
                return ScriptValue.CreateValue(m_Script, func.Call(m_Script, true, null, parameters.values, 2));
            }
        }
    }
}
