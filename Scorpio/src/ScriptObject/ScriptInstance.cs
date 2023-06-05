using System.Collections;
using System.Collections.Generic;
using Scorpio.Library;
using Scorpio.Tools;

namespace Scorpio {
    public class ScriptInstance : ScriptObject, IEnumerable<KeyValuePair<string, ScriptValue>> {
        internal Dictionary<string, ScriptValue> m_Values = new Dictionary<string, ScriptValue>();         //所有的数据(函数和数据都在一个数组)
        protected ScriptValue m_PrototypeValue;
        protected ScriptType m_Prototype = null;
        public ScriptInstance(Script script) : base(script, ObjectType.Instance) { }
        public ScriptInstance(Script script, ObjectType objectType) : base(script, objectType) { }
        public ScriptInstance Set(ScriptValue prototypeValue) {
            m_PrototypeValue.CopyFrom(prototypeValue);
            m_Prototype = prototypeValue.Get<ScriptType>();
            return this;
        }
        public override string ValueTypeName => $"Object<{m_Prototype}>";            //变量名称
        public ScriptType Prototype { get { return m_Prototype; } set { m_Prototype = value; } }
        protected void Release() {
            m_PrototypeValue.Free();
            m_Prototype = null;
            m_Values.Free();
        }
        public override void Free() {
            Release();
            m_Script.Free(this);
        }
        public override ScriptValue GetValue(string key) {
            return m_Values.TryGetValue(key, out var value) ? value : m_Prototype.GetValue(key, this);
        }
        public void SetValue(string key, ScriptObject scriptObject) {
            using (var value = new ScriptValue(scriptObject)) {
                SetValue(key, value);
            }
        }
        public override void SetValue(string key, ScriptValue value) {
            if (m_Values.TryGetValue(key, out var result)) {
                result.Free();
            }
            //正常引用计数
            m_Values[key] = value.Reference();
        }
        public virtual bool HasValue(string key) {
            return m_Values.ContainsKey(key);
        }
        public IEnumerator<KeyValuePair<string, ScriptValue>> GetEnumerator() { return m_Values.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return m_Values.GetEnumerator(); }
        public override ScriptValue Plus(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Plus).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Plus(obj);
        }
        public override ScriptValue Minus(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Minus).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Minus(obj);
        }
        public override ScriptValue Multiply(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Multiply).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Multiply(obj);
        }
        public override ScriptValue Divide(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Divide).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Divide(obj);
        }
        public override ScriptValue Modulo(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Modulo).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Modulo(obj);
        }
        public override ScriptValue InclusiveOr(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.InclusiveOr).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.InclusiveOr(obj);
        }
        public override ScriptValue Combine(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Combine).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Combine(obj);
        }
        public override ScriptValue XOR(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.XOR).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.XOR(obj);
        }
        public override ScriptValue Shi(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Shi).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Shi(obj);
        }
        public override ScriptValue Shr(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Shr).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1);
                }
            }
            return base.Shr(obj);
        }
        public override bool Greater(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Greater).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1).valueType == ScriptValue.trueValueType;
                }
            }
            return base.Greater(obj);
        }
        public override bool GreaterOrEqual(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.GreaterOrEqual).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1).valueType == ScriptValue.trueValueType;
                }
            }
            return base.GreaterOrEqual(obj);
        }
        public override bool Less(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.Less).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1).valueType == ScriptValue.trueValueType;
                }
            }
            return base.Less(obj);
        }
        public override bool LessOrEqual(ScriptValue obj) {
            var func = m_Prototype.GetValue(ScriptOperator.LessOrEqual).Get<ScriptFunction>();
            if (func != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return func.Call(ThisValue, parameters.values, 1).valueType == ScriptValue.trueValueType;
                }
            }
            return base.LessOrEqual(obj);
        }
        public override bool Equals(ScriptValue obj) {
            if (m_Prototype.EqualFunction != null) {
                using (var parameters = ScorpioParameters.Get()) {
                    parameters[0] = obj;
                    return m_Prototype.EqualFunction.Call(ThisValue, parameters.values, 1).valueType == ScriptValue.trueValueType;
                }
            }
            return base.Equals(obj);
        }
        public ScriptValue Call(ScriptValue parameter1) {
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = parameter1;
                return Call(ScriptValue.Null, parameters.values, 1);
            }
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2) {
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = parameter1;
                parameters[1] = parameter2;
                return Call(ScriptValue.Null, parameters.values, 2);
            }
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3) {
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = parameter1;
                parameters[1] = parameter2;
                parameters[2] = parameter3;
                return Call(ScriptValue.Null, parameters.values, 3);
            }
        }
        public ScriptValue Call(ScriptValue parameter1, ScriptValue parameter2, ScriptValue parameter3, ScriptValue parameter4) {
            using (var parameters = ScorpioParameters.Get()) {
                parameters[0] = parameter1;
                parameters[1] = parameter2;
                parameters[2] = parameter3;
                parameters[3] = parameter4;
                return Call(ScriptValue.Null, parameters.values, 4);
            }
        }
        public ScriptValue Call(ScriptValue[] parameters, int length) {
            return Call(ScriptValue.Null, parameters, length);
        }
        public override ScriptValue Call(ScriptValue thisObject, ScriptValue[] parameters, int length) {
            var func = m_Prototype.GetValue(ScriptOperator.Invoke).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(ThisValue, parameters, length);
            }
            return base.Call(thisObject, parameters, length);
        }
        public override string ToString() {
            var func = m_Prototype.GetValueNoDefault(ScriptOperator.toString).Get<ScriptFunction>();
            if (func != null) {
                return func.Call(ThisValue, ScriptValue.EMPTY, 0).ToString();
            }
            return $"Object<{m_Prototype}>";
        }
        internal virtual void ToJson(ScorpioJsonSerializer jsonSerializer) {
            var builder = jsonSerializer.m_Builder;
            builder.Append("{");
            var first = true;
            foreach (var pair in m_Values) {
                if (first) { first = false; } else { builder.Append(","); }
                jsonSerializer.Serializer(pair.Key);
                builder.Append(":");
                jsonSerializer.Serializer(pair.Value);
            }
            builder.Append("}");
        }
    }
}
