using System.Collections.Generic;
using Scorpio.Exception;
using Scorpio.Function;

namespace Scorpio.Userdata {
    /// <summary> 普通Object类型 </summary>
    public class ScriptUserdataObject : ScriptUserdata {
        protected UserdataType m_UserdataType;
        protected Dictionary<string, ScriptValue> m_Methods = new Dictionary<string, ScriptValue>();
        protected Dictionary<string, ScriptMethodFunction> m_Operators = new Dictionary<string, ScriptMethodFunction>();
        public ScriptUserdataObject(object value, UserdataType type) {
            this.m_Value = value;
            this.m_ValueType = value.GetType();
            this.m_UserdataType = type;
        }
        public override ScriptValue GetValue(string key) {
            if (m_Methods.ContainsKey(key)) return m_Methods[key];
            var ret = m_UserdataType.GetValue(m_Value, key);
            if (ret is UserdataMethod) {
                return m_Methods[key] = new ScriptValue(new ScriptInstanceMethodFunction((UserdataMethod)ret, m_Value));
            }
            return ScriptValue.CreateObject(ret);
        }
        public override void SetValue(string key, ScriptValue value) {
            m_UserdataType.SetValue(m_Value, key, value);
        }
        public override ScriptValue GetValue(object index) {
            var func = GetOperator(UserdataOperator.GetItem);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] get ]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { ScriptValue.CreateObject(index) }, 1);
        }
        public override void SetValue(object index, ScriptValue value) {
            var func = GetOperator(UserdataOperator.SetItem);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[ [] set ]运算符重载");
            func.Call(ScriptValue.Null, new ScriptValue[] { ScriptValue.CreateObject(index), value }, 2);
        }
        public override string ToString() { return m_Value.ToString(); }
        ScriptMethodFunction GetOperator(string oper) {
            if (m_Operators.ContainsKey(oper)) return m_Operators[oper];
            var ret = m_UserdataType.GetValue(m_Value, oper);
            if (ret is UserdataMethod) {
                switch (oper) {
                    case UserdataOperator.SetItem:
                    case UserdataOperator.GetItem:
                        return m_Operators[oper] = new ScriptInstanceMethodFunction((UserdataMethod)ret, m_Value);
                    default:
                        return m_Operators[oper] = new ScriptStaticMethodFunction((UserdataMethod)ret);
                }
            }
            return m_Operators[oper] = null;
        }
        public override bool Less(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Less);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2).valueType == ScriptValue.trueValueType;
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.LessOrEqual);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<=]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2).valueType == ScriptValue.trueValueType;
        }
        public override bool Greater(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Greater);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2).valueType == ScriptValue.trueValueType;
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.GreaterOrEqual);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>=]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2).valueType == ScriptValue.trueValueType;
        }
        public override bool Equals(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Equal);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[==]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2).valueType == ScriptValue.trueValueType;
        }
        public override ScriptValue Plus(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Plus);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[+]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Minus);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[-]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Multiply);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[*]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Divide);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[/]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Modulo);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[%]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.InclusiveOr);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[|]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Combine);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[&]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.XOR);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[^]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Shi);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[<<]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var func = GetOperator(UserdataOperator.Shr);
            if (func == null) throw new ExecutionException($"类[{m_ValueType.Name}]找不到[>>]运算符重载");
            return func.Call(ScriptValue.Null, new ScriptValue[] { new ScriptValue(this), obj }, 2);
        }
    }
}
