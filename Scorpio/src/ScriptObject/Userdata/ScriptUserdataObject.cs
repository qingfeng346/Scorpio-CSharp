using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Function;

namespace Scorpio.Userdata {
    /// <summary> 普通Object类型 </summary>
    public class ScriptUserdataObject : ScriptUserdata {
        protected UserdataType m_UserdataType;
        protected Dictionary<string, ScriptValue> m_Methods = new Dictionary<string, ScriptValue>();
        public ScriptUserdataObject(object value, UserdataType type) {
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            this.m_UserdataType = type;
        }
        public override ScriptValue GetValue(string key) {
            if (m_Methods.TryGetValue(key, out var value)) {
                return value;
            }
            var ret = m_UserdataType.GetValue(m_Value, key);
            if (ret is UserdataMethod) {
                key = string.Intern(key);
                return m_Methods[key] = new ScriptValue(new ScriptInstanceMethodFunction((UserdataMethod)ret, m_Value, key));
            }
            return ScriptValue.CreateValue(ret);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(m_Value, key, value);
        }
        public override ScriptValue GetValue(object index) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            ScriptValue.Parameters[0] = ScriptValue.CreateValue(index);
            return ScriptValue.CreateValue(func.Call(false, m_Value, ScriptValue.Parameters, 1));
        }
        public override void SetValue(object index, ScriptValue value) {
            var func = m_UserdataType.GetOperator(UserdataOperator.SetItemIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            ScriptValue.Parameters[0] = ScriptValue.CreateValue(index);
            ScriptValue.Parameters[1] = value;
            func.Call(false, m_Value, ScriptValue.Parameters, 2);
        }
        public override string ToString() { return m_Value.ToString(); }
        public override bool Less(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.LessIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return (bool)func.Call(true, null, ScriptValue.Parameters, 2);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.LessOrEqualIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<=]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return (bool)func.Call(true, null, ScriptValue.Parameters, 2);
        }
        public override bool Greater(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GreaterIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return (bool)func.Call(true, null, ScriptValue.Parameters, 2);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.GreaterOrEqualIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>=]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return (bool)func.Call(true, null, ScriptValue.Parameters, 2);
        }
        public override bool Equals(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.EqualIndex);
            if (func != null) {
                ScriptValue.Parameters[0] = ThisValue;
                ScriptValue.Parameters[1] = obj;
                if (func.CallNoThrow(true, null, ScriptValue.Parameters, 2, out var result)) {
                    return (bool)result;
                }
            }
            switch (obj.valueType) {
                case ScriptValue.nullValueType: return false;
                case ScriptValue.scriptValueType: return m_Value == obj.scriptValue.Value;
                default: return false;
            }
        }
        public override ScriptValue Plus(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.PlusIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[+]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.MinusIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[-]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.MultiplyIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[*]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.DivideIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[/]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.ModuloIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[%]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.InclusiveOrIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[|]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.CombineIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[&]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.XORIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[^]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.ShiIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<<]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var func = m_UserdataType.GetOperator(UserdataOperator.ShrIndex);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>>]运算符重载");
            ScriptValue.Parameters[0] = ThisValue;
            ScriptValue.Parameters[1] = obj;
            return ScriptValue.CreateValue(func.Call(true, null, ScriptValue.Parameters, 2));
        }
    }
}
